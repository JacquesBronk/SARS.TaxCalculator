using System;

namespace SARS.TaxCalculator.Utilities;

/// <summary>
/// SARS-compliant rounding utilities
/// Source: SARS validation rules and reconciliation guidelines
/// Reference: https://www.sars.gov.za/guide-for-validation-rules-applicable-to-reconciliation-declarations-2025/
/// </summary>
public static class SarsRounding
{
    /// <summary>
    /// Rounds a decimal value according to SARS rounding rules for currency amounts.
    /// SARS requires rounding to 2 decimal places for intermediate calculations,
    /// and final tax amounts are typically rounded to the nearest cent.
    /// </summary>
    /// <param name="value">The value to round</param>
    /// <returns>The rounded value</returns>
    public static decimal RoundCurrency(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Rounds a decimal value to the nearest rand according to SARS requirements.
    /// Used for final tax amounts that must be expressed in whole rands.
    /// </summary>
    /// <param name="value">The value to round</param>
    /// <returns>The rounded value</returns>
    public static decimal RoundToRand(decimal value)
    {
        return Math.Round(value, 0, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Rounds PAYE calculation specifically according to SARS guidelines.
    /// PAYE calculations use standard currency rounding to the nearest cent.
    /// Source: Fourth Schedule to the Income Tax Act
    /// Reference: SARS validation rules specify cents must be included for tax amounts
    /// </summary>
    /// <param name="value">The PAYE value to round</param>
    /// <returns>The rounded PAYE value</returns>
    public static decimal RoundPaye(decimal value)
    {
        // PAYE calculations use standard currency rounding per validation rules
        return RoundCurrency(value);
    }

    /// <summary>
    /// Rounds UIF contribution according to SARS guidelines.
    /// UIF is calculated as 1% of remuneration up to ceiling and rounded to the nearest cent.
    /// Source: SARS validation rules specify cents must be included for UIF amounts
    /// Reference: Unemployment Insurance Act and SARS reconciliation rules
    /// </summary>
    /// <param name="value">The UIF value to round</param>
    /// <returns>The rounded UIF value</returns>
    public static decimal RoundUif(decimal value)
    {
        return RoundCurrency(value);
    }

    /// <summary>
    /// Rounds SDL contribution according to SARS guidelines.
    /// SDL is calculated as 1% of annual payroll above threshold and rounded to the nearest cent.
    /// Source: Skills Development Act and SARS validation rules
    /// Reference: SARS validation rules specify cents must be included for SDL amounts
    /// </summary>
    /// <param name="value">The SDL value to round</param>
    /// <returns>The rounded SDL value</returns>
    public static decimal RoundSdl(decimal value)
    {
        return RoundCurrency(value);
    }

    /// <summary>
    /// Rounds ETI amount according to SARS guidelines.
    /// ETI amounts are calculated according to specific bands and cents are dropped (truncated).
    /// Final ETI amounts on SARS forms must be in whole rands without cents.
    /// </summary>
    /// <param name="value">The ETI value to round</param>
    /// <returns>The rounded ETI value</returns>
    public static decimal RoundEti(decimal value)
    {
        // SARS validation rule: "All cents for Rands must be dropped off" (truncated to whole Rands)
        // Exception: Tax, SDL and UIF amounts require cents - ETI is NOT in this exception list
        // Source: SARS Guide for Validation Rules Applicable to Reconciliation Declarations 2025
        // Reference: https://www.sars.gov.za/guide-for-validation-rules-applicable-to-reconciliation-declarations-2025/
        // ETI amounts must be in whole Rands when submitted on reconciliation forms (EMP201/EMP501)
        return Math.Truncate(value);
    }
}
