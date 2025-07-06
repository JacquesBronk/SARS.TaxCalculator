using System;
using Xunit;
using SARS.TaxCalculator.Calculators;
using SARS.TaxCalculator.Configuration;

namespace SARS.TaxCalculator.Tests;

public class UifCalculatorTests
{
    private readonly UifCalculator _calculator;
    private readonly UifConfiguration _config;

    public UifCalculatorTests()
    {
        _config = new UifConfiguration
        {
            EmployeeRate = 0.01m,
            EmployerRate = 0.01m,
            MonthlyCeiling = 17712m
        };
        _calculator = new UifCalculator(_config);
    }

    [Theory]
    [InlineData(5000, 50, 50, 100)]
    [InlineData(10000, 100, 100, 200)]
    [InlineData(17712, 177.12, 177.12, 354.24)] // At ceiling
    [InlineData(20000, 177.12, 177.12, 354.24)] // Above ceiling
    [InlineData(50000, 177.12, 177.12, 354.24)] // Well above ceiling
    public void CalculateMonthly_VariousIncomes_ReturnsCorrectContributions(
        decimal monthlyIncome, decimal expectedEmployee, decimal expectedEmployer, decimal expectedTotal)
    {
        var result = _calculator.CalculateMonthly(monthlyIncome);

        Assert.Equal(expectedEmployee, result.EmployeeAmount);
        Assert.Equal(expectedEmployer, result.EmployerAmount);
        Assert.Equal(expectedTotal, result.TotalAmount);
        Assert.Equal(Math.Min(monthlyIncome, 17712m), result.ContributionBase);
    }

    [Theory]
    [InlineData(10000, false)]
    [InlineData(17712, false)]
    [InlineData(17713, true)]
    [InlineData(25000, true)]
    public void CalculateMonthly_CeilingApplication_SetsCorrectFlag(
        decimal monthlyIncome, bool expectedCeilingApplied)
    {
        var result = _calculator.CalculateMonthly(monthlyIncome);
        Assert.Equal(expectedCeilingApplied, result.CeilingApplied);
    }

    [Theory]
    [InlineData(100000, 1000, 1000, 2000)]
    [InlineData(212544, 2125.44, 2125.44, 4250.88)] // At annual ceiling
    [InlineData(300000, 2125.44, 2125.44, 4250.88)] // Above annual ceiling
    public void CalculateAnnual_VariousIncomes_ReturnsCorrectContributions(
        decimal annualIncome, decimal expectedEmployee, decimal expectedEmployer, decimal expectedTotal)
    {
        var result = _calculator.CalculateAnnual(annualIncome);

        Assert.Equal(expectedEmployee, result.EmployeeAmount);
        Assert.Equal(expectedEmployer, result.EmployerAmount);
        Assert.Equal(expectedTotal, result.TotalAmount);
    }

    [Fact]
    public void ExceedsCeiling_BelowCeiling_ReturnsFalse()
    {
        var result = _calculator.ExceedsCeiling(15000);
        Assert.False(result);
    }

    [Fact]
    public void ExceedsCeiling_AboveCeiling_ReturnsTrue()
    {
        var result = _calculator.ExceedsCeiling(20000);
        Assert.True(result);
    }

    [Fact]
    public void GetMaximumMonthlyContribution_ReturnsCorrectAmount()
    {
        var result = _calculator.GetMaximumMonthlyContribution();

        Assert.Equal(177.12m, result.EmployeeAmount);
        Assert.Equal(177.12m, result.EmployerAmount);
        Assert.Equal(354.24m, result.TotalAmount);
        Assert.Equal(17712m, result.ContributionBase);
        Assert.False(result.CeilingApplied);
    }

    [Fact]
    public void CalculateMonthly_NegativeIncome_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            _calculator.CalculateMonthly(-1000));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Fact]
    public void CalculateAnnual_NegativeIncome_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            _calculator.CalculateAnnual(-10000));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1000.50, 10.01, 10.01)]
    [InlineData(5555.55, 55.56, 55.56)]
    public void CalculateMonthly_RoundsToTwoCents(decimal income, decimal expectedEmployee, decimal expectedEmployer)
    {
        var result = _calculator.CalculateMonthly(income);

        Assert.Equal(expectedEmployee, result.EmployeeAmount);
        Assert.Equal(expectedEmployer, result.EmployerAmount);
    }

    [Fact]
    public void Constructor_NullConfig_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new UifCalculator(null!));
    }
}
