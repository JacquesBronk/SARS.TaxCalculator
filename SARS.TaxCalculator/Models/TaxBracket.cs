using System;

namespace SARS.TaxCalculator.Models;

/// <summary>
/// Represents a tax bracket with its income range and calculation parameters
/// </summary>
public class TaxBracket
{
    /// <summary>
    /// The minimum income for this tax bracket (inclusive)
    /// </summary>
    public decimal MinIncome { get; init; }
    
    /// <summary>
    /// The maximum income for this tax bracket (inclusive, null for unlimited)
    /// </summary>
    public decimal? MaxIncome { get; init; }
    
    /// <summary>
    /// The base tax amount for this bracket
    /// </summary>
    public decimal BaseTax { get; init; }
    
    /// <summary>
    /// The tax rate percentage for income above MinIncome
    /// </summary>
    public decimal Rate { get; init; }

    /// <summary>
    /// Calculates the tax for a given taxable income using this bracket
    /// </summary>
    /// <param name="taxableIncome">The taxable income</param>
    /// <returns>The calculated tax amount</returns>
    public decimal CalculateTax(decimal taxableIncome)
    {
        if (taxableIncome < MinIncome)
            return 0;
            
        var taxableAmountInBracket = MaxIncome.HasValue 
            ? Math.Min(taxableIncome - MinIncome, MaxIncome.Value - MinIncome)
            : taxableIncome - MinIncome;
            
        return BaseTax + (taxableAmountInBracket * Rate / 100);
    }
}