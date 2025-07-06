namespace SARS.TaxCalculator.Models.Results;

/// <summary>
/// Represents UIF contribution details
/// </summary>
public class UifContribution
{
    /// <summary>
    /// Employee contribution amount
    /// </summary>
    public decimal EmployeeAmount { get; init; }

    /// <summary>
    /// Employer contribution amount
    /// </summary>
    public decimal EmployerAmount { get; init; }

    /// <summary>
    /// Total UIF contribution (employee + employer)
    /// </summary>
    public decimal TotalAmount { get; init; }

    /// <summary>
    /// The income amount used for contribution calculation (after ceiling applied)
    /// </summary>
    public decimal ContributionBase { get; init; }

    /// <summary>
    /// Indicates whether the UIF ceiling was applied
    /// </summary>
    public bool CeilingApplied { get; init; }
}
