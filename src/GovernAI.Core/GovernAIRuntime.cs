using GovernAI.Abstractions;

namespace GovernAI.Core;

/// <summary>
/// The primary GovernAI runtime entry point.
/// Inject this class to track AI operations with policy enforcement, hashing, and event export.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="GovernAIRuntime"/> delegates all tracking logic to <see cref="GovernAITracker"/>
/// and serves as the public-facing API surface for application code.
/// </para>
/// <example>
/// <code>
/// var result = await governAI.TrackAsync(
///     context,
///     async cancellationToken =>
///     {
///         return await ExecuteAiCallAsync(context.Prompt, cancellationToken);
///     },
///     cancellationToken);
/// </code>
/// </example>
/// </remarks>
public sealed class GovernAIRuntime
{
    private readonly GovernAITracker _tracker;

    /// <summary>
    /// Initializes a new instance of <see cref="GovernAIRuntime"/>.
    /// </summary>
    /// <param name="tracker">The underlying tracker that executes policy and export logic.</param>
    public GovernAIRuntime(GovernAITracker tracker)
    {
        _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
    }

    /// <summary>
    /// Tracks an AI operation, enforcing policies, measuring duration, hashing
    /// prompt/response, and exporting a <see cref="GovernAIEvent"/>.
    /// </summary>
    /// <typeparam name="TResult">The result type returned by the AI operation.</typeparam>
    /// <param name="context">The context describing the AI operation.</param>
    /// <param name="operation">
    /// A delegate representing the AI call. Receives a <see cref="CancellationToken"/>
    /// and returns the AI response.
    /// </param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The result produced by <paramref name="operation"/>.</returns>
    /// <exception cref="GovernAIDeniedException">
    /// Thrown when the policy evaluator returns <see cref="GovernAIPolicyDecisionType.Deny"/>.
    /// </exception>
    public Task<TResult> TrackAsync<TResult>(
        GovernAIContext context,
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
        => _tracker.TrackAsync(context, operation, cancellationToken);

    /// <summary>
    /// Tracks an AI operation without a return value.
    /// </summary>
    /// <param name="context">The context describing the AI operation.</param>
    /// <param name="operation">A delegate representing the AI call.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <exception cref="GovernAIDeniedException">
    /// Thrown when the policy evaluator returns <see cref="GovernAIPolicyDecisionType.Deny"/>.
    /// </exception>
    public Task TrackAsync(
        GovernAIContext context,
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
        => _tracker.TrackAsync(context, operation, cancellationToken);
}
