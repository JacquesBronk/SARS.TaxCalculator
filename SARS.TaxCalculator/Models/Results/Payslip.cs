using System.Collections.Generic;

namespace SARS.TaxCalculator.Models.Results;

/// <summary>
/// Represents a comprehensive payslip containing employee details, earnings, deductions, and tax calculations.
/// </summary>
/// <remarks>
/// This is the primary result object returned by the SARS Tax Calculator, containing all payslip components
/// including PAYE, UIF, SDL, and ETI calculations where applicable.
/// </remarks>
/// <example>
/// <code>
/// var payslip = taxCalculator.Calculate(payslipInput);
/// Console.WriteLine($"Net Pay: {payslip.Summary.NetPay:C}");
/// Console.WriteLine($"PAYE: {payslip.Deductions.PAYE:C}");
/// </code>
/// </example>
public class Payslip
{
    /// <summary>
    /// Gets or sets the employee information for this payslip.
    /// </summary>
    /// <value>The <see cref="EmployeeInfo"/> containing employee details.</value>
    public EmployeeInfo Employee { get; init; } = new();

    /// <summary>
    /// Gets or sets the pay period information for this payslip.
    /// </summary>
    /// <value>The <see cref="PayPeriod"/> containing month, year, and tax year details.</value>
    public PayPeriod Period { get; init; } = new();

    /// <summary>
    /// Gets or sets the earnings breakdown for this payslip.
    /// </summary>
    /// <value>The <see cref="Earnings"/> containing salary and gross earnings information.</value>
    public Earnings Earnings { get; init; } = new();

    /// <summary>
    /// Gets or sets the employee deductions for this payslip.
    /// </summary>
    /// <value>The <see cref="EmployeeDeductions"/> containing PAYE, UIF, and other deductions.</value>
    public EmployeeDeductions Deductions { get; init; } = new();

    /// <summary>
    /// Gets or sets the employer contributions for this payslip.
    /// </summary>
    /// <value>The <see cref="EmployerContributions"/> containing UIF, SDL, and other employer contributions.</value>
    public EmployerContributions EmployerContributions { get; init; } = new();

    /// <summary>
    /// Gets or sets the Employment Tax Incentive information for this payslip.
    /// </summary>
    /// <value>The <see cref="EtiInfo"/> containing ETI amount and eligibility details, or null if not applicable.</value>
    public EtiInfo? ETI { get; init; }

    /// <summary>
    /// Gets or sets the payslip summary containing totals and net pay calculations.
    /// </summary>
    /// <value>The <see cref="PayslipSummary"/> containing gross pay, total deductions, and net pay.</value>
    public PayslipSummary Summary { get; init; } = new();
}

/// <summary>
/// Represents employee information included in payslip calculations.
/// </summary>
/// <remarks>
/// Contains essential employee details required for tax calculations and payslip generation.
/// </remarks>
public class EmployeeInfo
{
    /// <summary>
    /// Gets or sets the unique identifier for the employee.
    /// </summary>
    /// <value>The employee identifier as provided in the payslip input.</value>
    /// <example>"EMP001", "12345"</example>
    public string EmployeeId { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name of the employee.
    /// </summary>
    /// <value>The employee's full legal name.</value>
    /// <example>"John Doe", "Jane Smith"</example>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee's age in years.
    /// </summary>
    /// <value>The age used for determining applicable tax rebates and ETI eligibility.</value>
    /// <example>25, 35, 65</example>
    public int Age { get; init; }

    /// <summary>
    /// Gets or sets the employee's South African tax number.
    /// </summary>
    /// <value>The 10-digit SARS tax reference number.</value>
    /// <example>"1234567890"</example>
    public string TaxNumber { get; init; } = string.Empty;
}

/// <summary>
/// Represents the pay period information for payslip calculations.
/// </summary>
/// <remarks>
/// Contains the specific month, year, and tax year for which the payslip is calculated.
/// The tax year determines which tax tables and rates are applied.
/// </remarks>
public class PayPeriod
{
    /// <summary>
    /// Gets or sets the month for which the payslip is calculated.
    /// </summary>
    /// <value>The month number (1-12) where 1 = January, 12 = December.</value>
    /// <example>1, 6, 12</example>
    public int Month { get; init; }

