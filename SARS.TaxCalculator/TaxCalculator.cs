using System;
using System.Collections.Generic;
using System.Linq;
using SARS.TaxCalculator.Calculators;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Inputs;
using SARS.TaxCalculator.Models.Results;
using SARS.TaxCalculator.Utilities;

namespace SARS.TaxCalculator;

/// <summary>
/// Fluent API entry point for tax calculations
/// </summary>
public static class TaxCalculator
{
    /// <summary>
    /// Creates a tax calculation builder for the specified tax year
    /// </summary>
    /// <param name="year">Tax year (2023, 2024, 2025, or 2026)</param>
    /// <returns>Tax calculation builder</returns>
    public static TaxCalculationBuilder ForTaxYear(int year)
    {
        var config = TaxYearData.GetConfiguration(year);
        return new TaxCalculationBuilder(config);
    }

    /// <summary>
    /// Gets all supported tax years
    /// </summary>
    public static IEnumerable<int> SupportedYears => TaxYearData.SupportedYears;
}

/// <summary>
/// Fluent builder for tax calculations
/// </summary>
public class TaxCalculationBuilder
{
    private readonly TaxYearConfiguration _config;
    private decimal _grossSalary;
    private bool _isAnnual;
    private int _age = 30; // Default age
    private int _medicalAidMembers;
    private decimal _medicalAidContribution;
    private decimal _retirementContributionPercentage;
    private decimal _retirementContributionAmount;
    private decimal _annualPayroll = 1000000; // Default above SDL threshold
    private EtiEmployee? _etiEmployee;

