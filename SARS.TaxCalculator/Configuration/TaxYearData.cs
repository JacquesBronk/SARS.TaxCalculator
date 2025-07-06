using System;
using System.Collections.Generic;
using SARS.TaxCalculator.Models;

namespace SARS.TaxCalculator.Configuration;

/// <summary>
/// Provides tax year configurations for supported years
/// Source: SARS official tax rates and deduction tables
/// Reference: https://www.sars.gov.za/tax-rates/income-tax/rates-of-tax-for-individuals/
/// </summary>
public static class TaxYearData
{
    private static readonly Dictionary<int, TaxYearConfiguration> Configurations = new()
    {
        [2023] = Create2023Configuration(),
        [2024] = Create2024Configuration(),
        [2025] = Create2025Configuration(),
        [2026] = Create2026Configuration()
    };

    /// <summary>
    /// Gets the tax year configuration for a specific year
    /// </summary>
    /// <param name="year">The tax year</param>
    /// <returns>The tax year configuration</returns>
    /// <exception cref="ArgumentException">If the year is not supported</exception>
    public static TaxYearConfiguration GetConfiguration(int year)
    {
        if (!Configurations.TryGetValue(year, out var config))
        {
            throw new ArgumentException($"Tax year {year} is not supported. Supported years are: {string.Join(", ", Configurations.Keys)}", nameof(year));
        }

        return config;
    }

    /// <summary>
    /// Gets all supported tax years
    /// </summary>
    public static IEnumerable<int> SupportedYears => Configurations.Keys;

    private static TaxYearConfiguration Create2023Configuration()
    {
        return new TaxYearConfiguration
        {
            Year = 2023,
            StartDate = new DateTime(2022, 3, 1),
            EndDate = new DateTime(2023, 2, 28),
            TaxBrackets = new List<TaxBracket>
            {
                new() { MinIncome = 0, MaxIncome = 237100, BaseTax = 0, Rate = 18 },
                new() { MinIncome = 237101, MaxIncome = 370500, BaseTax = 42678, Rate = 26 },
                new() { MinIncome = 370501, MaxIncome = 512800, BaseTax = 77362, Rate = 31 },
                new() { MinIncome = 512801, MaxIncome = 673000, BaseTax = 121475, Rate = 36 },
                new() { MinIncome = 673001, MaxIncome = 857900, BaseTax = 179147, Rate = 39 },
                new() { MinIncome = 857901, MaxIncome = 1817000, BaseTax = 251258, Rate = 41 },
                new() { MinIncome = 1817001, MaxIncome = null, BaseTax = 644489, Rate = 45 }
            },
            TaxRebates = new List<TaxRebate>
            {
                new() { Type = RebateType.Primary, Amount = 17235, MinAge = null },
                new() { Type = RebateType.Secondary, Amount = 9444, MinAge = 65 },
                new() { Type = RebateType.Tertiary, Amount = 3145, MinAge = 75 }
            },
            TaxThresholds = new List<TaxThreshold>
            {
                new() { MinAge = null, MaxAge = 64, Amount = 95750 },
                new() { MinAge = 65, MaxAge = 74, Amount = 148217 },
                new() { MinAge = 75, MaxAge = null, Amount = 165689 }
            },
            MedicalAidCredit = new MedicalAidCredit
            {
                MainMemberCredit = 364,
                FirstDependentCredit = 364,
                AdditionalDependentCredit = 246
            },
            UifConfig = new UifConfiguration
            {
                EmployeeRate = 0.01m,
                EmployerRate = 0.01m,
                MonthlyCeiling = 17712
            },
            SdlConfig = new SdlConfiguration
            {
                Rate = 0.01m,
                ExemptionThreshold = 500000
            },
            EtiConfig = new EtiConfiguration
            {
                MinAge = 18,
                MaxAge = 29,
                MaxQualifyingSalary = 6500,
                Bands = new List<EtiBand>
                {
                    new() { MinSalary = 0, MaxSalary = 2000, FirstYearAmount = 1500, SecondYearAmount = 750 },
                    new() { MinSalary = 2001, MaxSalary = 4500, FirstYearAmount = 1500, SecondYearAmount = 750, ReductionRate = 0.5m },
                    new() { MinSalary = 4501, MaxSalary = 6500, FirstYearAmount = 750, SecondYearAmount = 375, ReductionRate = 0.25m },
                    new() { MinSalary = 6501, MaxSalary = decimal.MaxValue, FirstYearAmount = 0, SecondYearAmount = 0 }
                }
            },
            RetirementLimits = new RetirementContributionLimits
            {
                MaxPercentage = 0.275m,
                AnnualCap = 350000
            }
        };
    }

