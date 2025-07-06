using System;
using System.Collections.Generic;
using Xunit;
using SARS.TaxCalculator.Calculators;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Inputs;

namespace SARS.TaxCalculator.Tests;

public class CalculatorEdgeCaseTests
{
    [Fact]
    public void PayeCalculator_NullConfiguration_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new PayeCalculator(null!));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-1000)]
    [InlineData(-0.01)]
    public void PayeCalculator_NegativeTaxableIncome_ThrowsException(decimal negativeIncome)
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        Assert.Throws<ArgumentException>(() => 
            calculator.CalculateAnnualPaye(negativeIncome, 35));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(151)]
    [InlineData(200)]
    public void PayeCalculator_InvalidAge_ThrowsException(int invalidAge)
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        Assert.Throws<ArgumentException>(() => 
            calculator.CalculateAnnualPaye(100000, invalidAge));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public void PayeCalculator_NegativeMedicalAidMembers_ThrowsException(int negativeMembers)
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        Assert.Throws<ArgumentException>(() => 
            calculator.CalculateAnnualPaye(100000, 35, negativeMembers));
    }

    [Fact]
    public void PayeCalculator_ZeroIncome_ReturnsZeroTax()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        var result = calculator.CalculateAnnualPaye(0, 35);
        Assert.Equal(0, result);
    }

    [Fact]
    public void PayeCalculator_IncomeBelowThreshold_ReturnsZeroTax()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        // Test with income below threshold for each age group
        Assert.Equal(0, calculator.CalculateAnnualPaye(50000, 30)); // Below 95,750
        Assert.Equal(0, calculator.CalculateAnnualPaye(100000, 70)); // Below 148,217
        Assert.Equal(0, calculator.CalculateAnnualPaye(150000, 80)); // Below 165,689
    }

    [Fact]
    public void PayeCalculator_ExtremelyHighIncome_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        var result = calculator.CalculateAnnualPaye(100000000, 35); // R100M
        Assert.True(result > 0);
        Assert.True(result < 100000000); // Tax should be less than income
    }

    [Fact]
    public void UifCalculator_NullConfiguration_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new UifCalculator(null!));
    }

    [Fact]
    public void UifCalculator_ZeroIncome_ReturnsZeroContribution()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new UifCalculator(config.UifConfig);

        var result = calculator.CalculateMonthly(0);
        Assert.Equal(0, result.EmployeeAmount);
        Assert.Equal(0, result.EmployerAmount);
        Assert.False(result.CeilingApplied);
    }

    [Fact]
    public void UifCalculator_ExactCeilingIncome_AppliesCeilingCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new UifCalculator(config.UifConfig);

        var result = calculator.CalculateMonthly(17712); // Exact ceiling
        Assert.Equal(177.12m, result.EmployeeAmount);
        Assert.Equal(177.12m, result.EmployerAmount);
        Assert.False(result.CeilingApplied); // At ceiling, not above
    }

    [Theory]
    [InlineData(17713)]
    [InlineData(20000)]
    [InlineData(100000)]
    public void UifCalculator_AboveCeilingIncome_AppliesCeilingCorrectly(decimal incomeAboveCeiling)
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new UifCalculator(config.UifConfig);

        var result = calculator.CalculateMonthly(incomeAboveCeiling);
        Assert.Equal(177.12m, result.EmployeeAmount);
        Assert.Equal(177.12m, result.EmployerAmount);
        Assert.True(result.CeilingApplied);
    }

    [Fact]
    public void SdlCalculator_NullConfiguration_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new SdlCalculator(null!));
    }

    [Fact]
    public void SdlCalculator_BelowExemptionThreshold_ReturnsExemptContribution()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new SdlCalculator(config.SdlConfig);

        var result = calculator.CalculateMonthly(10000, 400000); // Annual payroll below 500k
        Assert.Equal(0, result.Amount);
        Assert.True(result.IsExempt);
    }

    [Fact]
    public void SdlCalculator_ExactExemptionThreshold_ReturnsExemptContribution()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new SdlCalculator(config.SdlConfig);

        var result = calculator.CalculateMonthly(10000, 500000); // Exact threshold
        Assert.Equal(0, result.Amount);
        Assert.True(result.IsExempt);
    }

    [Fact]
    public void SdlCalculator_AboveExemptionThreshold_CalculatesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new SdlCalculator(config.SdlConfig);

        var result = calculator.CalculateMonthly(10000, 500001); // Just above threshold
        Assert.Equal(100m, result.Amount); // 1% of 10,000
        Assert.False(result.IsExempt);
    }

    [Fact]
    public void SdlCalculator_ZeroIncome_ReturnsZeroContribution()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new SdlCalculator(config.SdlConfig);

        var result = calculator.CalculateMonthly(0, 1000000);
        Assert.Equal(0, result.Amount);
        Assert.False(result.IsExempt); // Not exempt due to payroll, but zero due to income
    }

    [Fact]
    public void EtiCalculator_NullConfiguration_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new EtiCalculator(null!));
    }

    [Fact]
    public void EtiCalculator_NullEmployee_ThrowsException()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new EtiCalculator(config.EtiConfig);

        Assert.Throws<ArgumentNullException>(() => calculator.CalculateMonthly(null!));
    }

    [Fact]
    public void EtiCalculator_ExactAgeBoundaries_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new EtiCalculator(config.EtiConfig);

        // Age 17 - just below minimum
        var employee17 = new EtiEmployee { Age = 17, MonthlySalary = 3000, EmploymentMonths = 6, IsFirstTimeEmployee = true };
        var result17 = calculator.CalculateMonthly(employee17);
        Assert.False(result17.IsEligible);
        Assert.Contains("Age 17", result17.IneligibilityReason);

        // Age 18 - exact minimum
        var employee18 = new EtiEmployee { Age = 18, MonthlySalary = 3000, EmploymentMonths = 6, IsFirstTimeEmployee = true };
        var result18 = calculator.CalculateMonthly(employee18);
        Assert.True(result18.IsEligible);

        // Age 29 - exact maximum
        var employee29 = new EtiEmployee { Age = 29, MonthlySalary = 3000, EmploymentMonths = 6, IsFirstTimeEmployee = true };
        var result29 = calculator.CalculateMonthly(employee29);
        Assert.True(result29.IsEligible);

        // Age 30 - just above maximum
        var employee30 = new EtiEmployee { Age = 30, MonthlySalary = 3000, EmploymentMonths = 6, IsFirstTimeEmployee = true };
        var result30 = calculator.CalculateMonthly(employee30);
        Assert.False(result30.IsEligible);
        Assert.Contains("Age 30", result30.IneligibilityReason);
    }

    [Fact]
    public void EtiCalculator_ExactSalaryBoundaries_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new EtiCalculator(config.EtiConfig);

        // Salary at maximum qualifying
        var employeeAtMax = new EtiEmployee { Age = 25, MonthlySalary = 7500, EmploymentMonths = 6, IsFirstTimeEmployee = true };
        var resultAtMax = calculator.CalculateMonthly(employeeAtMax);
        Assert.Equal(0, resultAtMax.Amount); // Top bracket gives 0 ETI

        // Salary above maximum qualifying
        var employeeAboveMax = new EtiEmployee { Age = 25, MonthlySalary = 7501, EmploymentMonths = 6, IsFirstTimeEmployee = true };
        var resultAboveMax = calculator.CalculateMonthly(employeeAboveMax);
        Assert.False(resultAboveMax.IsEligible);
        Assert.Contains("exceeds maximum", resultAboveMax.IneligibilityReason);
    }

    [Fact]
    public void EtiCalculator_EmploymentPeriodBoundaries_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new EtiCalculator(config.EtiConfig);

        // Exactly 24 months - should be ineligible for non-first-time
        var employee24Months = new EtiEmployee 
        { 
            Age = 25, 
            MonthlySalary = 3000, 
            EmploymentMonths = 24, 
            IsFirstTimeEmployee = false 
        };
        var result24 = calculator.CalculateMonthly(employee24Months);
        Assert.False(result24.IsEligible);
        Assert.Contains("ETI period has expired", result24.IneligibilityReason);

        // 23 months - should be eligible for non-first-time
        var employee23Months = new EtiEmployee 
        { 
            Age = 25, 
            MonthlySalary = 3000, 
            EmploymentMonths = 23, 
            IsFirstTimeEmployee = false 
        };
        var result23 = calculator.CalculateMonthly(employee23Months);
        Assert.True(result23.IsEligible);
    }

    [Fact]
    public void EtiCalculator_BulkCalculation_NullList_ThrowsException()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new EtiCalculator(config.EtiConfig);

        Assert.Throws<ArgumentNullException>(() => calculator.CalculateBulk(null!));
    }

    [Fact]
    public void EtiCalculator_BulkCalculation_EmptyList_ReturnsValidResult()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new EtiCalculator(config.EtiConfig);

        var result = calculator.CalculateBulk(new List<EtiEmployee>());
        Assert.Equal(0, result.TotalEmployees);
        Assert.Equal(0, result.EligibleEmployees);
        Assert.Equal(0, result.TotalEtiAmount);
        Assert.Empty(result.IndividualResults);
    }

    [Fact]
    public void PayeCalculator_ExtremeRetirementContribution_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        // Retirement contribution at maximum allowable (27.5% of income)
        var result = calculator.CalculatePayeWithRetirement(600000, 165000, 35, 0);
        var resultWithoutRetirement = calculator.CalculateAnnualPaye(600000, 35, 0);
        Assert.True(result < resultWithoutRetirement); // Should have lower tax with retirement

        // Retirement contribution above maximum (should be capped)
        var result2 = calculator.CalculatePayeWithRetirement(600000, 600000, 35, 0);
        Assert.Equal(result, result2); // Same as maximum allowable since it's capped
    }
}