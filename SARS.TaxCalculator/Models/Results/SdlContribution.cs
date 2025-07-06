using System.Collections.Generic;

namespace SARS.TaxCalculator.Models.Results;

/// <summary>
/// Represents SDL contribution details
/// </summary>
public class SdlContribution
{
    /// <summary>
    /// SDL amount
    /// </summary>
    public decimal Amount { get; init; }
    
    /// <summary>
    /// Indicates whether the company is exempt from SDL
    /// </summary>
    public bool IsExempt { get; init; }
    
    /// <summary>
    /// SDL rate applied
    /// </summary>
    public decimal Rate { get; init; }
    
    /// <summary>
    /// Company's total annual payroll
    /// </summary>
    public decimal AnnualPayroll { get; init; }
    
    /// <summary>
    /// SDL exemption threshold
    /// </summary>
    public decimal ExemptionThreshold { get; init; }
}

/// <summary>
/// Represents bulk SDL calculation results
/// </summary>
public class SdlBulkContribution
{
    /// <summary>
    /// Total annual payroll
    /// </summary>
    public decimal TotalPayroll { get; init; }
    
    /// <summary>
    /// Total SDL amount
    /// </summary>
    public decimal TotalSdl { get; init; }
    
    /// <summary>
    /// Indicates whether the company is exempt
    /// </summary>
    public bool IsExempt { get; init; }
    
    /// <summary>
    /// Number of employees
    /// </summary>
    public int EmployeeCount { get; init; }
    
    /// <summary>
    /// Individual SDL contributions
    /// </summary>
    public IReadOnlyList<IndividualSdlContribution> IndividualContributions { get; init; } = new List<IndividualSdlContribution>();
}

/// <summary>
/// Represents individual employee SDL contribution
/// </summary>
public class IndividualSdlContribution
{
    /// <summary>
    /// Employee's annual salary
    /// </summary>
    public decimal AnnualSalary { get; init; }
    
    /// <summary>
    /// SDL amount for this employee
    /// </summary>
    public decimal SdlAmount { get; init; }
}