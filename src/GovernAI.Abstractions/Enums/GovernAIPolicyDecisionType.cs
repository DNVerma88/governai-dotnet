namespace GovernAI.Abstractions;

/// <summary>
/// Represents the policy decision type for a GovernAI-tracked AI operation.
/// </summary>
public enum GovernAIPolicyDecisionType
{
    /// <summary>The AI operation is allowed to proceed.</summary>
    Allow = 0,

    /// <summary>The AI operation requires manual review before proceeding.</summary>
    Review = 1,

    /// <summary>The AI operation is denied and must not proceed.</summary>
    Deny = 2
}
