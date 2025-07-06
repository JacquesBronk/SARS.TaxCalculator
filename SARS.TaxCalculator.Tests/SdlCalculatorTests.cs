using System;
using System.Linq;
using Xunit;
using SARS.TaxCalculator.Calculators;
using SARS.TaxCalculator.Configuration;

namespace SARS.TaxCalculator.Tests;

public class SdlCalculatorTests
{
    private readonly SdlCalculator _calculator;
    private readonly SdlConfiguration _config;

    public SdlCalculatorTests()
    {
        _config = new SdlConfiguration
        {
            Rate = 0.01m,
            ExemptionThreshold = 500000m
        };
        _calculator = new SdlCalculator(_config);
    }

    [Theory]
    [InlineData(10000, 400000, 0)] // Below threshold
    [InlineData(10000, 500000, 0)] // At threshold
    [InlineData(10000, 600000, 100)] // Above threshold
    [InlineData(50000, 1000000, 500)] // High salary, above threshold
    public void CalculateMonthly_VariousPayrolls_ReturnsCorrectSdl(
        decimal monthlyIncome, decimal annualPayroll, decimal expectedSdl)
    {
        var result = _calculator.CalculateMonthly(monthlyIncome, annualPayroll);
        
        Assert.Equal(expectedSdl, result.Amount);
        Assert.Equal(annualPayroll <= 500000, result.IsExempt);
    }

    [Theory]
    [InlineData(120000, 400000, 0)]
    [InlineData(120000, 600000, 1200)]
    [InlineData(600000, 1000000, 6000)]
    public void CalculateAnnual_VariousPayrolls_ReturnsCorrectSdl(
        decimal annualIncome, decimal annualPayroll, decimal expectedSdl)
    {
        var result = _calculator.CalculateAnnual(annualIncome, annualPayroll);
        
        Assert.Equal(expectedSdl, result.Amount);
    }

    [Theory]
    [InlineData(400000, 0)]
    [InlineData(500000, 0)]
    [InlineData(500001, 5000.01)]
    [InlineData(1000000, 10000)]
    public void CalculateTotalSdl_CompanyPayroll_ReturnsCorrectTotal(
        decimal annualPayroll, decimal expectedTotal)
    {
        var result = _calculator.CalculateTotalSdl(annualPayroll);
        Assert.Equal(expectedTotal, result);
    }

    [Theory]
    [InlineData(400000, true)]
    [InlineData(500000, true)]
    [InlineData(500001, false)]
    public void IsExempt_VariousPayrolls_ReturnsCorrectStatus(
        decimal annualPayroll, bool expectedExempt)
    {
        var result = _calculator.IsExempt(annualPayroll);
        Assert.Equal(expectedExempt, result);
    }

    [Fact]
    public void CalculateBulk_MultipleEmployees_ReturnsCorrectTotals()
    {
        var salaries = new[] { 120000m, 240000m, 360000m, 480000m }; // Total: 1,200,000
        
        var result = _calculator.CalculateBulk(salaries);
        
        Assert.Equal(1200000, result.TotalPayroll);
        Assert.Equal(12000, result.TotalSdl); // 1% of total
        Assert.False(result.IsExempt);
        Assert.Equal(4, result.EmployeeCount);
        Assert.Equal(4, result.IndividualContributions.Count);
        
        // Verify individual contributions
        Assert.Equal(1200, result.IndividualContributions[0].SdlAmount);
        Assert.Equal(2400, result.IndividualContributions[1].SdlAmount);
    }

    [Fact]
    public void CalculateBulk_BelowThreshold_AllExempt()
    {
        var salaries = new[] { 100000m, 150000m, 200000m }; // Total: 450,000
        
        var result = _calculator.CalculateBulk(salaries);
        
        Assert.Equal(450000, result.TotalPayroll);
        Assert.Equal(0, result.TotalSdl);
        Assert.True(result.IsExempt);
        Assert.All(result.IndividualContributions, c => Assert.Equal(0, c.SdlAmount));
    }

    [Fact]
    public void CalculateMonthly_NegativeIncome_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() => 
            _calculator.CalculateMonthly(-1000, 1000000));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Fact]
    public void CalculateMonthly_NegativePayroll_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() => 
            _calculator.CalculateMonthly(10000, -1000000));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Fact]
    public void CalculateBulk_NullList_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => _calculator.CalculateBulk(null!));
    }

    [Fact]
    public void CalculateBulk_NegativeSalary_ThrowsException()
    {
        var salaries = new[] { 100000m, -50000m, 200000m };
        
        var exception = Assert.Throws<ArgumentException>(() => 
            _calculator.CalculateBulk(salaries));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Fact]
    public void Constructor_NullConfig_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new SdlCalculator(null!));
    }
}