    private static TaxYearConfiguration Create2024Configuration()
    {
        // 2024 tax year (1 March 2023 - 29 February 2024)
        // Source: SARS Tax Rates for Individuals
        // Reference: https://www.sars.gov.za/tax-rates/income-tax/rates-of-tax-for-individuals/
        // No changes from 2023 per SARS announcement
        return new TaxYearConfiguration
        {
            Year = 2024,
            StartDate = new DateTime(2023, 3, 1),
            EndDate = new DateTime(2024, 2, 29), // 2024 is a leap year
            TaxBrackets = new List<TaxBracket>
            {
                new() { MinIncome = 0, MaxIncome = 237100, BaseTax = 0, Rate = 18 },
                new() { MinIncome = 237101, MaxIncome = 370500, BaseTax = 42678, Rate = 26 },
                new() { MinIncome = 370501, MaxIncome = 512800, BaseTax = 77362, Rate = 31 },
                new() { MinIncome = 512801, MaxIncome = 673000, BaseTax = 121475, Rate = 36 },
                new() { MinIncome = 673001, MaxIncome = 857900, BaseTax = 179147, Rate = 39 },
                new() { MinIncome = 857901, MaxIncome = 1817000, BaseTax = 251258, Rate = 41 },
                new() { MinIncome = 1817001, MaxIncome = null, BaseTax = 644489, Rate = 45 }
            },
            TaxRebates = new List<TaxRebate>
            {
                new() { Type = RebateType.Primary, Amount = 17235, MinAge = null },
                new() { Type = RebateType.Secondary, Amount = 9444, MinAge = 65 },
                new() { Type = RebateType.Tertiary, Amount = 3145, MinAge = 75 }
            },
            TaxThresholds = new List<TaxThreshold>
            {
                new() { MinAge = null, MaxAge = 64, Amount = 95750 },
                new() { MinAge = 65, MaxAge = 74, Amount = 148217 },
                new() { MinAge = 75, MaxAge = null, Amount = 165689 }
            },
            MedicalAidCredit = new MedicalAidCredit
            {
                MainMemberCredit = 364,
                FirstDependentCredit = 364,
                AdditionalDependentCredit = 246
            },
            UifConfig = new UifConfiguration
            {
                EmployeeRate = 0.01m,
                EmployerRate = 0.01m,
                MonthlyCeiling = 17712
            },
            SdlConfig = new SdlConfiguration
            {
                Rate = 0.01m,
                ExemptionThreshold = 500000
            },
            EtiConfig = new EtiConfiguration
            {
                MinAge = 18,
                MaxAge = 29,
                MaxQualifyingSalary = 6500,
                Bands = new List<EtiBand>
                {
                    new() { MinSalary = 0, MaxSalary = 2000, FirstYearAmount = 1500, SecondYearAmount = 750 },
                    new() { MinSalary = 2001, MaxSalary = 4500, FirstYearAmount = 1500, SecondYearAmount = 750, ReductionRate = 0.5m },
                    new() { MinSalary = 4501, MaxSalary = 6500, FirstYearAmount = 750, SecondYearAmount = 375, ReductionRate = 0.25m },
                    new() { MinSalary = 6501, MaxSalary = decimal.MaxValue, FirstYearAmount = 0, SecondYearAmount = 0 }
                }
            },
            RetirementLimits = new RetirementContributionLimits
            {
                MaxPercentage = 0.275m,
                AnnualCap = 350000
            }
        };
    }