    /// <summary>
    /// Gets or sets the calendar year for which the payslip is calculated.
    /// </summary>
    /// <value>The calendar year (e.g., 2024, 2025, 2026).</value>
    /// <example>2024, 2025, 2026</example>
    public int Year { get; init; }

    /// <summary>
    /// Gets or sets the tax year configuration used for calculations.
    /// </summary>
    /// <value>The tax year that determines which tax tables and rates are applied.</value>
    /// <remarks>
    /// Tax year typically runs from 1 March to 28/29 February of the following year.
    /// Supported tax years: 2023, 2024, 2025, 2026.
    /// </remarks>
    /// <example>2024, 2025, 2026</example>
    public int TaxYear { get; init; }
}

/// <summary>
/// Represents the earnings breakdown for payslip calculations.
/// </summary>
/// <remarks>
/// Contains the basic salary and gross earnings before any deductions are applied.
/// These amounts form the basis for all tax and contribution calculations.
/// </remarks>
public class Earnings
{
    /// <summary>
    /// Gets or sets the employee's basic salary amount.
    /// </summary>
    /// <value>The basic salary in South African Rand (ZAR) before any additions or deductions.</value>
    /// <example>25000.00m, 45000.00m</example>
    public decimal BasicSalary { get; init; }

    /// <summary>
    /// Gets or sets the total gross earnings amount.
    /// </summary>
    /// <value>The total gross earnings in ZAR, which may include basic salary plus allowances.</value>
    /// <remarks>
    /// This amount is used as the base for PAYE, UIF, and SDL calculations.
    /// </remarks>
    /// <example>25000.00m, 50000.00m</example>
    public decimal GrossEarnings { get; init; }
}

/// <summary>
/// Represents all deductions applied to the employee's gross salary.
/// </summary>
/// <remarks>
/// Contains all statutory and voluntary deductions including PAYE, UIF, retirement contributions,
/// medical aid contributions, and other deductions. Also includes medical aid tax credits.
/// </remarks>
public class EmployeeDeductions
{
    /// <summary>
    /// Gets or sets the Pay-As-You-Earn (PAYE) tax amount deducted.
    /// </summary>
    /// <value>The monthly PAYE tax amount in ZAR, calculated according to SARS tax tables.</value>
    /// <remarks>
    /// PAYE is calculated using progressive tax brackets with age-based rebates and medical aid credits applied.
    /// </remarks>
    /// <example>2500.00m, 8750.00m</example>
    public decimal PAYE { get; init; }

    /// <summary>
    /// Gets or sets the Unemployment Insurance Fund (UIF) contribution deducted from the employee.
    /// </summary>
    /// <value>The monthly UIF contribution in ZAR (1% of gross salary, capped at R177.12).</value>
    /// <remarks>
    /// UIF is calculated at 1% of gross salary with a maximum monthly contribution of R177.12.
    /// </remarks>
    /// <example>177.12m, 250.00m</example>
    public decimal UIF { get; init; }

    /// <summary>
    /// Gets or sets the retirement fund contribution made by the employee.
    /// </summary>
    /// <value>The monthly retirement contribution in ZAR, up to the allowed limits.</value>
    /// <remarks>
    /// Retirement contributions are limited to 27.5% of taxable income with an annual cap of R350,000.
    /// </remarks>
    /// <example>1875.00m, 5000.00m</example>
    public decimal RetirementContribution { get; init; }

    /// <summary>
    /// Gets or sets the medical aid contribution paid by the employee.
    /// </summary>
    /// <value>The monthly medical aid premium contribution in ZAR.</value>
    /// <remarks>
    /// This amount is deducted from gross salary before PAYE calculation.
    /// </remarks>
    /// <example>1500.00m, 3500.00m</example>
    public decimal MedicalAidContribution { get; init; }

    /// <summary>
    /// Gets or sets the medical aid tax credit applied to reduce PAYE.
    /// </summary>
    /// <value>The monthly medical aid tax credit in ZAR based on the number of medical aid members.</value>
    /// <remarks>
    /// Medical aid credits: Main member R364, first dependent R364, additional dependents R246 each.
    /// This credit reduces the PAYE amount payable.
    /// </remarks>
    /// <example>364.00m, 728.00m, 1338.00m</example>
    public decimal MedicalAidTaxCredit { get; init; }

