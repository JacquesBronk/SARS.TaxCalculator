using System;
using Xunit;
using SARS.TaxCalculator.Calculators;
using SARS.TaxCalculator.Configuration;

namespace SARS.TaxCalculator.Tests;

public class PayeCalculatorTests
{
    private readonly PayeCalculator _calculator;
    private readonly TaxYearConfiguration _config;

    public PayeCalculatorTests()
    {
        _config = TaxYearData.GetConfiguration(2024);
        _calculator = new PayeCalculator(_config);
    }

    [Theory]
    [InlineData(0, 30, 0, 0)] // No income
    [InlineData(95750, 30, 0, 0)] // At threshold
    [InlineData(100000, 30, 0, 63.75)] // Just above threshold
    [InlineData(250000, 30, 0, 2399.73)] // First bracket
    [InlineData(400000, 30, 0, 5772.64)] // Second bracket
    [InlineData(600000, 30, 0, 11302.64)] // Third bracket
    [InlineData(1000000, 30, 0, 24356.97)] // Fourth bracket
    [InlineData(2000000, 30, 0, 59133.63)] // Fifth bracket
    public void CalculateMonthlyPaye_VariousIncomes_ReturnsCorrectPaye(
        decimal annualIncome, int age, int medicalAidMembers, decimal expectedMonthlyPaye)
    {
        var monthlyIncome = annualIncome / 12;
        var result = _calculator.CalculateMonthlyPaye(monthlyIncome, age, medicalAidMembers);

        Assert.Equal(expectedMonthlyPaye, result, 2);
    }

    [Theory]
    [InlineData(30, 95750)] // Under 65
    [InlineData(65, 148217)] // 65-74
    [InlineData(75, 165689)] // 75+
    public void IsTaxPayable_AtThreshold_ReturnsFalse(int age, decimal threshold)
    {
        var result = _calculator.IsTaxPayable(threshold, age);
        Assert.False(result);
    }

    [Theory]
    [InlineData(30, 95751)] // Under 65
    [InlineData(65, 148218)] // 65-74
    [InlineData(75, 165690)] // 75+
    public void IsTaxPayable_AboveThreshold_ReturnsTrue(int age, decimal aboveThreshold)
    {
        var result = _calculator.IsTaxPayable(aboveThreshold, age);
        Assert.True(result);
    }

    [Theory]
    [InlineData(30, 17235)] // Primary only
    [InlineData(65, 26679)] // Primary + Secondary
    [InlineData(75, 29824)] // Primary + Secondary + Tertiary
    public void CalculateTotalRebates_VariousAges_ReturnsCorrectRebates(int age, decimal expectedRebates)
    {
        var annualIncome = 500000; // High enough to ensure tax is payable
        var grossTax = 117506.69m; // Calculated gross tax for R500,000
        var annualPaye = _calculator.CalculateAnnualPaye(annualIncome, age, 0);
        var expectedPaye = grossTax - expectedRebates;

        Assert.Equal(expectedPaye, annualPaye);
    }

    [Theory]
    [InlineData(1, 364)] // Main member only
    [InlineData(2, 728)] // Main + 1 dependent
    [InlineData(3, 974)] // Main + 2 dependents
    [InlineData(5, 1466)] // Main + 4 dependents
    public void CalculateMonthlyPaye_WithMedicalAid_AppliesCorrectCredits(
        int medicalAidMembers, decimal expectedMonthlyCredit)
    {
        var monthlyIncome = 50000m;
        var age = 35;

        var payeWithoutMedical = _calculator.CalculateMonthlyPaye(monthlyIncome, age, 0);
        var payeWithMedical = _calculator.CalculateMonthlyPaye(monthlyIncome, age, medicalAidMembers);

        var difference = payeWithoutMedical - payeWithMedical;
        Assert.Equal(expectedMonthlyCredit, difference, 2);
    }

    [Fact]
    public void CalculatePayeWithRetirement_AppliesDeduction()
    {
        var annualGross = 600000m;
        var retirementContribution = 165000m; // 27.5% of gross
        var age = 40;

        var payeWithoutRetirement = _calculator.CalculateAnnualPaye(annualGross, age, 0);
        var payeWithRetirement = _calculator.CalculatePayeWithRetirement(
            annualGross, retirementContribution, age, 0);

        Assert.True(payeWithRetirement < payeWithoutRetirement);
    }

    [Fact]
    public void CalculatePayeWithRetirement_CapsAtMaximum()
    {
        var annualGross = 2000000m;
        var retirementContribution = 600000m; // 30% of gross, above cap
        var age = 40;

        var payeWithCappedRetirement = _calculator.CalculatePayeWithRetirement(
            annualGross, 350000m, age, 0); // Max deduction
        var payeWithExcessRetirement = _calculator.CalculatePayeWithRetirement(
            annualGross, retirementContribution, age, 0);

        Assert.Equal(payeWithCappedRetirement, payeWithExcessRetirement);
    }

    [Theory]
    [InlineData(-1000)]
    [InlineData(-1)]
    public void CalculateAnnualPaye_NegativeIncome_ThrowsException(decimal negativeIncome)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            _calculator.CalculateAnnualPaye(negativeIncome, 30, 0));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(151)]
    public void CalculateAnnualPaye_InvalidAge_ThrowsException(int invalidAge)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            _calculator.CalculateAnnualPaye(100000, invalidAge, 0));
        Assert.Contains("between 0 and 150", exception.Message);
    }

    [Fact]
    public void CalculateAnnualPaye_NegativeMedicalAidMembers_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            _calculator.CalculateAnnualPaye(100000, 30, -1));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Theory]
    [InlineData(2023)]
    [InlineData(2024)]
    [InlineData(2025)]
    [InlineData(2026)]
    public void TaxCalculation_ConsistentAcrossYears_SameRatesApply(int year)
    {
        var config = TaxYearData.GetConfiguration(year);
        var calculator = new PayeCalculator(config);

        var monthlyPaye = calculator.CalculateMonthlyPaye(50000, 35, 2);

        // All years have same rates
        Assert.Equal(10574.64m, monthlyPaye, 2);
    }
}