    internal TaxCalculationBuilder(TaxYearConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Sets the gross salary (monthly by default)
    /// </summary>
    /// <param name="amount">Salary amount</param>
    /// <returns>Builder instance</returns>
    public TaxCalculationBuilder WithGrossSalary(decimal amount)
    {
        _grossSalary = amount;
        _isAnnual = false;
        return this;
    }

    /// <summary>
    /// Sets the gross annual salary
    /// </summary>
    /// <param name="amount">Annual salary amount</param>
    /// <returns>Builder instance</returns>
    public TaxCalculationBuilder WithAnnualGrossSalary(decimal amount)
    {
        _grossSalary = amount;
        _isAnnual = true;
        return this;
    }

    /// <summary>
    /// Sets the employee age
    /// </summary>
    /// <param name="age">Employee age</param>
    /// <returns>Builder instance</returns>
    public TaxCalculationBuilder WithAge(int age)
    {
        if (age < 0 || age > 150)
            throw new ArgumentException("Age must be between 0 and 150", nameof(age));

        _age = age;
        return this;
    }

    /// <summary>
    /// Sets medical aid details
    /// </summary>
    /// <param name="numberOfMembers">Total number of members on medical aid (including main member)</param>
    /// <param name="monthlyContribution">Monthly medical aid contribution (optional)</param>
    /// <returns>Builder instance</returns>
    public TaxCalculationBuilder WithMedicalAid(int numberOfMembers, decimal monthlyContribution = 0)
    {
        if (numberOfMembers < 1)
            throw new ArgumentException("Number of medical aid members must be at least 1", nameof(numberOfMembers));

        _medicalAidMembers = numberOfMembers;
        _medicalAidContribution = monthlyContribution;
        return this;
    }

    /// <summary>
    /// Sets retirement contribution as a percentage of gross salary
    /// </summary>
    /// <param name="percentage">Contribution percentage (e.g., 0.075 for 7.5%)</param>
    /// <returns>Builder instance</returns>
    public TaxCalculationBuilder WithRetirementContribution(decimal percentage)
    {
        if (percentage < 0 || percentage > 1)
            throw new ArgumentException("Retirement contribution percentage must be between 0 and 1", nameof(percentage));

        _retirementContributionPercentage = percentage;
        _retirementContributionAmount = 0;
        return this;
    }

    /// <summary>
    /// Sets retirement contribution as a fixed amount
    /// </summary>
    /// <param name="monthlyAmount">Monthly contribution amount</param>
    /// <returns>Builder instance</returns>
    public TaxCalculationBuilder WithRetirementContributionAmount(decimal monthlyAmount)
    {
        if (monthlyAmount < 0)
            throw new ArgumentException("Retirement contribution cannot be negative", nameof(monthlyAmount));

        _retirementContributionAmount = monthlyAmount;
        _retirementContributionPercentage = 0;
        return this;
    }

    /// <summary>
    /// Sets the company's annual payroll for SDL calculation
    /// </summary>
    /// <param name="annualPayroll">Total annual payroll</param>
    /// <returns>Builder instance</returns>
    public TaxCalculationBuilder WithCompanyPayroll(decimal annualPayroll)
    {
        if (annualPayroll < 0)
            throw new ArgumentException("Annual payroll cannot be negative", nameof(annualPayroll));

        _annualPayroll = annualPayroll;
        return this;
    }

    /// <summary>
    /// Sets ETI employee details
    /// </summary>
    /// <param name="employmentMonths">Number of months employed</param>
    /// <param name="isFirstTime">Is first-time employee</param>
    /// <param name="inSez">Works in Special Economic Zone</param>
    /// <returns>Builder instance</returns>
    public TaxCalculationBuilder WithEtiDetails(int employmentMonths, bool isFirstTime = true, bool inSez = false)
    {
        _etiEmployee = new EtiEmployee
        {
            Age = _age,
            MonthlySalary = _isAnnual ? _grossSalary / 12 : _grossSalary,
            EmploymentMonths = employmentMonths,
            IsFirstTimeEmployee = isFirstTime,
            WorksInSpecialEconomicZone = inSez
        };
        return this;
    }

    /// <summary>
    /// Calculates tax based on provided parameters
    /// </summary>
    /// <returns>Complete tax calculation result</returns>
    public TaxCalculationResult Calculate()
    {
        // Normalize to monthly values
        var monthlyGross = _isAnnual ? _grossSalary / 12 : _grossSalary;
        var annualGross = _isAnnual ? _grossSalary : _grossSalary * 12;

        // Calculate retirement contribution
        var monthlyRetirement = _retirementContributionPercentage > 0
            ? monthlyGross * _retirementContributionPercentage
            : _retirementContributionAmount;
        var annualRetirement = monthlyRetirement * 12;

        // Initialize calculators
        var payeCalculator = new PayeCalculator(_config);
        var uifCalculator = new UifCalculator(_config.UifConfig);
        var sdlCalculator = new SdlCalculator(_config.SdlConfig);
        var etiCalculator = new EtiCalculator(_config.EtiConfig);

        // Calculate PAYE
        var annualPaye = payeCalculator.CalculatePayeWithRetirement(
            annualGross, annualRetirement, _age, _medicalAidMembers);
        var monthlyPaye = SarsRounding.RoundPaye(annualPaye / 12);

        // Calculate UIF
        var uif = uifCalculator.CalculateMonthly(monthlyGross);

        // Calculate SDL
        var sdl = sdlCalculator.CalculateMonthly(monthlyGross, _annualPayroll);

        // Calculate ETI if applicable
        decimal etiAmount = 0;
        if (_etiEmployee != null)
        {
            var etiResult = etiCalculator.CalculateMonthly(_etiEmployee);
            etiAmount = etiResult.Amount;
        }

        // Calculate net salary
        var totalDeductions = monthlyPaye + uif.EmployeeAmount + monthlyRetirement + _medicalAidContribution;
        var netSalary = monthlyGross - totalDeductions;

        // Calculate employer costs
        var employerCosts = uif.EmployerAmount + sdl.Amount;
        var costToCompany = monthlyGross + employerCosts;

        return new TaxCalculationResult
        {
            GrossSalary = monthlyGross,
            PAYE = monthlyPaye,
            UIF = uif.EmployeeAmount,
            SDL = sdl.Amount,
            ETI = etiAmount,
            RetirementContribution = monthlyRetirement,
            MedicalAidContribution = _medicalAidContribution,
            MedicalAidTaxCredit = _medicalAidMembers > 0
                ? _config.MedicalAidCredit.CalculateMonthlyCredit(_medicalAidMembers - 1)
                : 0,
            NetSalary = netSalary,
            EmployerUIF = uif.EmployerAmount,
            EmployerSDL = sdl.Amount,
            CostToCompany = costToCompany,
            TaxYear = _config.Year,
            Age = _age,
            AnnualGrossSalary = annualGross,
            AnnualPAYE = annualPaye,
            UifCeilingApplied = uif.CeilingApplied,
            SdlExempt = sdl.IsExempt
        };
    }

    /// <summary>
    /// Calculates only PAYE
    /// </summary>
    /// <returns>PAYE calculation result</returns>
    public PayeOnlyResult CalculatePaye()
    {
        var monthlyGross = _isAnnual ? _grossSalary / 12 : _grossSalary;
        var annualGross = _isAnnual ? _grossSalary : _grossSalary * 12;

        var monthlyRetirement = _retirementContributionPercentage > 0
            ? monthlyGross * _retirementContributionPercentage
            : _retirementContributionAmount;
        var annualRetirement = monthlyRetirement * 12;

        var payeCalculator = new PayeCalculator(_config);
        var annualPaye = payeCalculator.CalculatePayeWithRetirement(
            annualGross, annualRetirement, _age, _medicalAidMembers);

        return new PayeOnlyResult
        {
            MonthlyPAYE = SarsRounding.RoundPaye(annualPaye / 12),
            AnnualPAYE = annualPaye,
            TaxableIncome = annualGross - annualRetirement,
            TaxThreshold = _config.TaxThresholds.FirstOrDefault(t => t.AppliesTo(_age))?.Amount ?? 0,
            TotalRebates = _config.TaxRebates.Where(r => r.QualifiesForRebate(_age)).Sum(r => r.Amount)
        };
    }
}
