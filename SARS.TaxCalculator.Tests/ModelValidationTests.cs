using System;
using Xunit;
using SARS.TaxCalculator.Models;

namespace SARS.TaxCalculator.Tests;

public class ModelValidationTests
{
    [Fact]
    public void TaxThreshold_AppliesTo_EdgeCases_ReturnsCorrectResults()
    {
        // Test null MinAge and MaxAge (should apply to all ages)
        var allAgesThreshold = new TaxThreshold { MinAge = null, MaxAge = null, Amount = 95750 };
        Assert.True(allAgesThreshold.AppliesTo(0));
        Assert.True(allAgesThreshold.AppliesTo(25));
        Assert.True(allAgesThreshold.AppliesTo(150));

        // Test only MinAge specified
        var minAgeOnlyThreshold = new TaxThreshold { MinAge = 65, MaxAge = null, Amount = 148217 };
        Assert.False(minAgeOnlyThreshold.AppliesTo(64));
        Assert.True(minAgeOnlyThreshold.AppliesTo(65));
        Assert.True(minAgeOnlyThreshold.AppliesTo(100));

        // Test only MaxAge specified
        var maxAgeOnlyThreshold = new TaxThreshold { MinAge = null, MaxAge = 64, Amount = 95750 };
        Assert.True(maxAgeOnlyThreshold.AppliesTo(0));
        Assert.True(maxAgeOnlyThreshold.AppliesTo(64));
        Assert.False(maxAgeOnlyThreshold.AppliesTo(65));

        // Test exact boundary conditions
        var boundaryThreshold = new TaxThreshold { MinAge = 65, MaxAge = 74, Amount = 148217 };
        Assert.False(boundaryThreshold.AppliesTo(64));
        Assert.True(boundaryThreshold.AppliesTo(65));
        Assert.True(boundaryThreshold.AppliesTo(74));
        Assert.False(boundaryThreshold.AppliesTo(75));
    }

    [Fact]
    public void TaxRebate_QualifiesForRebate_EdgeCases_ReturnsCorrectResults()
    {
        // Test null MinAge (should qualify all ages)
        var allAgesRebate = new TaxRebate { Type = RebateType.Primary, Amount = 17235, MinAge = null };
        Assert.True(allAgesRebate.QualifiesForRebate(0));
        Assert.True(allAgesRebate.QualifiesForRebate(25));
        Assert.True(allAgesRebate.QualifiesForRebate(150));

        // Test exact boundary conditions
        var ageSpecificRebate = new TaxRebate { Type = RebateType.Secondary, Amount = 9444, MinAge = 65 };
        Assert.False(ageSpecificRebate.QualifiesForRebate(64));
        Assert.True(ageSpecificRebate.QualifiesForRebate(65));
        Assert.True(ageSpecificRebate.QualifiesForRebate(100));

        // Test extreme age values
        var extremeAgeRebate = new TaxRebate { Type = RebateType.Tertiary, Amount = 3145, MinAge = 75 };
        Assert.False(extremeAgeRebate.QualifiesForRebate(74));
        Assert.True(extremeAgeRebate.QualifiesForRebate(75));
        Assert.True(extremeAgeRebate.QualifiesForRebate(150));
    }

    [Fact]
    public void MedicalAidCredit_CalculateMonthlyCredit_NegativeDependents_ThrowsException()
    {
        var medicalAid = new MedicalAidCredit
        {
            MainMemberCredit = 364,
            FirstDependentCredit = 364,
            AdditionalDependentCredit = 246
        };

        Assert.Throws<ArgumentException>(() => medicalAid.CalculateMonthlyCredit(-1));
        Assert.Throws<ArgumentException>(() => medicalAid.CalculateMonthlyCredit(-10));
    }

