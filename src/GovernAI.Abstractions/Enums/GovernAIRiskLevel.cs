namespace GovernAI.Abstractions;

/// <summary>
/// Represents the risk classification level for a GovernAI-tracked AI operation.
/// </summary>
public enum GovernAIRiskLevel
{
    /// <summary>No risk detected.</summary>
    None = 0,

    /// <summary>Low risk detected. Operation is generally safe.</summary>
    Low = 1,

    /// <summary>Medium risk detected. Operation may warrant attention.</summary>
    Medium = 2,

    /// <summary>High risk detected. Operation requires review before proceeding.</summary>
    High = 3,

    /// <summary>Critical risk detected. Operation should be denied.</summary>
    Critical = 4
}
