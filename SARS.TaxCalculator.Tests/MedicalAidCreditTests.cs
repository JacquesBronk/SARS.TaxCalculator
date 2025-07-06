using System;
using Xunit;
using SARS.TaxCalculator.Models;

namespace SARS.TaxCalculator.Tests;

public class MedicalAidCreditTests
{
    private readonly MedicalAidCredit _credit;

    public MedicalAidCreditTests()
    {
        _credit = new MedicalAidCredit
        {
            MainMemberCredit = 364,
            FirstDependentCredit = 364,
            AdditionalDependentCredit = 246
        };
    }

    [Theory]
    [InlineData(0, 364)]    // Main member only
    [InlineData(1, 728)]    // Main + 1 dependent
    [InlineData(2, 974)]    // Main + 2 dependents
    [InlineData(3, 1220)]   // Main + 3 dependents
    [InlineData(5, 1712)]   // Main + 5 dependents
    public void CalculateMonthlyCredit_VariousDependents_ReturnsCorrectCredit(
        int numberOfDependents, decimal expectedCredit)
    {
        var result = _credit.CalculateMonthlyCredit(numberOfDependents);
        Assert.Equal(expectedCredit, result);
    }

    [Theory]
    [InlineData(0, 4368)]   // 364 * 12
    [InlineData(1, 8736)]   // 728 * 12
    [InlineData(2, 11688)]  // 974 * 12
    public void CalculateAnnualCredit_VariousDependents_ReturnsCorrectCredit(
        int numberOfDependents, decimal expectedCredit)
    {
        var result = _credit.CalculateAnnualCredit(numberOfDependents);
        Assert.Equal(expectedCredit, result);
    }

    [Fact]
    public void CalculateMonthlyCredit_NegativeDependents_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() => 
            _credit.CalculateMonthlyCredit(-1));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Fact]
    public void CalculateAnnualCredit_NegativeDependents_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() => 
            _credit.CalculateAnnualCredit(-1));
        Assert.Contains("cannot be negative", exception.Message);
    }

    [Fact]
    public void CalculateMonthlyCredit_LargeFamilies_CalculatesCorrectly()
    {
        // Test edge case of large families
        var result = _credit.CalculateMonthlyCredit(10);
        // Main (364) + First (364) + 9 additional (246 * 9 = 2214) = 2942
        Assert.Equal(2942, result);
    }
}