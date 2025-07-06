using System;
using System.Collections.Generic;
using System.Linq;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Inputs;
using SARS.TaxCalculator.Models.Results;
using SARS.TaxCalculator.Utilities;

namespace SARS.TaxCalculator.Calculators;

/// <summary>
/// Calculates complete payslip details including all deductions and contributions
/// </summary>
public class PayslipCalculator
{
    private readonly TaxYearConfiguration _config;
    private readonly PayeCalculator _payeCalculator;
    private readonly UifCalculator _uifCalculator;
    private readonly SdlCalculator _sdlCalculator;
    private readonly EtiCalculator _etiCalculator;

    /// <summary>
    /// Initializes a new instance of the payslip calculator
    /// </summary>
    /// <param name="config">Tax year configuration</param>
    public PayslipCalculator(TaxYearConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _payeCalculator = new PayeCalculator(config);
        _uifCalculator = new UifCalculator(config.UifConfig);
        _sdlCalculator = new SdlCalculator(config.SdlConfig);
        _etiCalculator = new EtiCalculator(config.EtiConfig);
    }

    /// <summary>
    /// Calculates a complete payslip
    /// </summary>
    /// <param name="input">Payslip calculation input</param>
    /// <returns>Complete payslip details</returns>
    public Payslip Calculate(PayslipInput input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        ValidateInput(input);

        // Calculate gross amounts
        var monthlyGross = input.IsAnnualSalary ? input.GrossSalary / 12 : input.GrossSalary;
        var annualGross = input.IsAnnualSalary ? input.GrossSalary : input.GrossSalary * 12;

        // Calculate deductions
        var deductions = CalculateDeductions(input, monthlyGross, annualGross);
        
        // Calculate employer contributions
        var employerContributions = CalculateEmployerContributions(input, monthlyGross);
        
        // Calculate ETI if applicable
        var eti = CalculateEti(input, monthlyGross);
        
        // Build payslip
        return new Payslip
        {
            Employee = new EmployeeInfo
            {
                EmployeeId = input.EmployeeId,
                Name = input.EmployeeName,
                Age = input.Age,
                TaxNumber = input.TaxNumber
            },
            Period = new PayPeriod
            {
                Month = input.PayMonth,
                Year = input.PayYear,
                TaxYear = _config.Year
            },
            Earnings = new Earnings
            {
                BasicSalary = monthlyGross,
                GrossEarnings = monthlyGross
            },
            Deductions = deductions,
            EmployerContributions = employerContributions,
            ETI = eti,
            Summary = CalculateSummary(monthlyGross, deductions, employerContributions, eti)
        };
    }

    /// <summary>
    /// Calculates payslips for multiple employees
    /// </summary>
    /// <param name="inputs">List of payslip inputs</param>
    /// <returns>Bulk payslip result</returns>
    public BulkPayslipResult CalculateBulk(IEnumerable<PayslipInput> inputs)
    {
        var inputList = inputs?.ToList() ?? throw new ArgumentNullException(nameof(inputs));
        var payslips = inputList.Select(Calculate).ToList();

        return new BulkPayslipResult
        {
            Payslips = payslips,
            Summary = new BulkPayslipSummary
            {
                TotalEmployees = payslips.Count,
                TotalGrossEarnings = payslips.Sum(p => p.Earnings.GrossEarnings),
                TotalPAYE = payslips.Sum(p => p.Deductions.PAYE),
                TotalUIF = payslips.Sum(p => p.Deductions.UIF + p.EmployerContributions.UIF),
                TotalSDL = payslips.Sum(p => p.EmployerContributions.SDL),
                TotalETI = payslips.Sum(p => p.ETI?.Amount ?? 0),
                TotalNetPay = payslips.Sum(p => p.Summary.NetPay),
                TotalCostToCompany = payslips.Sum(p => p.Summary.CostToCompany)
            }
        };
    }

