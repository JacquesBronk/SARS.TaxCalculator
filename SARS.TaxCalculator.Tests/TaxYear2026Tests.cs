using System;
using Xunit;
using SARS.TaxCalculator;
using SARS.TaxCalculator.Configuration;

namespace SARS.TaxCalculator.Tests;

public class TaxYear2026Tests
{
    [Fact]
    public void ForTaxYear2026_ValidConfiguration_ReturnsBuilder()
    {
        var builder = TaxCalculator.ForTaxYear(2026);
        Assert.NotNull(builder);
    }

    [Fact]
    public void TaxYear2026Configuration_ValidValues_CorrectDates()
    {
        var config = TaxYearData.GetConfiguration(2026);

        Assert.Equal(2026, config.Year);
        Assert.Equal(new DateTime(2025, 3, 1), config.StartDate);
        Assert.Equal(new DateTime(2026, 2, 28), config.EndDate);
    }

    [Fact]
    public void TaxYear2026_BasicCalculation_ReturnsExpectedResult()
    {
        var result = TaxCalculator
            .ForTaxYear(2026)
            .WithGrossSalary(25000)
            .WithAge(35)
            .WithMedicalAid(2, 3500)
            .WithRetirementContribution(0.075m)
            .Calculate();

        Assert.Equal(25000, result.GrossSalary);
        Assert.Equal(35, result.Age);
        Assert.Equal(2026, result.TaxYear);
        Assert.True(result.NetSalary > 0);
        Assert.True(result.PAYE > 0);
        Assert.Equal(177.12m, result.UIF); // Capped at ceiling
        Assert.Equal(1875m, result.RetirementContribution); // 7.5% of 25000
        Assert.Equal(3500, result.MedicalAidContribution);
        Assert.Equal(728m, result.MedicalAidTaxCredit); // R364 + R364 for 2 members
    }

    [Fact]
    public void TaxYear2026_TaxBrackets_SameAsCurrentYears()
    {
        var config2026 = TaxYearData.GetConfiguration(2026);
        var config2025 = TaxYearData.GetConfiguration(2025);

        Assert.Equal(config2025.TaxBrackets.Count, config2026.TaxBrackets.Count);

        for (int i = 0; i < config2025.TaxBrackets.Count; i++)
        {
            var bracket2025 = config2025.TaxBrackets[i];
            var bracket2026 = config2026.TaxBrackets[i];

            Assert.Equal(bracket2025.MinIncome, bracket2026.MinIncome);
            Assert.Equal(bracket2025.MaxIncome, bracket2026.MaxIncome);
            Assert.Equal(bracket2025.Rate, bracket2026.Rate);
            Assert.Equal(bracket2025.BaseTax, bracket2026.BaseTax);
        }
    }

    [Fact]
    public void TaxYear2026_TaxRebates_SameAsCurrentYears()
    {
        var config2026 = TaxYearData.GetConfiguration(2026);
        var config2025 = TaxYearData.GetConfiguration(2025);

        Assert.Equal(config2025.TaxRebates.Count, config2026.TaxRebates.Count);

        for (int i = 0; i < config2025.TaxRebates.Count; i++)
        {
            var rebate2025 = config2025.TaxRebates[i];
            var rebate2026 = config2026.TaxRebates[i];

            Assert.Equal(rebate2025.Amount, rebate2026.Amount);
            Assert.Equal(rebate2025.MinAge, rebate2026.MinAge);
            Assert.Equal(rebate2025.Type, rebate2026.Type);
        }
    }

    [Fact]
    public void TaxYear2026_EtiConfiguration_ReflectsApril2025Changes()
    {
        var config2026 = TaxYearData.GetConfiguration(2026);
        var config2025 = TaxYearData.GetConfiguration(2025);

        // Age limits remain the same
        Assert.Equal(config2025.EtiConfig.MinAge, config2026.EtiConfig.MinAge);
        Assert.Equal(config2025.EtiConfig.MaxAge, config2026.EtiConfig.MaxAge);

        // ETI changes effective April 1, 2025 (within 2026 tax year)
        Assert.Equal(7500, config2026.EtiConfig.MaxQualifyingSalary); // Increased from 6500
        Assert.Equal(4, config2026.EtiConfig.Bands.Count);

        // Verify new maximum ETI amounts
        var band1 = config2026.EtiConfig.Bands[0];
        Assert.Equal(2500, band1.FirstYearAmount); // Increased from 1500
        Assert.Equal(1250, band1.SecondYearAmount); // Increased from 750
    }
}