    [Fact]
    public void MedicalAidCredit_CalculateMonthlyCredit_EdgeCases_ReturnsCorrectAmounts()
    {
        var medicalAid = new MedicalAidCredit
        {
            MainMemberCredit = 364,
            FirstDependentCredit = 364,
            AdditionalDependentCredit = 246
        };

        // Test zero dependents (main member only)
        Assert.Equal(364m, medicalAid.CalculateMonthlyCredit(0));

        // Test one dependent
        Assert.Equal(728m, medicalAid.CalculateMonthlyCredit(1)); // 364 + 364

        // Test two dependents
        Assert.Equal(974m, medicalAid.CalculateMonthlyCredit(2)); // 364 + 364 + 246

        // Test many dependents
        Assert.Equal(1466m, medicalAid.CalculateMonthlyCredit(4)); // 364 + 364 + 246 + 246 + 246

        // Test extreme number of dependents
        Assert.Equal(4910m, medicalAid.CalculateMonthlyCredit(18)); // 364 + 364 + (246 * 17)
    }

    [Fact]
    public void MedicalAidCredit_CalculateAnnualCredit_ReturnsCorrectAmounts()
    {
        var medicalAid = new MedicalAidCredit
        {
            MainMemberCredit = 364,
            FirstDependentCredit = 364,
            AdditionalDependentCredit = 246
        };

        // Annual should be 12 times monthly
        Assert.Equal(4368m, medicalAid.CalculateAnnualCredit(0)); // 364 * 12
        Assert.Equal(8736m, medicalAid.CalculateAnnualCredit(1)); // 728 * 12
        Assert.Equal(11688m, medicalAid.CalculateAnnualCredit(2)); // 974 * 12
    }

    [Fact]
    public void TaxBracket_CalculateTax_IncomeBelowMinimum_ReturnsZero()
    {
        var bracket = new TaxBracket
        {
            MinIncome = 237101,
            MaxIncome = 370500,
            BaseTax = 42678,
            Rate = 26
        };

        Assert.Equal(0m, bracket.CalculateTax(0));
        Assert.Equal(0m, bracket.CalculateTax(100000));
        Assert.Equal(0m, bracket.CalculateTax(237100));
    }

    [Fact]
    public void TaxBracket_CalculateTax_ExactBoundaries_ReturnsCorrectTax()
    {
        var bracket = new TaxBracket
        {
            MinIncome = 237101,
            MaxIncome = 370500,
            BaseTax = 42678,
            Rate = 26
        };

        // Exact minimum - should return base tax only
        Assert.Equal(42678m, bracket.CalculateTax(237101));

        // Exact maximum - should return base tax + full bracket tax
        var expectedMax = 42678m + ((370500m - 237101m) * 26m / 100m);
        Assert.Equal(expectedMax, bracket.CalculateTax(370500));

        // Above maximum - should be capped at bracket max
        Assert.Equal(expectedMax, bracket.CalculateTax(500000));
    }

    [Fact]
    public void TaxBracket_CalculateTax_TopBracketNoMaximum_CalculatesCorrectly()
    {
        var topBracket = new TaxBracket
        {
            MinIncome = 1817001,
            MaxIncome = null,
            BaseTax = 644489,
            Rate = 45
        };

        // Test at minimum
        Assert.Equal(644489m, topBracket.CalculateTax(1817001));

        // Test well above minimum
        var expectedHigh = 644489m + ((2000000m - 1817001m) * 45m / 100m);
        Assert.Equal(expectedHigh, topBracket.CalculateTax(2000000));

        // Test very high income
        var expectedVeryHigh = 644489m + ((10000000m - 1817001m) * 45m / 100m);
        Assert.Equal(expectedVeryHigh, topBracket.CalculateTax(10000000));
    }

    [Fact]
    public void TaxBracket_CalculateTax_FirstBracket_NoBaseTax()
    {
        var firstBracket = new TaxBracket
        {
            MinIncome = 0,
            MaxIncome = 237100,
            BaseTax = 0,
            Rate = 18
        };

        // Test at zero
        Assert.Equal(0m, firstBracket.CalculateTax(0));

        // Test mid-bracket
        var expectedMid = 0m + (100000m * 18m / 100m);
        Assert.Equal(expectedMid, firstBracket.CalculateTax(100000));

        // Test at maximum
        var expectedMax = 0m + (237100m * 18m / 100m);
        Assert.Equal(expectedMax, firstBracket.CalculateTax(237100));

        // Test above maximum
        Assert.Equal(expectedMax, firstBracket.CalculateTax(300000));
    }
}