    /// <summary>
    /// Gets or sets other miscellaneous deductions from the employee's salary.
    /// </summary>
    /// <value>The total of all other deductions in ZAR (e.g., union fees, garnishments, loans).</value>
    /// <remarks>
    /// These deductions are applied after tax calculations and do not affect PAYE.
    /// </remarks>
    /// <example>500.00m, 1200.00m</example>
    public decimal OtherDeductions { get; init; }

    /// <summary>
    /// Gets or sets the total of all deductions applied to the employee's salary.
    /// </summary>
    /// <value>The sum of all employee deductions in ZAR.</value>
    /// <remarks>
    /// Includes PAYE, UIF, retirement contributions, medical aid contributions, and other deductions.
    /// Medical aid tax credits are subtracted from this total.
    /// </remarks>
    /// <example>7500.00m, 15000.00m</example>
    public decimal TotalDeductions { get; init; }
}

/// <summary>
/// Represents all contributions made by the employer on behalf of the employee.
/// </summary>
/// <remarks>
/// Contains statutory contributions (UIF, SDL) and voluntary contributions (retirement, medical aid)
/// that the employer makes. These do not affect the employee's net pay but contribute to cost-to-company.
/// </remarks>
public class EmployerContributions
{
    /// <summary>
    /// Gets or sets the Unemployment Insurance Fund (UIF) contribution paid by the employer.
    /// </summary>
    /// <value>The monthly employer UIF contribution in ZAR (1% of gross salary, capped at R177.12).</value>
    /// <remarks>
    /// Employer UIF matches the employee contribution at 1% of gross salary.
    /// </remarks>
    /// <example>177.12m, 250.00m</example>
    public decimal UIF { get; init; }

    /// <summary>
    /// Gets or sets the Skills Development Levy (SDL) paid by the employer.
    /// </summary>
    /// <value>The monthly SDL amount in ZAR (1% of gross salary for companies with annual payroll > R500,000).</value>
    /// <remarks>
    /// SDL is only payable by employers with annual payroll exceeding R500,000.
    /// Calculated at 1% of gross salary with no upper limit.
    /// </remarks>
    /// <example>0.00m (exempt company), 250.00m, 500.00m</example>
    public decimal SDL { get; init; }

    /// <summary>
    /// Gets or sets the retirement fund contribution made by the employer.
    /// </summary>
    /// <value>The monthly employer retirement contribution in ZAR.</value>
    /// <remarks>
    /// This is an optional contribution that does not affect employee tax calculations
    /// but increases the total cost-to-company.
    /// </remarks>
    /// <example>0.00m, 1875.00m, 3750.00m</example>
    public decimal RetirementContribution { get; init; }

    /// <summary>
    /// Gets or sets the medical aid contribution made by the employer.
    /// </summary>
    /// <value>The monthly employer medical aid contribution in ZAR.</value>
    /// <remarks>
    /// This is an optional contribution that does not affect employee tax calculations
    /// but increases the total cost-to-company.
    /// </remarks>
    /// <example>0.00m, 1000.00m, 2500.00m</example>
    public decimal MedicalAidContribution { get; init; }

