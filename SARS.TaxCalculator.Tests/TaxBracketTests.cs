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
        
        // Income of 300,000: BaseTax + (300,000 - 237,101) * 26%
        // 42,678 + 62,899 * 0.26 = 42,678 + 16,353.74 = 59,031.74
        var result = bracket.CalculateTax(300000);
        Assert.Equal(59031.74m, result);
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
        // BaseTax + (370,500 - 237,101) * 26%
        // 42,678 + 133,399 * 0.26 = 42,678 + 34,683.74 = 77,361.74
        var result = bracket.CalculateTax(500000);
        Assert.Equal(77361.74m, result);
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
        
        // Income of 2,000,000: BaseTax + (2,000,000 - 1,817,001) * 45%
        // 644,489 + 182,999 * 0.45 = 644,489 + 82,349.55 = 726,838.55
        var result = bracket.CalculateTax(2000000);
        Assert.Equal(726838.55m, result);
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
    public void CalculateTax_ExactlyAtMinimum_ReturnsBaseTax()
    {
        var bracket = new TaxBracket
        {
            MinIncome = 237101,
            MaxIncome = 370500,
            BaseTax = 42678,
            Rate = 26
        };
        
        var result = bracket.CalculateTax(237101);
        Assert.Equal(42678, result);
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
        
        // BaseTax + (370,500 - 237,101) * 26%
        var result = bracket.CalculateTax(370500);
        Assert.Equal(77361.74m, result);
    }
}