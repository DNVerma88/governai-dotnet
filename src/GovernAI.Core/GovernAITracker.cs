using System.Diagnostics;
using GovernAI.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GovernAI.Core;

/// <summary>
/// The GovernAI runtime engine. Tracks AI operations, enforces policies, hashes prompts/responses,
/// and exports <see cref="GovernAIEvent"/> records.
/// </summary>
public sealed class GovernAITracker
{
    private readonly GovernAIOptions _options;
    private readonly IGovernAIClock _clock;
    private readonly IGovernAIPolicyEvaluator _policyEvaluator;
    private readonly IGovernAIExporter _exporter;
    private readonly IGovernAITenantResolver? _tenantResolver;
    private readonly IGovernAIUserResolver? _userResolver;
    private readonly ILogger<GovernAITracker> _logger;

    // Maximum length for label fields stored in events, to prevent log injection.
    private const int MaxLabelLength = 200;

    /// <summary>
    /// Initializes a new instance of <see cref="GovernAITracker"/>.
    /// </summary>
    public GovernAITracker(
        GovernAIOptions options,
        IGovernAIClock clock,
        IGovernAIPolicyEvaluator policyEvaluator,
        IGovernAIExporter exporter,
        IGovernAITenantResolver? tenantResolver = null,
        IGovernAIUserResolver? userResolver = null,
        ILogger<GovernAITracker>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _policyEvaluator = policyEvaluator ?? throw new ArgumentNullException(nameof(policyEvaluator));
        _exporter = exporter ?? throw new ArgumentNullException(nameof(exporter));
        _tenantResolver = tenantResolver;
        _userResolver = userResolver;
        _logger = logger ?? NullLogger<GovernAITracker>.Instance;
    }

