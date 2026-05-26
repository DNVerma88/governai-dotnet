using GovernAI.Abstractions;

namespace GovernAI.Core;

/// <summary>
/// Exception thrown when a GovernAI policy evaluator denies an AI operation.
/// </summary>
public sealed class GovernAIDeniedException : Exception
{
    /// <summary>
    /// Gets the policy decision that caused the denial.
    /// </summary>
    public GovernAIPolicyDecision PolicyDecision { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="GovernAIDeniedException"/>.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="policyDecision">The policy decision that resulted in the denial.</param>
    public GovernAIDeniedException(string message, GovernAIPolicyDecision policyDecision)
        : base(message)
    {
        PolicyDecision = policyDecision ?? throw new ArgumentNullException(nameof(policyDecision));
    }
}
