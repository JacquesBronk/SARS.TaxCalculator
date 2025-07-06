using System.Collections.Generic;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Inputs;

namespace SARS.TaxCalculator.Models.Results;

/// <summary>
/// ETI eligibility check result
/// </summary>
public class EtiEligibility
{
    /// <summary>
    /// Whether the employee is eligible
    /// </summary>
    public bool IsEligible { get; init; }

    /// <summary>
    /// Reason for ineligibility (if applicable)
    /// </summary>
    public string? Reason { get; init; }
}

/// <summary>
/// ETI calculation result
/// </summary>
public class EtiCalculationResult
{
    /// <summary>
    /// Monthly ETI amount
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Whether the employee is eligible
    /// </summary>
    public bool IsEligible { get; init; }

    /// <summary>
    /// Reason for ineligibility (if applicable)
    /// </summary>
    public string? IneligibilityReason { get; init; }

    /// <summary>
    /// Employee details
    /// </summary>
    public EtiEmployee Employee { get; init; } = new();

    /// <summary>
    /// Applied ETI band
    /// </summary>
    public EtiBand? Band { get; init; }
}

/// <summary>
/// Bulk ETI calculation results
/// </summary>
public class EtiBulkResult
{
    /// <summary>
    /// Total number of employees
    /// </summary>
    public int TotalEmployees { get; init; }

    /// <summary>
    /// Number of eligible employees
    /// </summary>
    public int EligibleEmployees { get; init; }

    /// <summary>
    /// Total ETI amount
    /// </summary>
    public decimal TotalEtiAmount { get; init; }

    /// <summary>
    /// Individual calculation results
    /// </summary>
    public IReadOnlyList<EtiCalculationResult> IndividualResults { get; init; } = new List<EtiCalculationResult>();
}