    private static TaxYearConfiguration Create2025Configuration()
    {
        // 2025 tax year (1 March 2024 - 28 February 2025)
        // Source: SARS Tax Rates for Individuals - "No changes" announced 21 February 2024
        // Reference: https://www.sars.gov.za/tax-rates/income-tax/rates-of-tax-for-individuals/
        // Note: ETI changes effective 1 April 2025 do NOT apply to this tax year (ends 28 Feb 2025)
        return new TaxYearConfiguration
        {
            Year = 2025,
            StartDate = new DateTime(2024, 3, 1),
            EndDate = new DateTime(2025, 2, 28),
            TaxBrackets = new List<TaxBracket>
            {
                new() { MinIncome = 0, MaxIncome = 237100, BaseTax = 0, Rate = 18 },
                new() { MinIncome = 237101, MaxIncome = 370500, BaseTax = 42678, Rate = 26 },
                new() { MinIncome = 370501, MaxIncome = 512800, BaseTax = 77362, Rate = 31 },
                new() { MinIncome = 512801, MaxIncome = 673000, BaseTax = 121475, Rate = 36 },
                new() { MinIncome = 673001, MaxIncome = 857900, BaseTax = 179147, Rate = 39 },
                new() { MinIncome = 857901, MaxIncome = 1817000, BaseTax = 251258, Rate = 41 },
                new() { MinIncome = 1817001, MaxIncome = null, BaseTax = 644489, Rate = 45 }
            },
            TaxRebates = new List<TaxRebate>
            {
                new() { Type = RebateType.Primary, Amount = 17235, MinAge = null },
                new() { Type = RebateType.Secondary, Amount = 9444, MinAge = 65 },
                new() { Type = RebateType.Tertiary, Amount = 3145, MinAge = 75 }
            },
            TaxThresholds = new List<TaxThreshold>
            {
                new() { MinAge = null, MaxAge = 64, Amount = 95750 },
                new() { MinAge = 65, MaxAge = 74, Amount = 148217 },
                new() { MinAge = 75, MaxAge = null, Amount = 165689 }
            },
            MedicalAidCredit = new MedicalAidCredit
            {
                MainMemberCredit = 364,
                FirstDependentCredit = 364,
                AdditionalDependentCredit = 246
            },
            UifConfig = new UifConfiguration
            {
                EmployeeRate = 0.01m,
                EmployerRate = 0.01m,
                MonthlyCeiling = 17712
            },
            SdlConfig = new SdlConfiguration
            {
                Rate = 0.01m,
                ExemptionThreshold = 500000
            },
            EtiConfig = new EtiConfiguration
            {
                MinAge = 18,
                MaxAge = 29,
                MaxQualifyingSalary = 6500,
                Bands = new List<EtiBand>
                {
                    new() { MinSalary = 0, MaxSalary = 2000, FirstYearAmount = 1500, SecondYearAmount = 750 },
                    new() { MinSalary = 2001, MaxSalary = 4500, FirstYearAmount = 1500, SecondYearAmount = 750, ReductionRate = 0.5m },
                    new() { MinSalary = 4501, MaxSalary = 6500, FirstYearAmount = 750, SecondYearAmount = 375, ReductionRate = 0.25m },
                    new() { MinSalary = 6501, MaxSalary = decimal.MaxValue, FirstYearAmount = 0, SecondYearAmount = 0 }
                }
            },
            RetirementLimits = new RetirementContributionLimits
            {
                MaxPercentage = 0.275m,
                AnnualCap = 350000
            }
        };
    }

