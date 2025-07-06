namespace SARS.TaxCalculator.Models.Results;

/// <summary>
/// Complete tax calculation result
/// </summary>
public class TaxCalculationResult
{
    /// <summary>
    /// Monthly gross salary
    /// </summary>
    public decimal GrossSalary { get; init; }
    
    /// <summary>
    /// Monthly PAYE tax
    /// </summary>
    public decimal PAYE { get; init; }
    
    /// <summary>
    /// Monthly UIF employee contribution
    /// </summary>
    public decimal UIF { get; init; }
    
    /// <summary>
    /// Monthly SDL (employer pays)
    /// </summary>
    public decimal SDL { get; init; }
    
    /// <summary>
    /// Monthly ETI amount (reduces employer's PAYE liability)
    /// </summary>
    public decimal ETI { get; init; }
    
    /// <summary>
    /// Monthly retirement contribution
    /// </summary>
    public decimal RetirementContribution { get; init; }
    
    /// <summary>
    /// Monthly medical aid contribution
    /// </summary>
    public decimal MedicalAidContribution { get; init; }
    
    /// <summary>
    /// Monthly medical aid tax credit
    /// </summary>
    public decimal MedicalAidTaxCredit { get; init; }
    
    /// <summary>
    /// Monthly net salary (take-home pay)
    /// </summary>
    public decimal NetSalary { get; init; }
    
    /// <summary>
    /// Monthly employer UIF contribution
    /// </summary>
    public decimal EmployerUIF { get; init; }
    
    /// <summary>
    /// Monthly employer SDL contribution
    /// </summary>
    public decimal EmployerSDL { get; init; }
    
    /// <summary>
    /// Total monthly cost to company
    /// </summary>
    public decimal CostToCompany { get; init; }
    
    /// <summary>
    /// Tax year used for calculation
    /// </summary>
    public int TaxYear { get; init; }
    
    /// <summary>
    /// Employee age
    /// </summary>
    public int Age { get; init; }
    
    /// <summary>
    /// Annual gross salary
    /// </summary>
    public decimal AnnualGrossSalary { get; init; }
    
    /// <summary>
    /// Annual PAYE
    /// </summary>
    public decimal AnnualPAYE { get; init; }
    
    /// <summary>
    /// Whether UIF ceiling was applied
    /// </summary>
    public bool UifCeilingApplied { get; init; }
    
    /// <summary>
    /// Whether company is SDL exempt
    /// </summary>
    public bool SdlExempt { get; init; }
}

/// <summary>
/// PAYE-only calculation result
/// </summary>
public class PayeOnlyResult
{
    /// <summary>
    /// Monthly PAYE amount
    /// </summary>
    public decimal MonthlyPAYE { get; init; }
    
    /// <summary>
    /// Annual PAYE amount
    /// </summary>
    public decimal AnnualPAYE { get; init; }
    
    /// <summary>
    /// Annual taxable income after deductions
    /// </summary>
    public decimal TaxableIncome { get; init; }
    
    /// <summary>
    /// Tax threshold for the age group
    /// </summary>
    public decimal TaxThreshold { get; init; }
    
    /// <summary>
    /// Total rebates applied
    /// </summary>
    public decimal TotalRebates { get; init; }
}