    /// <summary>
    /// Gets or sets the total of all employer contributions.
    /// </summary>
    /// <value>The sum of all employer contributions in ZAR (UIF + SDL + retirement + medical aid).</value>
    /// <remarks>
    /// This total is used in cost-to-company calculations.
    /// </remarks>
    /// <example>427.12m, 2500.00m, 5000.00m</example>
    public decimal TotalContributions { get; init; }
}

/// <summary>
/// Represents Employment Tax Incentive (ETI) information and calculations.
/// </summary>
/// <remarks>
/// Contains ETI amount, eligibility status, and reasons for ineligibility if applicable.
/// ETI is a government incentive to encourage employment of young people aged 18-29
/// (or any age for Special Economic Zone employees).
/// </remarks>
public class EtiInfo
{
    /// <summary>
    /// Gets or sets the Employment Tax Incentive amount calculated for this employee.
    /// </summary>
    /// <value>The monthly ETI amount in ZAR, truncated to whole Rands as per SARS requirements.</value>
    /// <remarks>
    /// ETI amounts are calculated based on salary bands and employment duration.
    /// Year 1 rates: R0-R2,000: R1,500, R2,001-R4,500: R1,500 (50% reduction), R4,501-R6,500: R750 (25% reduction)
    /// Year 2 rates: Half of Year 1 rates.
    /// </remarks>
    /// <example>1500, 750, 375</example>
    public decimal Amount { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the employee is eligible for ETI.
    /// </summary>
    /// <value><c>true</c> if the employee qualifies for ETI; otherwise, <c>false</c>.</value>
    /// <remarks>
    /// Eligibility depends on age (18-29, or any age for SEZ), salary (≤ R7,500), and employment duration (≤ 24 months for experienced employees).
    /// </remarks>
    public bool IsEligible { get; init; }

    /// <summary>
    /// Gets or sets the reason why the employee is not eligible for ETI.
    /// </summary>
    /// <value>A descriptive string explaining ETI ineligibility, or null if eligible.</value>
    /// <remarks>
    /// Common reasons include: "Age outside range (18-29)", "Salary exceeds R7,500", "Employment period exceeds 24 months".
    /// </remarks>
    /// <example>"Age outside range (18-29)", "Salary exceeds R7,500"</example>
    public string? IneligibilityReason { get; init; }
}

/// <summary>
/// Represents the payslip summary containing key financial totals and calculations.
/// </summary>
/// <remarks>
/// Provides a consolidated view of gross pay, deductions, net pay, and cost-to-company calculations.
/// This is typically the most important section for both employees and employers.
/// </remarks>
public class PayslipSummary
{
    /// <summary>
    /// Gets or sets the gross pay amount before any deductions.
    /// </summary>
    /// <value>The total gross earnings in ZAR before any deductions are applied.</value>
    /// <example>25000.00m, 50000.00m</example>
    public decimal GrossPay { get; init; }

    /// <summary>
    /// Gets or sets the total of all deductions applied to the gross pay.
    /// </summary>
    /// <value>The sum of all employee deductions in ZAR (PAYE + UIF + contributions + other deductions - medical aid credits).</value>
    /// <example>7500.00m, 15000.00m</example>
    public decimal TotalDeductions { get; init; }

    /// <summary>
    /// Gets or sets the net pay amount after all deductions.
    /// </summary>
    /// <value>The amount paid to the employee in ZAR (Gross Pay - Total Deductions).</value>
    /// <remarks>
    /// This is the final amount the employee receives in their bank account.
    /// </remarks>
    /// <example>17500.00m, 35000.00m</example>
    public decimal NetPay { get; init; }

    /// <summary>
    /// Gets or sets the total cost to the company for employing this person.
    /// </summary>
    /// <value>The total employment cost in ZAR (Gross Pay + Employer Contributions).</value>
    /// <remarks>
    /// Includes gross salary plus employer UIF, SDL, retirement, and medical aid contributions.
    /// </remarks>
    /// <example>27500.00m, 55000.00m</example>
    public decimal CostToCompany { get; init; }

    /// <summary>
    /// Gets or sets the net PAYE amount payable to SARS after ETI offset.
    /// </summary>
    /// <value>The final PAYE amount in ZAR that must be paid to SARS (PAYE - ETI).</value>
    /// <remarks>
    /// This is the amount the employer must pay to SARS for this employee's income tax,
    /// reduced by any applicable Employment Tax Incentive.
    /// </remarks>
    /// <example>1000.00m, 7250.00m</example>
    public decimal NetPAYEPayable { get; init; }
}

/// <summary>
/// Represents the result of bulk payslip calculations for multiple employees.
/// </summary>
/// <remarks>
/// Contains individual payslips for each employee plus aggregated summary totals.
/// Used when processing payroll for multiple employees in a single operation.
/// </remarks>
/// <example>
/// <code>
/// var bulkResult = taxCalculator.CalculateBulk(employeeInputs);
/// Console.WriteLine($"Total employees: {bulkResult.Summary.TotalEmployees}");
/// Console.WriteLine($"Total net pay: {bulkResult.Summary.TotalNetPay:C}");
/// </code>
/// </example>
public class BulkPayslipResult
{
    /// <summary>
    /// Gets or sets the collection of individual payslips calculated.
    /// </summary>
    /// <value>A read-only list of <see cref="Payslip"/> objects, one for each employee processed.</value>
    /// <remarks>
    /// Each payslip contains complete calculation details for an individual employee.
    /// </remarks>
    public IReadOnlyList<Payslip> Payslips { get; init; } = new List<Payslip>();

