namespace SARS.TaxCalculator.Models;

/// <summary>
/// Types of tax rebates available
/// </summary>
public enum RebateType
{
    /// <summary>
    /// Primary rebate for all taxpayers
    /// </summary>
    Primary,
    
    /// <summary>
    /// Secondary rebate for taxpayers 65 years and older
    /// </summary>
    Secondary,
    
    /// <summary>
    /// Tertiary rebate for taxpayers 75 years and older
    /// </summary>
    Tertiary
}

/// <summary>
/// Represents a tax rebate with age requirements
/// </summary>
public class TaxRebate
{
    /// <summary>
    /// The type of rebate
    /// </summary>
    public RebateType Type { get; init; }
    
    /// <summary>
    /// The annual rebate amount
    /// </summary>
    public decimal Amount { get; init; }
    
    /// <summary>
    /// Minimum age requirement for this rebate (null for no requirement)
    /// </summary>
    public int? MinAge { get; init; }

    /// <summary>
    /// Checks if a person of given age qualifies for this rebate
    /// </summary>
    /// <param name="age">The person's age</param>
    /// <returns>True if qualified, false otherwise</returns>
    public bool QualifiesForRebate(int age)
    {
        return !MinAge.HasValue || age >= MinAge.Value;
    }
}