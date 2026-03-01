using System;
using Xunit;
using SARS.TaxCalculator.Models;

namespace SARS.TaxCalculator.Tests;

public class TaxBracketTests
{
    [Fact]
    public void CalculateTax_IncomeBelowMinimum_ReturnsZero()
    {
        var bracket = new TaxBracket
        {
            MinIncome = 237101,
            MaxIncome = 370500,
            BaseTax = 42678,
            Rate = 26
        };

        var result = bracket.CalculateTax(200000);
        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateTax_IncomeInBracket_ReturnsCorrectTax()
    {
        var bracket = new TaxBracket
        {
            MinIncome = 237101,
            MaxIncome = 370500,
            BaseTax = 42678,
            Rate = 26
        };

        // SARS formula: BaseTax + (300,000 - 237,100) * 26%
        // 42,678 + 62,900 * 0.26 = 42,678 + 16,354 = 59,032
        var result = bracket.CalculateTax(300000);
        Assert.Equal(59032m, result);
    }

    [Fact]
    public void CalculateTax_IncomeAboveBracket_CapsAtMaximum()
    {
        var bracket = new TaxBracket
        {
            MinIncome = 237101,
            MaxIncome = 370500,
            BaseTax = 42678,
            Rate = 26
        };

        // Should calculate only up to MaxIncome
        // SARS formula: BaseTax + (370,500 - 237,100) * 26%
        // 42,678 + 133,400 * 0.26 = 42,678 + 34,684 = 77,362
        var result = bracket.CalculateTax(500000);
        Assert.Equal(77362m, result);
    }

    [Fact]
    public void CalculateTax_TopBracketNoMaximum_CalculatesCorrectly()
    {
        var bracket = new TaxBracket
        {
            MinIncome = 1817001,
            MaxIncome = null,
            BaseTax = 644489,
            Rate = 45
        };

        // SARS formula: BaseTax + (2,000,000 - 1,817,000) * 45%
        // 644,489 + 183,000 * 0.45 = 644,489 + 82,350 = 726,839
        var result = bracket.CalculateTax(2000000);
        Assert.Equal(726839m, result);
    }

    [Fact]
    public void CalculateTax_FirstBracket_NoBaseTax()
    {
        var bracket = new TaxBracket
        {
            MinIncome = 0,
            MaxIncome = 237100,
            BaseTax = 0,
            Rate = 18
        };

        // Income of 100,000: 0 + 100,000 * 18% = 18,000
        var result = bracket.CalculateTax(100000);
        Assert.Equal(18000, result);
    }

    [Fact]
    public void CalculateTax_ExactlyAtMinimum_ReturnsBaseTaxPlusOneRandAtRate()
    {
        var bracket = new TaxBracket
        {
            MinIncome = 237101,
            MaxIncome = 370500,
            BaseTax = 42678,
            Rate = 26
        };

        // At MinIncome, SARS taxes the R1 above previous bracket's max at the new rate
        // 42,678 + 26% of R1 = 42,678.26
        var result = bracket.CalculateTax(237101);
        Assert.Equal(42678.26m, result);
    }

    [Fact]
    public void CalculateTax_ExactlyAtMaximum_ReturnsFullBracketTax()
    {
        var bracket = new TaxBracket
        {
            MinIncome = 237101,
            MaxIncome = 370500,
            BaseTax = 42678,
            Rate = 26
        };

        // SARS formula: BaseTax + (370,500 - 237,100) * 26% = 42,678 + 34,684 = 77,362
        // This equals the next bracket's BaseTax, ensuring continuity
        var result = bracket.CalculateTax(370500);
        Assert.Equal(77362m, result);
    }
}
