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
}