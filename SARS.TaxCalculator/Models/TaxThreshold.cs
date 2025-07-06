namespace SARS.TaxCalculator.Models;

/// <summary>
/// Represents a tax threshold based on age
/// </summary>
public class TaxThreshold
{
    /// <summary>
    /// Minimum age for this threshold (null for no minimum)
    /// </summary>
    public int? MinAge { get; init; }
    
    /// <summary>
    /// Maximum age for this threshold (null for no maximum)
    /// </summary>
    public int? MaxAge { get; init; }
    
    /// <summary>
    /// The annual income threshold below which no tax is payable
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Checks if a person of given age falls within this threshold category
    /// </summary>
    /// <param name="age">The person's age</param>
    /// <returns>True if the age falls within the threshold range</returns>
    public bool AppliesTo(int age)
    {
        var meetsMinAge = !MinAge.HasValue || age >= MinAge.Value;
        var meetsMaxAge = !MaxAge.HasValue || age <= MaxAge.Value;
        return meetsMinAge && meetsMaxAge;
    }
}