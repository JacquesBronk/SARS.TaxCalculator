namespace SARS.TaxCalculator.Models.Inputs;

/// <summary>
/// Represents input data required for comprehensive payslip calculations including PAYE, UIF, SDL, and ETI.
/// </summary>
/// <example>
/// <code>
/// var input = new PayslipInput
/// {
///     EmployeeId = "EMP001",
///     EmployeeName = "John Doe",
///     Age = 35,
///     GrossSalary = 25000,
///     MedicalAidMembers = 3,
///     MedicalAidContribution = 3500,
///     RetirementContribution = 1875,
///     CompanyAnnualPayroll = 5000000
/// };
/// </code>
/// </example>
public class PayslipInput
{
    /// <summary>
    /// Gets or sets the unique identifier for the employee.
    /// </summary>
    /// <value>The employee identifier, typically an alphanumeric code.</value>
    /// <example>"EMP001", "12345", "ADMIN_USER"</example>
    public string EmployeeId { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name of the employee.
    /// </summary>
    /// <value>The employee's full legal name as it appears on official documents.</value>
    /// <example>"John Doe", "Jane Smith", "Dr. Michael Johnson"</example>
    public string EmployeeName { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee's South African tax number.
    /// </summary>
    /// <value>The 10-digit SARS tax reference number.</value>
    /// <example>"1234567890", "9876543210"</example>
    public string TaxNumber { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the employee's age in years for tax calculation purposes.
    /// </summary>
    /// <value>The age must be between 16 and 150 years for valid tax calculations.</value>
    /// <remarks>
    /// Age determines applicable tax rebates:
    /// - Under 65: Primary rebate only
    /// - 65-74: Primary + Secondary rebate
    /// - 75+: Primary + Secondary + Tertiary rebate
    /// </remarks>
    /// <example>25, 35, 65, 75</example>
    public int Age { get; init; }

    /// <summary>
    /// Gets or sets the employee's gross salary amount.
    /// </summary>
    /// <value>The gross salary in South African Rand (ZAR), before any deductions.</value>
    /// <remarks>
    /// This amount is interpreted based on <see cref="IsAnnualSalary"/>:
    /// - If <c>true</c>: Annual gross salary
    /// - If <c>false</c>: Monthly gross salary
    /// </remarks>
    /// <example>25000.00m (monthly), 300000.00m (annual)</example>
    public decimal GrossSalary { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="GrossSalary"/> represents an annual amount.
    /// </summary>
    /// <value><c>true</c> if the gross salary is annual; <c>false</c> if monthly.</value>
    /// <remarks>
    /// When <c>true</c>, the gross salary will be divided by 12 for monthly calculations.
    /// When <c>false</c>, the gross salary is treated as a monthly amount.
    /// </remarks>
    public bool IsAnnualSalary { get; init; }

    /// <summary>
    /// Gets or sets the month for which the payslip is being calculated.
    /// </summary>
    /// <value>The month number (1-12) where 1 = January, 12 = December.</value>
    /// <example>1 (January), 6 (June), 12 (December)</example>
    public int PayMonth { get; init; }

    /// <summary>
    /// Gets or sets the year for which the payslip is being calculated.
    /// </summary>
    /// <value>The calendar year (e.g., 2024, 2025, 2026).</value>
    /// <remarks>
    /// This determines which tax year configuration to use for calculations.
    /// Supported years: 2023, 2024, 2025, 2026.
    /// </remarks>
    /// <example>2024, 2025, 2026</example>
    public int PayYear { get; init; }

    /// <summary>
    /// Gets or sets the number of medical aid members including the main member.
    /// </summary>
    /// <value>
    /// The total count of medical aid members (1 for main member only, 2+ for dependents).
    /// Set to 0 if no medical aid coverage.
    /// </value>
    /// <remarks>
    /// Medical aid tax credits are calculated as:
    /// - Main member: R364/month
    /// - First dependent: R364/month
    /// - Additional dependents: R246/month each
    /// </remarks>
    /// <example>0 (no medical aid), 1 (main member only), 4 (main member + 3 dependents)</example>
    public int MedicalAidMembers { get; init; }

    /// <summary>
    /// Gets or sets the monthly medical aid contribution amount paid by the employee.
    /// </summary>
    /// <value>The employee's monthly medical aid contribution in ZAR.</value>
    /// <remarks>
    /// This amount is deducted from the employee's gross salary before PAYE calculation.
    /// Does not affect the medical aid tax credit calculation.
    /// </remarks>
    /// <example>0.00m (no contribution), 1500.00m, 3500.00m</example>
    public decimal MedicalAidContribution { get; init; }

    /// <summary>
    /// Gets or sets the monthly retirement fund contribution amount paid by the employee.
    /// </summary>
    /// <value>The employee's monthly retirement contribution in ZAR.</value>
    /// <remarks>
    /// This amount is deducted from gross salary before PAYE calculation.
    /// Annual limit: 27.5% of taxable income, maximum R350,000 per year.
    /// </remarks>
    /// <example>0.00m (no contribution), 1875.00m (7.5% of R25,000), 5000.00m</example>
    public decimal RetirementContribution { get; init; }

    /// <summary>
    /// Gets or sets the monthly retirement fund contribution amount paid by the employer.
    /// </summary>
    /// <value>The employer's monthly retirement contribution in ZAR.</value>
    /// <remarks>
    /// This amount does not affect employee PAYE calculations but is included in cost-to-company calculations.
    /// Typically matches or supplements the employee contribution.
    /// </remarks>
    /// <example>0.00m (no employer contribution), 1875.00m (matching employee), 3750.00m (double matching)</example>
    public decimal EmployerRetirementContribution { get; init; }

    /// <summary>
    /// Gets or sets the monthly medical aid contribution amount paid by the employer.
    /// </summary>
    /// <value>The employer's monthly medical aid contribution in ZAR.</value>
    /// <remarks>
    /// This amount does not affect employee PAYE calculations but is included in cost-to-company calculations.
    /// Often used when employer subsidizes employee medical aid premiums.
    /// </remarks>
    /// <example>0.00m (no employer contribution), 1000.00m, 2500.00m</example>
    public decimal EmployerMedicalAidContribution { get; init; }

    /// <summary>
    /// Gets or sets the amount of other monthly deductions from the employee's salary.
    /// </summary>
    /// <value>The total of all other deductions in ZAR (e.g., union fees, garnishments, loans).</value>
    /// <remarks>
    /// These deductions are subtracted from net salary after tax calculations.
    /// Does not include standard deductions like PAYE, UIF, medical aid, or retirement contributions.
    /// </remarks>
    /// <example>0.00m (no other deductions), 500.00m (union fees), 1200.00m (loan repayment)</example>
    public decimal OtherDeductions { get; init; }

    /// <summary>
    /// Gets or sets the company's total annual payroll for SDL calculation purposes.
    /// </summary>
    /// <value>The company's total annual payroll in ZAR. Default is 1,000,000.</value>
    /// <remarks>
    /// SDL (Skills Development Levy) exemption rules:
    /// - Companies with annual payroll ≤ R500,000: Exempt from SDL
    /// - Companies with annual payroll > R500,000: Pay 1% SDL
    /// </remarks>
    /// <example>400000.00m (exempt), 500000.00m (at threshold), 5000000.00m (liable)</example>
    public decimal CompanyAnnualPayroll { get; init; } = 1000000;

    /// <summary>
    /// Gets or sets a value indicating whether the employee is eligible for Employment Tax Incentive (ETI).
    /// </summary>
    /// <value><c>true</c> if the employee qualifies for ETI; otherwise, <c>false</c>.</value>
    /// <remarks>
    /// ETI eligibility criteria:
    /// - Age: 18-29 years (SEZ employees exempt from age limit)
    /// - Salary: Maximum R7,500 per month
    /// - Employment duration: Maximum 24 months for non-first-time employees
    /// This property allows manual override of automatic eligibility calculation.
    /// </remarks>
    public bool IsEtiEligible { get; init; }

    /// <summary>
    /// Gets or sets the number of months the employee has been employed for ETI calculation purposes.
    /// </summary>
    /// <value>The employment duration in months (1-24 for ETI eligibility).</value>
    /// <remarks>
    /// ETI employment period limits:
    /// - First-time employees: No limit on employment duration
    /// - Experienced employees: Maximum 24 months ETI eligibility
    /// - Used to determine ETI rate (Year 1 vs Year 2 rates)
    /// </remarks>
    /// <example>1 (first month), 12 (one year), 24 (maximum for experienced)</example>
    public int EmploymentMonths { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether this is the employee's first-time employment.
    /// </summary>
    /// <value><c>true</c> if this is the employee's first job; otherwise, <c>false</c>.</value>
    /// <remarks>
    /// First-time employee status affects ETI calculations:
    /// - First-time employees: Higher ETI rates and no employment duration limit
    /// - Experienced employees: Lower ETI rates and 24-month maximum
    /// </remarks>
    public bool IsFirstTimeEmployee { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the employee works in a Special Economic Zone (SEZ).
    /// </summary>
    /// <value><c>true</c> if the employee works in an SEZ; otherwise, <c>false</c>.</value>
    /// <remarks>
    /// Special Economic Zone provisions:
    /// - SEZ employees are exempt from ETI age restrictions (18-29 years)
    /// - SEZ employees can qualify for ETI regardless of age
    /// - All other ETI criteria still apply (salary limits, employment duration)
    /// </remarks>
    public bool WorksInSpecialEconomicZone { get; init; }
}