    /// <summary>
    /// Gets or sets the aggregated summary of all payslips processed.
    /// </summary>
    /// <value>The <see cref="BulkPayslipSummary"/> containing totals across all employees.</value>
    /// <remarks>
    /// Provides consolidated totals for gross earnings, tax, contributions, and net pay.
    /// </remarks>
    public BulkPayslipSummary Summary { get; init; } = new();
}

/// <summary>
/// Represents aggregated summary totals for bulk payslip calculations.
/// </summary>
/// <remarks>
/// Provides consolidated financial totals across all employees processed in a bulk calculation.
/// Useful for payroll reporting and reconciliation purposes.
/// </remarks>
public class BulkPayslipSummary
{
    /// <summary>
    /// Gets or sets the total number of employees processed.
    /// </summary>
    /// <value>The count of employees included in the bulk calculation.</value>
    /// <example>50, 100, 250</example>
    public int TotalEmployees { get; init; }

    /// <summary>
    /// Gets or sets the total gross earnings across all employees.
    /// </summary>
    /// <value>The sum of all gross earnings in ZAR across all employees processed.</value>
    /// <example>1250000.00m, 2500000.00m</example>
    public decimal TotalGrossEarnings { get; init; }

    /// <summary>
    /// Gets or sets the total PAYE tax across all employees.
    /// </summary>
    /// <value>The sum of all PAYE amounts in ZAR across all employees processed.</value>
    /// <remarks>
    /// This represents the total income tax before ETI offsets.
    /// </remarks>
    /// <example>187500.00m, 375000.00m</example>
    public decimal TotalPAYE { get; init; }

    /// <summary>
    /// Gets or sets the total UIF contributions across all employees.
    /// </summary>
    /// <value>The sum of all UIF amounts in ZAR (employee + employer contributions).</value>
    /// <remarks>
    /// Includes both employee and employer UIF contributions (2% total).
    /// </remarks>
    /// <example>25000.00m, 50000.00m</example>
    public decimal TotalUIF { get; init; }

    /// <summary>
    /// Gets or sets the total Skills Development Levy across all employees.
    /// </summary>
    /// <value>The sum of all SDL amounts in ZAR paid by the employer.</value>
    /// <remarks>
    /// SDL is 1% of gross salary for companies with annual payroll exceeding R500,000.
    /// </remarks>
    /// <example>12500.00m, 25000.00m</example>
    public decimal TotalSDL { get; init; }

    /// <summary>
    /// Gets or sets the total Employment Tax Incentive across all employees.
    /// </summary>
    /// <value>The sum of all ETI amounts in ZAR for eligible employees.</value>
    /// <remarks>
    /// ETI reduces the net PAYE payable to SARS and encourages youth employment.
    /// </remarks>
    /// <example>45000.00m, 90000.00m</example>
    public decimal TotalETI { get; init; }

    /// <summary>
    /// Gets or sets the total net pay across all employees.
    /// </summary>
    /// <value>The sum of all net pay amounts in ZAR across all employees processed.</value>
    /// <remarks>
    /// This represents the total amount paid to all employees after deductions.
    /// </remarks>
    /// <example>875000.00m, 1750000.00m</example>
    public decimal TotalNetPay { get; init; }

    /// <summary>
    /// Gets or sets the total cost to company across all employees.
    /// </summary>
    /// <value>The sum of all cost-to-company amounts in ZAR across all employees processed.</value>
    /// <remarks>
    /// Includes gross pay plus all employer contributions (UIF, SDL, retirement, medical aid).
    /// </remarks>
    /// <example>1375000.00m, 2750000.00m</example>
    public decimal TotalCostToCompany { get; init; }
}
