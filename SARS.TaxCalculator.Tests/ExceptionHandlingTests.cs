using System;
using System.Collections.Generic;
using Xunit;
using SARS.TaxCalculator.Calculators;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Inputs;

namespace SARS.TaxCalculator.Tests;

public class ExceptionHandlingTests
{
    [Fact]
    public void PayslipCalculator_NullConfiguration_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new PayslipCalculator(null!));
    }

    [Fact]
    public void PayslipCalculator_NullInput_ThrowsArgumentNullException()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayslipCalculator(config);

        Assert.Throws<ArgumentNullException>(() => calculator.Calculate(null!));
    }

    [Fact]
    public void TaxCalculator_InvalidMedicalAidMembers_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2024).WithMedicalAid(-1));
        
        Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2024).WithMedicalAid(0));
    }

    [Fact]
    public void TaxCalculator_InvalidRetirementPercentage_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2024).WithRetirementContribution(-0.1m));
        
        Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2024).WithRetirementContribution(1.1m));
    }

    [Fact]
    public void TaxCalculator_InvalidRetirementAmount_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2024).WithRetirementContributionAmount(-1000));
    }

    [Fact]
    public void TaxCalculator_InvalidCompanyPayroll_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2024).WithCompanyPayroll(-100000));
    }

    [Fact]
    public void TaxCalculator_InvalidAge_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2024).WithAge(-1));
        
        Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2024).WithAge(151));
    }

    [Fact]
    public void TaxCalculator_UnsupportedTaxYear_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => TaxCalculator.ForTaxYear(2020));
        Assert.Throws<ArgumentException>(() => TaxCalculator.ForTaxYear(2030));
        Assert.Throws<ArgumentException>(() => TaxCalculator.ForTaxYear(1990));
    }

    [Fact]
    public void EtiCalculator_NullEmployeeList_ThrowsArgumentNullException()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new EtiCalculator(config.EtiConfig);

        Assert.Throws<ArgumentNullException>(() => calculator.CalculateBulk(null!));
    }

    [Fact]
    public void SdlCalculator_BulkCalculation_NullList_ThrowsArgumentNullException()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new SdlCalculator(config.SdlConfig);

        Assert.Throws<ArgumentNullException>(() => calculator.CalculateBulk(null!));
    }

    [Fact]
    public void SdlCalculator_BulkCalculation_EmptyList_ReturnsValidResult()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new SdlCalculator(config.SdlConfig);

        var result = calculator.CalculateBulk(new List<decimal>());
        
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalPayroll);
        Assert.Equal(0, result.TotalSdl);
        Assert.Equal(0, result.EmployeeCount);
        Assert.Empty(result.IndividualContributions);
    }

    [Fact]
    public void PayeCalculator_ExtremeAge_ThrowsArgumentException()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        Assert.Throws<ArgumentException>(() => 
            calculator.CalculateAnnualPaye(100000, -5));
        
        Assert.Throws<ArgumentException>(() => 
            calculator.CalculateAnnualPaye(100000, 200));
    }

    [Fact]
    public void PayeCalculator_NegativeIncome_ThrowsArgumentException()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        Assert.Throws<ArgumentException>(() => 
            calculator.CalculateAnnualPaye(-50000, 30));
        
        Assert.Throws<ArgumentException>(() => 
            calculator.CalculateMonthlyPaye(-5000, 30));
    }

    [Fact]
    public void PayeCalculator_NegativeMedicalMembers_ThrowsArgumentException()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        Assert.Throws<ArgumentException>(() => 
            calculator.CalculateAnnualPaye(100000, 30, -2));
        
        Assert.Throws<ArgumentException>(() => 
            calculator.CalculateMonthlyPaye(10000, 30, -1));
    }

    [Fact]
    public void UifCalculator_BoundaryConditions_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new UifCalculator(config.UifConfig);

        // Test exactly at ceiling
        var result = calculator.CalculateMonthly(17712);
        Assert.Equal(177.12m, result.EmployeeAmount);
        Assert.Equal(177.12m, result.EmployerAmount);
        Assert.False(result.CeilingApplied);

        // Test just above ceiling  
        var resultAbove = calculator.CalculateMonthly(17713);
        Assert.Equal(177.12m, resultAbove.EmployeeAmount);
        Assert.Equal(177.12m, resultAbove.EmployerAmount);
        Assert.True(resultAbove.CeilingApplied);
    }

    [Fact]
    public void SdlCalculator_ExactThresholdConditions_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new SdlCalculator(config.SdlConfig);

        // Exactly at exemption threshold
        var resultAtThreshold = calculator.CalculateMonthly(10000, 500000);
        Assert.Equal(0, resultAtThreshold.Amount);
        Assert.True(resultAtThreshold.IsExempt);

        // Just above exemption threshold
        var resultAboveThreshold = calculator.CalculateMonthly(10000, 500001);
        Assert.Equal(100m, resultAboveThreshold.Amount);
        Assert.False(resultAboveThreshold.IsExempt);
    }

    [Fact]
    public void SdlCalculator_NegativeSalary_ThrowsArgumentException()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new SdlCalculator(config.SdlConfig);

        Assert.Throws<ArgumentException>(() => 
            calculator.CalculateMonthly(-1000, 1000000));
    }

    [Fact]
    public void SdlCalculator_NegativePayroll_ThrowsArgumentException()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new SdlCalculator(config.SdlConfig);

        Assert.Throws<ArgumentException>(() => 
            calculator.CalculateMonthly(10000, -500000));
    }
}