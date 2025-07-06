using System;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Results;

namespace SARS.TaxCalculator.Calculators;

/// <summary>
/// Calculates UIF (Unemployment Insurance Fund) contributions
/// </summary>
public class UifCalculator
{
    private readonly UifConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the UIF calculator
    /// </summary>
    /// <param name="config">UIF configuration</param>
    public UifCalculator(UifConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Calculates monthly UIF contributions
    /// </summary>
    /// <param name="monthlyIncome">Monthly income</param>
    /// <returns>UIF contribution details</returns>
    public UifContribution CalculateMonthly(decimal monthlyIncome)
    {
        if (monthlyIncome < 0)
            throw new ArgumentException("Monthly income cannot be negative", nameof(monthlyIncome));

        // Apply ceiling
        var contributionBase = Math.Min(monthlyIncome, _config.MonthlyCeiling);

        var employeeContribution = Math.Round(contributionBase * _config.EmployeeRate, 2, MidpointRounding.AwayFromZero);
        var employerContribution = Math.Round(contributionBase * _config.EmployerRate, 2, MidpointRounding.AwayFromZero);

        return new UifContribution
        {
            EmployeeAmount = employeeContribution,
            EmployerAmount = employerContribution,
            TotalAmount = employeeContribution + employerContribution,
            ContributionBase = contributionBase,
            CeilingApplied = monthlyIncome > _config.MonthlyCeiling
        };
    }

    /// <summary>
    /// Calculates annual UIF contributions
    /// </summary>
    /// <param name="annualIncome">Annual income</param>
    /// <returns>UIF contribution details</returns>
    public UifContribution CalculateAnnual(decimal annualIncome)
    {
        if (annualIncome < 0)
            throw new ArgumentException("Annual income cannot be negative", nameof(annualIncome));

        // Apply annual ceiling
        var contributionBase = Math.Min(annualIncome, _config.AnnualCeiling);

        var employeeContribution = Math.Round(contributionBase * _config.EmployeeRate, 2, MidpointRounding.AwayFromZero);
        var employerContribution = Math.Round(contributionBase * _config.EmployerRate, 2, MidpointRounding.AwayFromZero);

        return new UifContribution
        {
            EmployeeAmount = employeeContribution,
            EmployerAmount = employerContribution,
            TotalAmount = employeeContribution + employerContribution,
            ContributionBase = contributionBase,
            CeilingApplied = annualIncome > _config.AnnualCeiling
        };
    }

    /// <summary>
    /// Checks if income exceeds UIF ceiling
    /// </summary>
    /// <param name="monthlyIncome">Monthly income to check</param>
    /// <returns>True if income exceeds the ceiling</returns>
    public bool ExceedsCeiling(decimal monthlyIncome)
    {
        return monthlyIncome > _config.MonthlyCeiling;
    }

    /// <summary>
    /// Gets the maximum monthly UIF contribution
    /// </summary>
    /// <returns>Maximum contribution amounts</returns>
    public UifContribution GetMaximumMonthlyContribution()
    {
        return CalculateMonthly(_config.MonthlyCeiling);
    }
}