    private void ValidateInput(PayslipInput input)
    {
        if (input.GrossSalary <= 0)
            throw new ArgumentException("Gross salary must be positive", nameof(input));
        if (input.Age < 0 || input.Age > 150)
            throw new ArgumentException("Age must be between 0 and 150", nameof(input));
        if (input.MedicalAidMembers < 0)
            throw new ArgumentException("Medical aid members cannot be negative", nameof(input));
        if (input.RetirementContribution < 0)
            throw new ArgumentException("Retirement contribution cannot be negative", nameof(input));
        if (input.CompanyAnnualPayroll < 0)
            throw new ArgumentException("Company annual payroll cannot be negative", nameof(input));
    }

    private EmployeeDeductions CalculateDeductions(PayslipInput input, decimal monthlyGross, decimal annualGross)
    {
        // Calculate retirement deduction
        var monthlyRetirement = input.RetirementContribution;
        var annualRetirement = monthlyRetirement * 12;

        // Calculate PAYE
        var annualPaye = _payeCalculator.CalculatePayeWithRetirement(
            annualGross, annualRetirement, input.Age, input.MedicalAidMembers);
        var monthlyPaye = SarsRounding.RoundPaye(annualPaye / 12);

        // Calculate UIF
        var uif = _uifCalculator.CalculateMonthly(monthlyGross);

        // Calculate total deductions
        var totalDeductions = monthlyPaye + uif.EmployeeAmount + monthlyRetirement + 
                             input.MedicalAidContribution + input.OtherDeductions;

        return new EmployeeDeductions
        {
            PAYE = monthlyPaye,
            UIF = uif.EmployeeAmount,
            RetirementContribution = monthlyRetirement,
            MedicalAidContribution = input.MedicalAidContribution,
            OtherDeductions = input.OtherDeductions,
            TotalDeductions = totalDeductions,
            MedicalAidTaxCredit = input.MedicalAidMembers > 0 
                ? _config.MedicalAidCredit.CalculateMonthlyCredit(input.MedicalAidMembers - 1) 
                : 0
        };
    }

    private EmployerContributions CalculateEmployerContributions(PayslipInput input, decimal monthlyGross)
    {
        var uif = _uifCalculator.CalculateMonthly(monthlyGross);
        var sdl = _sdlCalculator.CalculateMonthly(monthlyGross, input.CompanyAnnualPayroll);

        return new EmployerContributions
        {
            UIF = uif.EmployerAmount,
            SDL = sdl.Amount,
            RetirementContribution = input.EmployerRetirementContribution,
            MedicalAidContribution = input.EmployerMedicalAidContribution,
            TotalContributions = uif.EmployerAmount + sdl.Amount + 
                               input.EmployerRetirementContribution + 
                               input.EmployerMedicalAidContribution
        };
    }

    private EtiInfo? CalculateEti(PayslipInput input, decimal monthlyGross)
    {
        if (!input.IsEtiEligible)
            return null;

        var etiEmployee = new EtiEmployee
        {
            EmployeeId = input.EmployeeId,
            Age = input.Age,
            MonthlySalary = monthlyGross,
            EmploymentMonths = input.EmploymentMonths,
            IsFirstTimeEmployee = input.IsFirstTimeEmployee,
            WorksInSpecialEconomicZone = input.WorksInSpecialEconomicZone
        };

        var result = _etiCalculator.CalculateMonthly(etiEmployee);

        return new EtiInfo
        {
            Amount = result.Amount,
            IsEligible = result.IsEligible,
            IneligibilityReason = result.IneligibilityReason
        };
    }

    private PayslipSummary CalculateSummary(decimal monthlyGross, EmployeeDeductions deductions, 
        EmployerContributions employerContributions, EtiInfo? eti)
    {
        var netPay = monthlyGross - deductions.TotalDeductions;
        var costToCompany = monthlyGross + employerContributions.TotalContributions;
        var netPayePayable = deductions.PAYE - (eti?.Amount ?? 0);

        return new PayslipSummary
        {
            GrossPay = monthlyGross,
            TotalDeductions = deductions.TotalDeductions,
            NetPay = netPay,
            CostToCompany = costToCompany,
            NetPAYEPayable = Math.Max(0, netPayePayable)
        };
    }
}