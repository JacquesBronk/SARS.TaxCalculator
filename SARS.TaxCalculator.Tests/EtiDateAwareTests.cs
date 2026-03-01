using System;
using Xunit;
using SARS.TaxCalculator;
using SARS.TaxCalculator.Calculators;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Inputs;

namespace SARS.TaxCalculator.Tests;

public class EtiDateAwareTests
{
    [Fact]
    public void GetEtiConfigForDate_March2025_ReturnsOldRates()
    {
        var config = TaxYearData.GetConfiguration(2026);

        var etiConfig = config.GetEtiConfigForDate(3, 2025);

        Assert.Equal(6500, etiConfig.MaxQualifyingSalary);
        Assert.Equal(4, etiConfig.Bands.Count);
        Assert.Equal(2000, etiConfig.Bands[0].MaxSalary);
        Assert.Equal(1500, etiConfig.Bands[0].FirstYearAmount);
        Assert.Equal(750, etiConfig.Bands[0].SecondYearAmount);
    }

    [Fact]
    public void GetEtiConfigForDate_April2025_ReturnsNewRates()
    {
        var config = TaxYearData.GetConfiguration(2026);

        var etiConfig = config.GetEtiConfigForDate(4, 2025);

        Assert.Equal(7500, etiConfig.MaxQualifyingSalary);
        Assert.Equal(4, etiConfig.Bands.Count);
        Assert.Equal(2499.99m, etiConfig.Bands[0].MaxSalary);
        Assert.Equal(2500, etiConfig.Bands[0].FirstYearAmount);
        Assert.Equal(1250, etiConfig.Bands[0].SecondYearAmount);
        Assert.True(etiConfig.Bands[0].UseRemunerationPercentage);
    }

    [Fact]
    public void GetEtiConfigForDate_NoOverrides_ReturnsDefault()
    {
        // 2025 config has no EtiConfigPeriods
        var config2025 = TaxYearData.GetConfiguration(2025);
        var etiConfig = config2025.GetEtiConfigForDate(6, 2024);
        Assert.Equal(6500, etiConfig.MaxQualifyingSalary);

        // 2027 config has no EtiConfigPeriods
        var config2027 = TaxYearData.GetConfiguration(2027);
        var etiConfig2027 = config2027.GetEtiConfigForDate(6, 2026);
        Assert.Equal(7500, etiConfig2027.MaxQualifyingSalary);
    }

    [Fact]
    public void FluentApi_ForPaymentDate_March2025_UsesOldRates()
    {
        // R7,000 salary: ineligible under old rates (max R6,500)
        var result = TaxCalculator.ForTaxYear(2026)
            .WithGrossSalary(7000)
            .WithAge(22)
            .WithEtiDetails(6)
            .ForPaymentDate(3, 2025)
            .Calculate();

        Assert.Equal(0, result.ETI);
    }

    [Fact]
    public void FluentApi_ForPaymentDate_April2025_UsesNewRates()
    {
        // R7,000 salary: eligible under new rates (max R7,500), Band 3 sliding scale
        var result = TaxCalculator.ForTaxYear(2026)
            .WithGrossSalary(7000)
            .WithAge(22)
            .WithEtiDetails(6)
            .ForPaymentDate(4, 2025)
            .Calculate();

        // Band 3: R1,500 - (R7,000 - R5,500) * 0.75 = R1,500 - R1,125 = R375
        Assert.Equal(375, result.ETI);
    }

    [Fact]
    public void FluentApi_NoPaymentDate_BackwardCompatible()
    {
        // Without ForPaymentDate, uses default EtiConfig (NEW rates)
        var result = TaxCalculator.ForTaxYear(2026)
            .WithGrossSalary(7000)
            .WithAge(22)
            .WithEtiDetails(6)
            .Calculate();

        // Band 3: R1,500 - (R7,000 - R5,500) * 0.75 = R1,500 - R1,125 = R375
        Assert.Equal(375, result.ETI);
    }

    [Fact]
    public void Payslip_March2025_UsesOldRates()
    {
        var config = TaxYearData.GetConfiguration(2026);
        var calculator = new PayslipCalculator(config);

        var input = new PayslipInput
        {
            EmployeeId = "EMP001",
            EmployeeName = "Test Employee",
            Age = 22,
            GrossSalary = 7000,
            PayMonth = 3,
            PayYear = 2025,
            IsEtiEligible = true,
            EmploymentMonths = 6,
            IsFirstTimeEmployee = true
        };

        var payslip = calculator.Calculate(input);

        // R7,000 exceeds old max qualifying salary of R6,500 - ineligible
        Assert.NotNull(payslip.ETI);
        Assert.Equal(0, payslip.ETI!.Amount);
        Assert.False(payslip.ETI.IsEligible);
    }

    [Fact]
    public void Payslip_April2025_UsesNewRates()
    {
        var config = TaxYearData.GetConfiguration(2026);
        var calculator = new PayslipCalculator(config);

        var input = new PayslipInput
        {
            EmployeeId = "EMP001",
            EmployeeName = "Test Employee",
            Age = 22,
            GrossSalary = 7000,
            PayMonth = 4,
            PayYear = 2025,
            IsEtiEligible = true,
            EmploymentMonths = 6,
            IsFirstTimeEmployee = true
        };

        var payslip = calculator.Calculate(input);

        // R7,000 eligible under new rates, Band 3: R1,500 - (R1,500 * 0.75) = R375
        Assert.NotNull(payslip.ETI);
        Assert.Equal(375, payslip.ETI!.Amount);
        Assert.True(payslip.ETI.IsEligible);
    }

    [Theory]
    [InlineData(3, 2025, 3000, 1000)] // March: old rates, Band 2 R1,500 - (R999 * 0.5) = R1,000
    [InlineData(4, 2025, 3000, 1500)] // April: new rates, Band 2 fixed R1,500
    [InlineData(3, 2025, 5000, 625)]  // March: old rates, Band 3 R750 - (R499 * 0.25) = R625
    [InlineData(4, 2025, 5000, 1500)] // April: new rates, Band 2 fixed R1,500
    [InlineData(3, 2025, 6500, 250)]  // March: old rates, Band 3 R750 - (R2,000 * 0.25) = R250
    [InlineData(4, 2025, 6500, 750)]  // April: new rates, Band 3 R1,500 - (R1,000 * 0.75) = R750
    [InlineData(3, 2025, 7000, 0)]    // March: old rates, salary exceeds R6,500 max
    [InlineData(4, 2025, 7000, 375)]  // April: new rates, Band 3 R1,500 - (R1,500 * 0.75) = R375
    public void FluentApi_VariousSalariesAndDates_ReturnsCorrectEti(
        int month, int year, decimal salary, decimal expectedEti)
    {
        var result = TaxCalculator.ForTaxYear(2026)
            .WithGrossSalary(salary)
            .WithAge(22)
            .WithEtiDetails(6)
            .ForPaymentDate(month, year)
            .Calculate();

        Assert.Equal(expectedEti, result.ETI);
    }

    [Fact]
    public void ForPaymentDate_InvalidMonth_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TaxCalculator.ForTaxYear(2026)
                .ForPaymentDate(0, 2025));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TaxCalculator.ForTaxYear(2026)
                .ForPaymentDate(13, 2025));
    }

    [Fact]
    public void ForPaymentDate_InvalidYear_ThrowsException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TaxCalculator.ForTaxYear(2026)
                .ForPaymentDate(3, 1999));
    }
}