    private static TaxYearConfiguration Create2026Configuration()
    {
        // 2026 tax year (1 March 2025 - 28 February 2026)
        // Source: SARS Budget Speech 12 March 2025 - "tax brackets and medical tax credits will remain unchanged"
        // Reference: https://www.sars.gov.za/latest-news/2026-employees-tax-deduction-tables/
        // ETI changes effective 1 April 2025 continue into 2026
        return new TaxYearConfiguration
        {
            Year = 2026,
            StartDate = new DateTime(2025, 3, 1),
            EndDate = new DateTime(2026, 2, 28),
            TaxBrackets = new List<TaxBracket>
            {
                new() { MinIncome = 0, MaxIncome = 237100, BaseTax = 0, Rate = 18 },
                new() { MinIncome = 237101, MaxIncome = 370500, BaseTax = 42678, Rate = 26 },
                new() { MinIncome = 370501, MaxIncome = 512800, BaseTax = 77362, Rate = 31 },
                new() { MinIncome = 512801, MaxIncome = 673000, BaseTax = 121475, Rate = 36 },
                new() { MinIncome = 673001, MaxIncome = 857900, BaseTax = 179147, Rate = 39 },
                new() { MinIncome = 857901, MaxIncome = 1817000, BaseTax = 251258, Rate = 41 },
                new() { MinIncome = 1817001, MaxIncome = null, BaseTax = 644489, Rate = 45 }
            },
            TaxRebates = new List<TaxRebate>
            {
                new() { Type = RebateType.Primary, Amount = 17235, MinAge = null },
                new() { Type = RebateType.Secondary, Amount = 9444, MinAge = 65 },
                new() { Type = RebateType.Tertiary, Amount = 3145, MinAge = 75 }
            },
            TaxThresholds = new List<TaxThreshold>
            {
                new() { MinAge = null, MaxAge = 64, Amount = 95750 },
                new() { MinAge = 65, MaxAge = 74, Amount = 148217 },
                new() { MinAge = 75, MaxAge = null, Amount = 165689 }
            },
            MedicalAidCredit = new MedicalAidCredit
            {
                MainMemberCredit = 364,
                FirstDependentCredit = 364,
                AdditionalDependentCredit = 246
            },
            UifConfig = new UifConfiguration
            {
                EmployeeRate = 0.01m,
                EmployerRate = 0.01m,
                MonthlyCeiling = 17712
            },
            SdlConfig = new SdlConfiguration
            {
                Rate = 0.01m,
                ExemptionThreshold = 500000
            },
            EtiConfig = new EtiConfiguration
            {
                MinAge = 18,
                MaxAge = 29,
                // ETI changes effective 1 April 2025
                // Source: https://www.sars.gov.za/latest-news/employment-tax-incentive-eti-changes-with-effect-from-1-april-2025/
                // Changes: MaxQualifyingSalary increased to R7,500
                // Maximum ETI for employees working 160+ hours: R2,500
                // ETI is prorated for employees working < 160 hours
                MaxQualifyingSalary = 7500,
                // ETI bands updated for April 2025 changes
                // Source: Employment Tax Incentive Act - Section 7
                // SARS ETI Guide (LAPD-ETI-G01) - Updated April 2025
                // Maximum ETI: R2,500 (first year) / R1,250 (second year) for 160+ hours worked
                Bands = new List<EtiBand>
                {
                    // Band 1: R0 - R2,499.99 - 60% of remuneration (capped at R2,500/R1,250)
                    new() { MinSalary = 0, MaxSalary = 2499.99m, FirstYearAmount = 2500, SecondYearAmount = 1250 },
                    // Band 2: R2,500 - R5,499.99 - Fixed amounts
                    new() { MinSalary = 2500, MaxSalary = 5499.99m, FirstYearAmount = 1500, SecondYearAmount = 750 },
                    // Band 3: R5,500 - R7,499.99 - Sliding scale reduction
                    new() { MinSalary = 5500, MaxSalary = 7499.99m, FirstYearAmount = 1500, SecondYearAmount = 750, ReductionRate = 0.75m },
                    // Band 4: R7,500+ - No ETI
                    new() { MinSalary = 7500, MaxSalary = decimal.MaxValue, FirstYearAmount = 0, SecondYearAmount = 0 }
                }
            },
            RetirementLimits = new RetirementContributionLimits
            {
                MaxPercentage = 0.275m,
                AnnualCap = 350000
            }
        };
    }
}