    /// <summary>
    /// Tracks an AI operation, enforcing policies, measuring duration, hashing
    /// prompt/response, and exporting a <see cref="GovernAIEvent"/>.
    /// </summary>
    /// <typeparam name="TResult">The result type returned by the AI operation.</typeparam>
    /// <param name="context">The context describing the AI operation.</param>
    /// <param name="operation">
    /// A delegate representing the AI call. Receives a <see cref="CancellationToken"/>
    /// and must return the response string and any additional result.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The result produced by <paramref name="operation"/>.</returns>
    /// <exception cref="GovernAIDeniedException">
    /// Thrown when the policy evaluator returns <see cref="GovernAIPolicyDecisionType.Deny"/>.
    /// </exception>
    public async Task<TResult> TrackAsync<TResult>(
        GovernAIContext context,
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(operation);

        // Resolve tenant/user if not explicitly set
        var tenantId = context.TenantId
            ?? (_tenantResolver is not null
                ? await _tenantResolver.ResolveTenantIdAsync(cancellationToken).ConfigureAwait(false)
                : null);

        var userId = context.UserId
            ?? (_userResolver is not null
                ? await _userResolver.ResolveUserIdAsync(cancellationToken).ConfigureAwait(false)
                : null);

        // Evaluate policy
        var policyDecision = await _policyEvaluator
            .EvaluateAsync(context, cancellationToken)
            .ConfigureAwait(false);

        // Deny — do not execute the operation
        if (policyDecision.Decision == GovernAIPolicyDecisionType.Deny)
        {
            var deniedEvent = BuildEvent(
                context, tenantId, userId, policyDecision,
                promptHash: HashPrompt(context.Prompt),
                responseHash: null,
                response: null,
                durationMs: 0,
                success: false,
                errorCode: "POLICY_DENIED",
                errorMessage: policyDecision.Reason ?? "Operation denied by policy");

            await ExportSafeAsync(deniedEvent, cancellationToken).ConfigureAwait(false);

            throw new GovernAIDeniedException(
                policyDecision.Reason ?? "The AI operation was denied by policy.",
                policyDecision);
        }

        // Execute the operation
        var sw = Stopwatch.StartNew();
        string? responseText = null;
        TResult result;

        try
        {
            result = await operation(cancellationToken).ConfigureAwait(false);

            // Extract response string if TResult is string
            responseText = result as string;
        }
        catch (Exception ex)
        {
            sw.Stop();

            var failureEvent = BuildEvent(
                context, tenantId, userId, policyDecision,
                promptHash: HashPrompt(context.Prompt),
                responseHash: null,
                response: null,
                durationMs: sw.ElapsedMilliseconds,
                success: false,
                errorCode: ex.GetType().Name,
                errorMessage: SanitizeErrorMessage(ex.Message));

            await ExportSafeAsync(failureEvent, cancellationToken).ConfigureAwait(false);

            throw;
        }

        sw.Stop();

        // Build and export success event
        var successEvent = BuildEvent(
            context, tenantId, userId, policyDecision,
            promptHash: HashPrompt(context.Prompt),
            responseHash: HashResponse(responseText),
            response: responseText,
            durationMs: sw.ElapsedMilliseconds,
            success: true,
            errorCode: null,
            errorMessage: null);

        await ExportSafeAsync(successEvent, cancellationToken).ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// Tracks an AI operation without a return value.
    /// </summary>
    public async Task TrackAsync(
        GovernAIContext context,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        await TrackAsync<object?>(
            context,
            async ct =>
            {
                await operation(ct).ConfigureAwait(false);
                return null;
            },
            cancellationToken).ConfigureAwait(false);
    }

    private GovernAIEvent BuildEvent(
        GovernAIContext context,
        string? tenantId,
        string? userId,
        GovernAIPolicyDecision policyDecision,
        string promptHash,
        string? responseHash,
        string? response,
        long durationMs,
        bool success,
        string? errorCode,
        string? errorMessage)
    {
        var inputTokens = context.InputTokens;
        var outputTokens = context.OutputTokens;
        int? totalTokens = (inputTokens.HasValue && outputTokens.HasValue)
            ? inputTokens.Value + outputTokens.Value
            : null;

        return new GovernAIEvent
        {
            EventId = Guid.NewGuid().ToString("N"),
            TraceId = SanitizeLabel(context.TraceId),
            CorrelationId = SanitizeLabel(context.CorrelationId),
            ApplicationName = SanitizeLabel(context.ApplicationName ?? _options.ApplicationName) ?? _options.ApplicationName,
            EnvironmentName = SanitizeLabel(context.EnvironmentName ?? _options.EnvironmentName),
            TenantId = SanitizeLabel(tenantId),
            UserId = SanitizeLabel(userId),
            AgentName = SanitizeLabel(context.AgentName),
            OperationName = SanitizeLabel(context.OperationName) ?? context.OperationName,
            ModelProvider = SanitizeLabel(context.ModelProvider),
            ModelName = SanitizeLabel(context.ModelName),
            Metadata = context.Metadata,
            PromptHash = string.IsNullOrEmpty(promptHash) ? null : promptHash,
            ResponseHash = string.IsNullOrEmpty(responseHash) ? null : responseHash,
            RawPrompt = _options.AllowRawPromptCapture ? context.Prompt : null,
            RawResponse = _options.AllowRawResponseCapture ? response : null,
            InputTokens = inputTokens,
            OutputTokens = outputTokens,
            TotalTokens = totalTokens,
            RiskScore = policyDecision.RiskScore,
            RiskLevel = policyDecision.RiskLevel,
            RiskCategory = policyDecision.RiskCategory,
            PolicyDecision = policyDecision.Decision,
            PolicyReason = policyDecision.Reason,
            DurationMs = durationMs,
            Success = success,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage,
            TimestampUtc = _clock.UtcNow
        };
    }

    private string HashPrompt(string? prompt)
        => _options.EnablePromptHashing ? PromptHasher.Hash(prompt, _options.PromptHashKey) : string.Empty;

    private string HashResponse(string? response)
        => _options.EnableResponseHashing ? ResponseHasher.Hash(response, _options.PromptHashKey) : string.Empty;

    /// <summary>
    /// Strips control characters from a label field and enforces a maximum length
    /// to prevent log injection via event metadata.
    /// </summary>
    private static string? SanitizeLabel(string? value, int maxLength = MaxLabelLength)
    {
        if (value is null) return null;
        Span<char> buffer = stackalloc char[Math.Min(value.Length, maxLength)];
        var len = 0;
        foreach (var ch in value)
        {
            if (len >= maxLength) break;
            if (!char.IsControl(ch))
                buffer[len++] = ch;
        }
        return new string(buffer[..len]);
    }

    private static string SanitizeErrorMessage(string? message)
    {
        // Truncate to prevent large error payloads and strip control characters
        // to block log injection via exception messages that contain newlines.
        if (string.IsNullOrEmpty(message))
        {
            return string.Empty;
        }

        const int maxErrorLength = 500;
        var sanitized = SanitizeLabel(message, maxErrorLength);
        return sanitized ?? string.Empty;
    }

    private async ValueTask ExportSafeAsync(GovernAIEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            await _exporter.ExportAsync(@event, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex) when (!_options.FailOnExporterError)
        {
            // Log the failure so operators can detect gaps in the audit trail
            // (VULN-11: silent swallowing would allow an adversary to eliminate audit evidence)
            _logger.LogError(ex,
                "GovernAI: exporter failed to persist audit event {EventId}. " +
                "The operation completed successfully but the governance record was not stored.",
                @event.EventId);
        }
    }
}
