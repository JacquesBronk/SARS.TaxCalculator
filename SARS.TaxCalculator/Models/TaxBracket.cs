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

        // SARS formula: BaseTax + Rate% of taxable income above previous bracket's ceiling
        // For bracket 1 (MinIncome=0): above R0 = taxableIncome
        // For bracket 2+ (MinIncome=N): above R(N-1) = taxableIncome - (N-1)
        var bracketFloor = MinIncome > 0 ? MinIncome - 1 : 0;
        var taxableAmountInBracket = MaxIncome.HasValue
            ? Math.Min(taxableIncome - bracketFloor, MaxIncome.Value - bracketFloor)
            : taxableIncome - bracketFloor;

        return BaseTax + (taxableAmountInBracket * Rate / 100);
    }
}
