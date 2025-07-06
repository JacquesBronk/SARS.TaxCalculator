using System;
using System.Collections.Generic;
using System.Linq;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Results;

namespace SARS.TaxCalculator.Calculators;

/// <summary>
/// Calculates SDL (Skills Development Levy)
/// </summary>
public class SdlCalculator
{
    private readonly SdlConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the SDL calculator
    /// </summary>
    /// <param name="config">SDL configuration</param>
    public SdlCalculator(SdlConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Calculates monthly SDL for a single employee
    /// </summary>
    /// <param name="monthlyIncome">Employee's monthly income</param>
    /// <param name="annualPayroll">Company's total annual payroll</param>
    /// <returns>SDL amount</returns>
    public SdlContribution CalculateMonthly(decimal monthlyIncome, decimal annualPayroll)
    {
        if (monthlyIncome < 0)
            throw new ArgumentException("Monthly income cannot be negative", nameof(monthlyIncome));
        if (annualPayroll < 0)
            throw new ArgumentException("Annual payroll cannot be negative", nameof(annualPayroll));

        var isExempt = annualPayroll <= _config.ExemptionThreshold;
        var amount = isExempt ? 0 : Math.Round(monthlyIncome * _config.Rate, 2, MidpointRounding.AwayFromZero);

        return new SdlContribution
        {
            Amount = amount,
            IsExempt = isExempt,
            Rate = _config.Rate,
            AnnualPayroll = annualPayroll,
            ExemptionThreshold = _config.ExemptionThreshold
        };
    }

    /// <summary>
    /// Calculates annual SDL for a single employee
    /// </summary>
    /// <param name="annualIncome">Employee's annual income</param>
    /// <param name="annualPayroll">Company's total annual payroll</param>
    /// <returns>SDL amount</returns>
    public SdlContribution CalculateAnnual(decimal annualIncome, decimal annualPayroll)
    {
        if (annualIncome < 0)
            throw new ArgumentException("Annual income cannot be negative", nameof(annualIncome));
        if (annualPayroll < 0)
            throw new ArgumentException("Annual payroll cannot be negative", nameof(annualPayroll));

        var isExempt = annualPayroll <= _config.ExemptionThreshold;
        var amount = isExempt ? 0 : Math.Round(annualIncome * _config.Rate, 2, MidpointRounding.AwayFromZero);

        return new SdlContribution
        {
            Amount = amount,
            IsExempt = isExempt,
            Rate = _config.Rate,
            AnnualPayroll = annualPayroll,
            ExemptionThreshold = _config.ExemptionThreshold
        };
    }

    /// <summary>
    /// Calculates total SDL for the entire company payroll
    /// </summary>
    /// <param name="annualPayroll">Total annual payroll</param>
    /// <returns>Total SDL amount</returns>
    public decimal CalculateTotalSdl(decimal annualPayroll)
    {
        if (annualPayroll < 0)
            throw new ArgumentException("Annual payroll cannot be negative", nameof(annualPayroll));

        if (annualPayroll <= _config.ExemptionThreshold)
            return 0;

        return Math.Round(annualPayroll * _config.Rate, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Checks if a company is exempt from SDL
    /// </summary>
    /// <param name="annualPayroll">Company's total annual payroll</param>
    /// <returns>True if exempt, false otherwise</returns>
    public bool IsExempt(decimal annualPayroll)
    {
        return annualPayroll <= _config.ExemptionThreshold;
    }

    /// <summary>
    /// Calculates SDL for multiple employees
    /// </summary>
    /// <param name="employeeSalaries">List of employee annual salaries</param>
    /// <returns>SDL contribution details</returns>
    public SdlBulkContribution CalculateBulk(IEnumerable<decimal> employeeSalaries)
    {
        var salaryList = employeeSalaries.ToList() ?? throw new ArgumentNullException(nameof(employeeSalaries));

        if (salaryList.Any(s => s < 0))
            throw new ArgumentException("Salaries cannot be negative", nameof(employeeSalaries));

        var totalPayroll = salaryList.Sum();
        var isExempt = totalPayroll <= _config.ExemptionThreshold;

        var contributions = salaryList.Select(salary => new
        {
            Salary = salary,
            Sdl = isExempt ? 0 : Math.Round(salary * _config.Rate, 2, MidpointRounding.AwayFromZero)
        }).ToList();

        return new SdlBulkContribution
        {
            TotalPayroll = totalPayroll,
            TotalSdl = contributions.Sum(c => c.Sdl),
            IsExempt = isExempt,
            EmployeeCount = salaryList.Count,
            IndividualContributions = contributions.Select(c => new IndividualSdlContribution
            {
                AnnualSalary = c.Salary,
                SdlAmount = c.Sdl
            }).ToList()
        };
    }
}
