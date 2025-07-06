using System;
using System.Collections.Generic;
using System.Linq;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Inputs;
using SARS.TaxCalculator.Models.Results;
using SARS.TaxCalculator.Utilities;

namespace SARS.TaxCalculator.Calculators;

/// <summary>
/// Calculates ETI (Employment Tax Incentive) according to Employment Tax Incentive Act
/// Source: Employment Tax Incentive Act, 2013 (Act No. 26 of 2013)
/// Reference: https://www.sars.gov.za/types-of-tax/pay-as-you-earn/employment-tax-incentive-eti/
/// Guide: SARS Guide to the Employment Tax Incentive (LAPD-ETI-G01)
/// Changes effective 1 April 2025: https://www.sars.gov.za/latest-news/employment-tax-incentive-eti-changes-with-effect-from-1-april-2025/
/// </summary>
public class EtiCalculator
{
    private readonly EtiConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the ETI calculator
    /// </summary>
    /// <param name="config">ETI configuration</param>
    public EtiCalculator(EtiConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Calculates monthly ETI amount
    /// </summary>
    /// <param name="employee">Employee details</param>
    /// <returns>ETI calculation result</returns>
    public EtiCalculationResult CalculateMonthly(EtiEmployee employee)
    {
        if (employee == null)
            throw new ArgumentNullException(nameof(employee));

        var eligibility = CheckEligibility(employee);
        if (!eligibility.IsEligible)
        {
            return new EtiCalculationResult
            {
                Amount = 0,
                IsEligible = false,
                IneligibilityReason = eligibility.Reason,
                Employee = employee
            };
        }

        var band = GetApplicableBand(employee.MonthlySalary);
        if (band == null)
        {
            return new EtiCalculationResult
            {
                Amount = 0,
                IsEligible = false,
                IneligibilityReason = "Salary exceeds maximum qualifying amount",
                Employee = employee
            };
        }

        var amount = CalculateIncentiveAmount(employee, band);

        return new EtiCalculationResult
        {
            Amount = amount,
            IsEligible = true,
            IneligibilityReason = null,
            Employee = employee,
            Band = band
        };
    }

    /// <summary>
    /// Calculates ETI for multiple employees
    /// </summary>
    /// <param name="employees">List of employees</param>
    /// <returns>Bulk ETI calculation results</returns>
    public EtiBulkResult CalculateBulk(IEnumerable<EtiEmployee> employees)
    {
        var employeeList = employees.ToList() ?? throw new ArgumentNullException(nameof(employees));
        var results = employeeList.Select(CalculateMonthly).ToList();

        return new EtiBulkResult
        {
            TotalEmployees = employeeList.Count,
            EligibleEmployees = results.Count(r => r.IsEligible),
            TotalEtiAmount = results.Sum(r => r.Amount),
            IndividualResults = results
        };
    }

    private EtiEligibility CheckEligibility(EtiEmployee employee)
    {
        // Special Economic Zone (SEZ) employees are exempt from age restrictions
        // Source: Employment Tax Incentive Act - SEZ provisions
        // Reference: SARS ETI validation rules and SEZ special provisions
        if (!employee.WorksInSpecialEconomicZone && (employee.Age < _config.MinAge || employee.Age > _config.MaxAge))
        {
            return new EtiEligibility
            {
                IsEligible = false,
                Reason = $"Age {employee.Age} is outside eligible range ({_config.MinAge}-{_config.MaxAge})"
            };
        }

        if (employee.MonthlySalary > _config.MaxQualifyingSalary)
        {
            return new EtiEligibility
            {
                IsEligible = false,
                Reason = $"Salary R{employee.MonthlySalary:N2} exceeds maximum qualifying salary of R{_config.MaxQualifyingSalary:N2}"
            };
        }

        if (employee is { IsFirstTimeEmployee: false, EmploymentMonths: >= 24 })
        {
            return new EtiEligibility
            {
                IsEligible = false,
                Reason = "ETI period has expired (24 months)"
            };
        }

        return new EtiEligibility { IsEligible = true };
    }

    private EtiBand? GetApplicableBand(decimal monthlySalary)
    {
        return _config.Bands.FirstOrDefault(b => monthlySalary >= b.MinSalary && monthlySalary <= b.MaxSalary);
    }

    private decimal CalculateIncentiveAmount(EtiEmployee employee, EtiBand band)
    {
        // Determine if first or second year
        var isFirstYear = employee.EmploymentMonths < 12;
        var baseAmount = isFirstYear ? band.FirstYearAmount : band.SecondYearAmount;

        // Apply reduction for salaries above minimum in band
        if (band.ReductionRate.HasValue && employee.MonthlySalary > band.MinSalary)
        {
            var excessAmount = employee.MonthlySalary - band.MinSalary;
            var reduction = excessAmount * band.ReductionRate.Value;
            baseAmount = Math.Max(0, baseAmount - reduction);
        }

        return SarsRounding.RoundEti(baseAmount);
    }
}
