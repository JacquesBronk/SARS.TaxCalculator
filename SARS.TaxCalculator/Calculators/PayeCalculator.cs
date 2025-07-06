using System;
using System.Linq;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Utilities;

namespace SARS.TaxCalculator.Calculators;

/// <summary>
/// Calculates PAYE (Pay As You Earn) tax according to Fourth Schedule of Income Tax Act
/// Source: Fourth Schedule to the Income Tax Act, 1962
/// Reference: SARS Guide for Employers in Respect of Employees' Tax
/// Tax rates and thresholds: https://www.sars.gov.za/tax-rates/income-tax/rates-of-tax-for-individuals/
/// </summary>
public class PayeCalculator
{
    private readonly TaxYearConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the PAYE calculator
    /// </summary>
    /// <param name="config">Tax year configuration</param>
    public PayeCalculator(TaxYearConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Calculates annual PAYE tax
    /// </summary>
    /// <param name="annualTaxableIncome">Annual taxable income</param>
    /// <param name="age">Employee age</param>
    /// <param name="medicalAidMembers">Number of medical aid members (0 if no medical aid)</param>
    /// <returns>Annual PAYE amount</returns>
    public decimal CalculateAnnualPaye(decimal annualTaxableIncome, int age, int medicalAidMembers = 0)
    {
        if (annualTaxableIncome < 0)
            throw new ArgumentException("Annual taxable income cannot be negative", nameof(annualTaxableIncome));
        if (age < 0 || age > 150)
            throw new ArgumentException("Age must be between 0 and 150", nameof(age));
        if (medicalAidMembers < 0)
            throw new ArgumentException("Medical aid members cannot be negative", nameof(medicalAidMembers));

        // Calculate gross tax using tax brackets
        var grossTax = CalculateGrossTax(annualTaxableIncome);

        // Apply rebates based on age
        var totalRebates = CalculateTotalRebates(age);

        // Apply medical aid credits
        var medicalCredits = medicalAidMembers > 0
            ? _config.MedicalAidCredit.CalculateAnnualCredit(medicalAidMembers - 1)
            : 0;

        // Calculate net PAYE
        var netPaye = grossTax - totalRebates - medicalCredits;

        // Check if income is below a tax threshold
        var threshold = GetTaxThreshold(age);
        if (annualTaxableIncome <= threshold)
            return 0;

        return SarsRounding.RoundPaye(Math.Max(0, netPaye));
    }

    /// <summary>
    /// Calculates monthly PAYE tax
    /// </summary>
    /// <param name="monthlyTaxableIncome">Monthly taxable income</param>
    /// <param name="age">Employee age</param>
    /// <param name="medicalAidMembers">Number of medical aid members (0 if no medical aid)</param>
    /// <returns>Monthly PAYE amount</returns>
    public decimal CalculateMonthlyPaye(decimal monthlyTaxableIncome, int age, int medicalAidMembers = 0)
    {
        var annualPaye = CalculateAnnualPaye(monthlyTaxableIncome * 12, age, medicalAidMembers);
        return SarsRounding.RoundPaye(annualPaye / 12);
    }

    /// <summary>
    /// Calculates PAYE with retirement contributions
    /// </summary>
    /// <param name="annualGrossIncome">Annual gross income before deductions</param>
    /// <param name="retirementContribution">Annual retirement contribution amount</param>
    /// <param name="age">Employee age</param>
    /// <param name="medicalAidMembers">Number of medical aid members</param>
    /// <returns>Annual PAYE amount after retirement deductions</returns>
    public decimal CalculatePayeWithRetirement(decimal annualGrossIncome, decimal retirementContribution,
        int age, int medicalAidMembers = 0)
    {
        // Calculate allowable retirement deduction
        var maxDeduction = Math.Min(
            annualGrossIncome * _config.RetirementLimits.MaxPercentage,
            _config.RetirementLimits.AnnualCap
        );
        var actualDeduction = Math.Min(retirementContribution, maxDeduction);

        // Calculate taxable income after retirement deduction
        var taxableIncome = annualGrossIncome - actualDeduction;

        return CalculateAnnualPaye(taxableIncome, age, medicalAidMembers);
    }

    private decimal CalculateGrossTax(decimal annualTaxableIncome)
    {
        decimal totalTax = 0;

        foreach (var bracket in _config.TaxBrackets)
        {
            if (annualTaxableIncome >= bracket.MinIncome)
            {
                totalTax = bracket.CalculateTax(annualTaxableIncome);

                // If income doesn't exceed this bracket's maximum, we're done
                if (!bracket.MaxIncome.HasValue || annualTaxableIncome <= bracket.MaxIncome.Value)
                    break;
            }
        }

        return totalTax;
    }

    private decimal CalculateTotalRebates(int age)
    {
        return _config.TaxRebates
            .Where(r => r.QualifiesForRebate(age))
            .Sum(r => r.Amount);
    }

    private decimal GetTaxThreshold(int age)
    {
        var threshold = _config.TaxThresholds.FirstOrDefault(t => t.AppliesTo(age));
        return threshold?.Amount ?? 0;
    }

    /// <summary>
    /// Determines if a person needs to pay tax based on income and age
    /// </summary>
    /// <param name="annualIncome">Annual income</param>
    /// <param name="age">Person's age</param>
    /// <returns>True if tax is payable</returns>
    public bool IsTaxPayable(decimal annualIncome, int age)
    {
        var threshold = GetTaxThreshold(age);
        return annualIncome > threshold;
    }
}
