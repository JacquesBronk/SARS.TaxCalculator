namespace SARS.TaxCalculator.Models.Inputs;

/// <summary>
/// Represents an employee for ETI calculation
/// </summary>
public class EtiEmployee
{
    /// <summary>
    /// Employee identifier
    /// </summary>
    public string EmployeeId { get; init; } = string.Empty;

    /// <summary>
    /// Employee age
    /// </summary>
    public int Age { get; init; }

    /// <summary>
    /// Monthly salary
    /// </summary>
    public decimal MonthlySalary { get; init; }

    /// <summary>
    /// Number of months employed
    /// </summary>
    public int EmploymentMonths { get; init; }

    /// <summary>
    /// Indicates if this is a first-time employee
    /// </summary>
    public bool IsFirstTimeEmployee { get; init; }

    /// <summary>
    /// Indicates if an employee works in a Special Economic Zone
    /// </summary>
    public bool WorksInSpecialEconomicZone { get; init; }

    /// <summary>
    /// Number of hours worked in the month (for ETI proration)
    /// If null, assumes 160+ hours (full ETI)
    /// </summary>
    public decimal? HoursWorkedInMonth { get; init; }
}
