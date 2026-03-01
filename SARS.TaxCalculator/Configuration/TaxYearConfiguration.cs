using System;
using System.Collections.Generic;
using SARS.TaxCalculator.Models;

namespace SARS.TaxCalculator.Configuration;

/// <summary>
/// Represents the complete tax configuration for a specific tax year
/// </summary>
public class TaxYearConfiguration
{
    /// <summary>
    /// The tax year (e.g., 2023, 2024, 2025)
    /// </summary>
    public int Year { get; init; }

    /// <summary>
    /// Start date of the tax year (typically March 1)
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// End date of the tax year (typically February 28/29)
    /// </summary>
    public DateTime EndDate { get; init; }

    /// <summary>
    /// Tax brackets for this year
    /// </summary>
    public IReadOnlyList<TaxBracket> TaxBrackets { get; init; } = new List<TaxBracket>();

    /// <summary>
    /// Tax rebates for this year
    /// </summary>
    public IReadOnlyList<TaxRebate> TaxRebates { get; init; } = new List<TaxRebate>();

    /// <summary>
    /// Tax thresholds by age for this year
    /// </summary>
    public IReadOnlyList<TaxThreshold> TaxThresholds { get; init; } = new List<TaxThreshold>();

    /// <summary>
    /// Medical aid credit configuration for this year
    /// </summary>
    public MedicalAidCredit MedicalAidCredit { get; init; } = new();

    /// <summary>
    /// UIF configuration
    /// </summary>
    public UifConfiguration UifConfig { get; init; } = new();

    /// <summary>
    /// SDL configuration
    /// </summary>
    public SdlConfiguration SdlConfig { get; init; } = new();

    /// <summary>
    /// ETI configuration (default rates for the year)
    /// </summary>
    public EtiConfiguration EtiConfig { get; init; } = new();

    /// <summary>
    /// Optional date-specific ETI configurations for mid-year rate changes.
    /// Must be ordered by EffectiveFrom date ascending.
    /// </summary>
    public IReadOnlyList<DatedEtiConfiguration>? EtiConfigPeriods { get; init; }

    /// <summary>
    /// Gets the applicable ETI configuration for a specific pay month/year.
    /// Falls back to EtiConfig if no periods are defined or none match.
    /// </summary>
    public EtiConfiguration GetEtiConfigForDate(int payMonth, int payYear)
    {
        if (EtiConfigPeriods == null || EtiConfigPeriods.Count == 0)
            return EtiConfig;

        var payDate = new DateTime(payYear, payMonth, 1);
        EtiConfiguration? applicable = null;

        foreach (var period in EtiConfigPeriods)
        {
            if (payDate >= period.EffectiveFrom)
                applicable = period.Config;
        }

        return applicable ?? EtiConfig;
    }

    /// <summary>
    /// Retirement contribution limits
    /// </summary>
    public RetirementContributionLimits RetirementLimits { get; init; } = new();
}

/// <summary>
/// UIF (Unemployment Insurance Fund) configuration
/// </summary>
public class UifConfiguration
{
    /// <summary>
    /// Employee contribution rate (as decimal, e.g., 0.01 for 1%)
    /// </summary>
    public decimal EmployeeRate { get; init; }

    /// <summary>
    /// Employer contribution rate (as decimal, e.g., 0.01 for 1%)
    /// </summary>
    public decimal EmployerRate { get; init; }

    /// <summary>
    /// Monthly income ceiling for UIF contributions
    /// </summary>
    public decimal MonthlyCeiling { get; init; }

    /// <summary>
    /// Annual income ceiling for UIF contributions
    /// </summary>
    public decimal AnnualCeiling => MonthlyCeiling * 12;
}

/// <summary>
/// SDL (Skills Development Levy) configuration
/// </summary>
public class SdlConfiguration
{
    /// <summary>
    /// SDL rate (as decimal, e.g., 0.01 for 1%)
    /// </summary>
    public decimal Rate { get; init; }

    /// <summary>
    /// Annual payroll exemption threshold
    /// </summary>
    public decimal ExemptionThreshold { get; init; }
}

/// <summary>
/// ETI (Employment Tax Incentive) configuration
/// </summary>
public class EtiConfiguration
{
    /// <summary>
    /// Minimum age for qualifying employees
    /// </summary>
    public int MinAge { get; init; }

    /// <summary>
    /// Maximum age for qualifying employees
    /// </summary>
    public int MaxAge { get; init; }

    /// <summary>
    /// Maximum qualifying monthly salary
    /// </summary>
    public decimal MaxQualifyingSalary { get; init; }

    /// <summary>
    /// ETI bands for different salary ranges and periods
    /// </summary>
    public IReadOnlyList<EtiBand> Bands { get; init; } = new List<EtiBand>();
}

/// <summary>
/// Represents an ETI band with salary range and incentive amounts
/// </summary>
public class EtiBand
{
    /// <summary>
    /// Minimum salary for this band
    /// </summary>
    public decimal MinSalary { get; init; }

    /// <summary>
    /// Maximum salary for this band
    /// </summary>
    public decimal MaxSalary { get; init; }

    /// <summary>
    /// Monthly incentive amount for the first 12 months
    /// </summary>
    public decimal FirstYearAmount { get; init; }

    /// <summary>
    /// Monthly incentive amount for the second 12 months
    /// </summary>
    public decimal SecondYearAmount { get; init; }

    /// <summary>
    /// Reduction rate for salaries above minimum (if applicable)
    /// </summary>
    public decimal? ReductionRate { get; init; }

    /// <summary>
    /// When true, ETI is calculated as a percentage of remuneration (60%/30%) capped at FirstYearAmount/SecondYearAmount
    /// </summary>
    public bool UseRemunerationPercentage { get; init; }
}

/// <summary>
/// Represents an ETI configuration that applies from a specific date
/// </summary>
public class DatedEtiConfiguration
{
    /// <summary>
    /// The date from which this ETI configuration applies
    /// </summary>
    public DateTime EffectiveFrom { get; init; }

    /// <summary>
    /// The ETI configuration for this period
    /// </summary>
    public EtiConfiguration Config { get; init; } = new();
}

/// <summary>
/// Retirement contribution limits configuration
/// </summary>
public class RetirementContributionLimits
{
    /// <summary>
    /// Maximum percentage of taxable income that can be deducted
    /// </summary>
    public decimal MaxPercentage { get; init; }

    /// <summary>
    /// Annual cap on retirement contribution deductions
    /// </summary>
    public decimal AnnualCap { get; init; }
}
