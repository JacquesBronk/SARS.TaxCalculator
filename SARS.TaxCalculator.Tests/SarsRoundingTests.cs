using System;
using Xunit;
using SARS.TaxCalculator.Utilities;

namespace SARS.TaxCalculator.Tests;

public class SarsRoundingTests
{
    [Theory]
    [InlineData(123.456, 123.46)]
    [InlineData(123.454, 123.45)]
    [InlineData(123.455, 123.46)] // MidpointRounding.AwayFromZero
    [InlineData(123.465, 123.47)]
    public void RoundCurrency_VariousValues_RoundsToTwoDecimalPlaces(decimal input, decimal expected)
    {
        var result = SarsRounding.RoundCurrency(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(123.4, 123)]
    [InlineData(123.5, 124)] // MidpointRounding.AwayFromZero
    [InlineData(123.6, 124)]
    [InlineData(123.49, 123)]
    public void RoundToRand_VariousValues_RoundsToWholeNumber(decimal input, decimal expected)
    {
        var result = SarsRounding.RoundToRand(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RoundPaye_UsesCorrectRounding()
    {
        var result = SarsRounding.RoundPaye(123.455m);
        Assert.Equal(123.46m, result);
    }

    [Fact]
    public void RoundUif_UsesCorrectRounding()
    {
        var result = SarsRounding.RoundUif(177.125m);
        Assert.Equal(177.13m, result);
    }

    [Fact]
    public void RoundSdl_UsesCorrectRounding()
    {
        var result = SarsRounding.RoundSdl(300.125m);
        Assert.Equal(300.13m, result);
    }

    [Fact]
    public void RoundEti_UsesCorrectRounding()
    {
        var result = SarsRounding.RoundEti(1000.505m);
        Assert.Equal(1000m, result); // ETI truncates cents per SARS rules
    }

    // === ETI truncation edge cases ===

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0.01, 0)]
    [InlineData(0.99, 0)]
    [InlineData(1.00, 1)]
    [InlineData(999.99, 999)]
    [InlineData(1500.50, 1500)]
    [InlineData(1499.999, 1499)]
    public void RoundEti_Truncation_AlwaysDropsCents(decimal input, decimal expected)
    {
        var result = SarsRounding.RoundEti(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void RoundEti_NearWholeNumber_TruncatesNotRounds()
    {
        // R1,499.99 should truncate to R1,499 (not round up to R1,500)
        Assert.Equal(1499m, SarsRounding.RoundEti(1499.99m));
        Assert.Equal(1499m, SarsRounding.RoundEti(1499.50m));
        Assert.Equal(1499m, SarsRounding.RoundEti(1499.01m));
    }

    // === Currency rounding (midpoint AwayFromZero) edge cases ===

    [Theory]
    [InlineData(0.005, 0.01)]   // Midpoint rounds up (AwayFromZero)
    [InlineData(0.004, 0.00)]   // Below midpoint rounds down
    [InlineData(0.015, 0.02)]   // Midpoint rounds up
    [InlineData(0.025, 0.03)]   // Midpoint rounds up (away from zero)
    [InlineData(0.00, 0.00)]    // Zero stays zero
    [InlineData(100.00, 100.00)] // Exact value unchanged
    public void RoundCurrency_MidpointBehavior_RoundsAwayFromZero(decimal input, decimal expected)
    {
        Assert.Equal(expected, SarsRounding.RoundCurrency(input));
    }

    [Theory]
    [InlineData(177.115, 177.12)]  // UIF ceiling fractional rounding
    [InlineData(177.125, 177.13)]  // Midpoint
    [InlineData(250.005, 250.01)]  // Typical PAYE fractional
    [InlineData(250.004, 250.00)]  // Just below midpoint
    public void RoundPaye_FractionalCents_RoundsCorrectly(decimal input, decimal expected)
    {
        Assert.Equal(expected, SarsRounding.RoundPaye(input));
    }

    // === Whole rand rounding edge cases ===

    [Theory]
    [InlineData(0.49, 0)]
    [InlineData(0.50, 1)]     // Midpoint rounds up (AwayFromZero)
    [InlineData(0.51, 1)]
    [InlineData(95750.50, 95751)] // Tax threshold boundary
    [InlineData(95750.49, 95750)]
    public void RoundToRand_MidpointBehavior_RoundsAwayFromZero(decimal input, decimal expected)
    {
        Assert.Equal(expected, SarsRounding.RoundToRand(input));
    }

    // === Rounding consistency across methods ===

    [Fact]
    public void AllRoundingMethods_ZeroInput_ReturnsZero()
    {
        Assert.Equal(0m, SarsRounding.RoundCurrency(0));
        Assert.Equal(0m, SarsRounding.RoundToRand(0));
        Assert.Equal(0m, SarsRounding.RoundPaye(0));
        Assert.Equal(0m, SarsRounding.RoundUif(0));
        Assert.Equal(0m, SarsRounding.RoundSdl(0));
        Assert.Equal(0m, SarsRounding.RoundEti(0));
    }

    [Fact]
    public void RoundEti_VsRoundCurrency_DifferentBehavior()
    {
        // ETI truncates; currency rounds. They should differ for fractional amounts.
        var amount = 1000.99m;
        var etiResult = SarsRounding.RoundEti(amount);
        var currencyResult = SarsRounding.RoundCurrency(amount);

        Assert.Equal(1000m, etiResult);      // Truncated
        Assert.Equal(1000.99m, currencyResult); // Rounded to cents
    }
}
