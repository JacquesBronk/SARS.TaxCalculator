using System;

namespace SARS.TaxCalculator.Models;

/// <summary>
/// Represents medical aid tax credit configuration
/// </summary>
public class MedicalAidCredit
{
    /// <summary>
    /// Monthly credit amount for the main member
    /// </summary>
    public decimal MainMemberCredit { get; init; }

    /// <summary>
    /// Monthly credit amount for the first dependent
    /// </summary>
    public decimal FirstDependentCredit { get; init; }

    /// <summary>
    /// Monthly credit amount for each additional dependent
    /// </summary>
    public decimal AdditionalDependentCredit { get; init; }

    /// <summary>
    /// Calculates the total monthly medical aid credit
    /// </summary>
    /// <param name="numberOfDependents">The number of dependents on the medical aid</param>
    /// <returns>The total monthly credit amount</returns>
    public decimal CalculateMonthlyCredit(int numberOfDependents)
    {
        if (numberOfDependents < 0)
            throw new ArgumentException("Number of dependents cannot be negative", nameof(numberOfDependents));

        if (numberOfDependents == 0)
            return MainMemberCredit;

        if (numberOfDependents == 1)
            return MainMemberCredit + FirstDependentCredit;

        return MainMemberCredit + FirstDependentCredit +
               (AdditionalDependentCredit * (numberOfDependents - 1));
    }

    /// <summary>
    /// Calculates the annual medical aid credit
    /// </summary>
    /// <param name="numberOfDependents">The number of dependents on the medical aid</param>
    /// <returns>The total annual credit amount</returns>
    public decimal CalculateAnnualCredit(int numberOfDependents)
    {
        return CalculateMonthlyCredit(numberOfDependents) * 12;
    }
}
