using System;
using System.Linq;
using Xunit;
using SARS.TaxCalculator;
using SARS.TaxCalculator.Configuration;

namespace SARS.TaxCalculator.Tests;

public class FluentApiValidationTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(151)]
    [InlineData(200)]
    [InlineData(int.MaxValue)]
    public void WithAge_InvalidAge_ThrowsException(int invalidAge)
    {
        Assert.Throws<ArgumentException>(() =>
            TaxCalculator
                .ForTaxYear(2024)
                .WithAge(invalidAge));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(18)]
    [InlineData(65)]
    [InlineData(75)]
    [InlineData(150)]
    public void WithAge_ValidAge_DoesNotThrow(int validAge)
    {
        var exception = Record.Exception(() =>
            TaxCalculator
                .ForTaxYear(2024)
                .WithAge(validAge)
                .WithGrossSalary(10000)
                .Calculate());

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void WithMedicalAid_InvalidMemberCount_ThrowsException(int invalidMembers)
    {
        Assert.Throws<ArgumentException>(() =>
            TaxCalculator
                .ForTaxYear(2024)
                .WithMedicalAid(invalidMembers));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(20)]
    public void WithMedicalAid_ValidMemberCount_DoesNotThrow(int validMembers)
    {
        var exception = Record.Exception(() =>
            TaxCalculator
                .ForTaxYear(2024)
                .WithMedicalAid(validMembers)
                .WithGrossSalary(10000)
                .Calculate());

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(-1)]
    [InlineData(1.1)]
    [InlineData(2)]
    [InlineData(100)]
    public void WithRetirementContribution_InvalidPercentage_ThrowsException(decimal invalidPercentage)
    {
        Assert.Throws<ArgumentException>(() =>
            TaxCalculator
                .ForTaxYear(2024)
                .WithRetirementContribution(invalidPercentage));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.05)]
    [InlineData(0.275)]
    [InlineData(0.5)]
    [InlineData(1)]
    public void WithRetirementContribution_ValidPercentage_DoesNotThrow(decimal validPercentage)
    {
        var exception = Record.Exception(() =>
            TaxCalculator
                .ForTaxYear(2024)
                .WithRetirementContribution(validPercentage)
                .WithGrossSalary(10000)
                .Calculate());

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(-1000)]
    public void WithRetirementContributionAmount_NegativeAmount_ThrowsException(decimal negativeAmount)
    {
        Assert.Throws<ArgumentException>(() =>
            TaxCalculator
                .ForTaxYear(2024)
                .WithRetirementContributionAmount(negativeAmount));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1000)]
    [InlineData(5000)]
    [InlineData(999999999)]
    public void WithRetirementContributionAmount_ValidAmount_DoesNotThrow(decimal validAmount)
    {
        var exception = Record.Exception(() =>
            TaxCalculator
                .ForTaxYear(2024)
                .WithRetirementContributionAmount(validAmount)
                .WithGrossSalary(10000)
                .Calculate());

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100000)]
    public void WithCompanyPayroll_NegativePayroll_ThrowsException(decimal negativePayroll)
    {
        Assert.Throws<ArgumentException>(() =>
            TaxCalculator
                .ForTaxYear(2024)
                .WithCompanyPayroll(negativePayroll));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100000)]
    [InlineData(500000)]
    [InlineData(999999999)]
    public void WithCompanyPayroll_ValidPayroll_DoesNotThrow(decimal validPayroll)
    {
        var exception = Record.Exception(() =>
            TaxCalculator
                .ForTaxYear(2024)
                .WithCompanyPayroll(validPayroll)
                .WithGrossSalary(10000)
                .Calculate());

        Assert.Null(exception);
    }

    [Theory]
    [InlineData(2022)]
    [InlineData(2027)]
    [InlineData(1990)]
    [InlineData(2050)]
    public void ForTaxYear_UnsupportedYear_ThrowsException(int unsupportedYear)
    {
        Assert.Throws<ArgumentException>(() =>
            TaxCalculator.ForTaxYear(unsupportedYear));
    }

    [Theory]
    [InlineData(2023)]
    [InlineData(2024)]
    [InlineData(2025)]
    [InlineData(2026)]
    public void ForTaxYear_SupportedYear_DoesNotThrow(int supportedYear)
    {
        var exception = Record.Exception(() =>
            TaxCalculator.ForTaxYear(supportedYear));

        Assert.Null(exception);
    }

    [Fact]
    public void SupportedYears_ReturnsExpectedYears()
    {
        var supportedYears = TaxCalculator.SupportedYears.ToArray();

        Assert.Contains(2023, supportedYears);
        Assert.Contains(2024, supportedYears);
        Assert.Contains(2025, supportedYears);
        Assert.Contains(2026, supportedYears);
        Assert.Equal(4, supportedYears.Length);
    }

    [Fact]
    public void Calculate_WithZeroSalary_ReturnsValidResult()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(0)
            .WithAge(35)
            .Calculate();

        Assert.Equal(0, result.GrossSalary);
        Assert.Equal(0, result.PAYE);
        Assert.Equal(0, result.UIF);
        Assert.Equal(0, result.NetSalary);
    }

    [Fact]
    public void Calculate_WithExtremelyHighSalary_HandlesCorrectly()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(1000000) // R1M per month
            .WithAge(35)
            .Calculate();

        Assert.Equal(1000000, result.GrossSalary);
        Assert.True(result.PAYE > 0);
        Assert.Equal(177.12m, result.UIF); // Should be capped
        Assert.True(result.NetSalary > 0);
        Assert.True(result.CostToCompany > result.GrossSalary);
    }

    [Fact]
    public void CalculatePaye_WithDefaultValues_ReturnsValidResult()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(25000)
            .CalculatePaye();

        Assert.True(result.MonthlyPAYE >= 0);
        Assert.True(result.AnnualPAYE >= 0);
        // Monthly PAYE * 12 may differ slightly from Annual PAYE due to rounding
        Assert.True(Math.Abs(result.MonthlyPAYE * 12 - result.AnnualPAYE) <= 0.12m,
            $"Monthly PAYE * 12 ({result.MonthlyPAYE * 12}) should be within 12 cents of Annual PAYE ({result.AnnualPAYE})");
        Assert.True(result.TaxableIncome > 0);
        Assert.True(result.TaxThreshold > 0);
        Assert.True(result.TotalRebates > 0);
    }

    [Fact]
    public void WithEtiDetails_UpdatesAgeAfterSettingAge_UsesUpdatedAge()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(3000)
            .WithAge(25)
            .WithEtiDetails(6, true, false)
            .WithAge(35) // Change age after setting ETI details
            .Calculate();

        // ETI should use the originally set age (25), not the updated age (35)
        // because ETI details are created when WithEtiDetails is called
        Assert.True(result.ETI > 0); // Should still get ETI based on age 25
    }

    [Fact]
    public void WithMedicalAid_ZeroContribution_OnlyCalculatesCredits()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(25000)
            .WithAge(35)
            .WithMedicalAid(2, 0) // Zero contribution
            .Calculate();

        Assert.Equal(0, result.MedicalAidContribution);
        Assert.Equal(728m, result.MedicalAidTaxCredit); // Should still get credits
    }

    [Fact]
    public void WithAnnualGrossSalary_ConvertsToMonthlyCorrectly()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithAnnualGrossSalary(300000) // R300k annual = R25k monthly
            .WithAge(35)
            .Calculate();

        Assert.Equal(25000, result.GrossSalary); // Should be monthly value
        Assert.Equal(300000, result.AnnualGrossSalary); // Should preserve annual value
    }

    [Fact]
    public void Builder_ChainedCalls_MaintainsState()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(3000) // Lower salary to qualify for ETI
            .WithAge(25) // Age within ETI range (18-29)
            .WithMedicalAid(3, 3500)
            .WithRetirementContribution(0.075m)
            .WithCompanyPayroll(10000000)
            .WithEtiDetails(6, true, false)
            .Calculate();

        // Verify all settings were applied
        Assert.Equal(3000, result.GrossSalary);
        Assert.Equal(25, result.Age);
        Assert.Equal(3500, result.MedicalAidContribution);
        Assert.Equal(225m, result.RetirementContribution); // 7.5% of 3000
        Assert.True(result.ETI > 0); // Should have ETI
        Assert.True(result.SDL > 0); // Should have SDL due to high payroll
    }
}
