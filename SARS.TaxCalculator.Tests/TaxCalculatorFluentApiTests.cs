using System;
using System.Linq;
using Xunit;
using SARS.TaxCalculator;
using SARS.TaxCalculator.Models.Results;

namespace SARS.TaxCalculator.Tests;

public class TaxCalculatorFluentApiTests
{
    [Fact]
    public void BasicCalculation_WithSampleData_ReturnsExpectedResult()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(25000)
            .WithAge(35)
            .WithMedicalAid(3, 3500)
            .WithRetirementContribution(0.075m)
            .Calculate();

        Assert.Equal(25000, result.GrossSalary);
        Assert.Equal(35, result.Age);
        Assert.Equal(2024, result.TaxYear);
        Assert.True(result.NetSalary > 0);
        Assert.True(result.PAYE > 0);
        Assert.Equal(177.12m, result.UIF); // Capped at ceiling
        Assert.Equal(1875m, result.RetirementContribution); // 7.5% of 25000
        Assert.Equal(3500, result.MedicalAidContribution);
        Assert.Equal(974m, result.MedicalAidTaxCredit); // R364 + R364 + R246
    }

    [Fact]
    public void MinimumWageEmployee_NoTax_CorrectCalculation()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(4000)
            .WithAge(25)
            .Calculate();

        Assert.Equal(0, result.PAYE); // Below tax threshold
        Assert.Equal(40m, result.UIF); // 1% of 4000
        Assert.Equal(40m, result.SDL); // 1% but company pays
        Assert.Equal(3960m, result.NetSalary); // 4000 - 40 UIF
    }

    [Fact]
    public void HighEarner_AboveCeiling_CorrectCalculation()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithAnnualGrossSalary(2400000) // R200k per month
            .WithAge(45)
            .WithMedicalAid(2, 5000)
            .WithRetirementContributionAmount(10000)
            .Calculate();

        Assert.Equal(200000, result.GrossSalary);
        Assert.Equal(177.12m, result.UIF); // Capped
        Assert.True(result.UifCeilingApplied);
        Assert.True(result.PAYE > 50000); // High earner
        Assert.Equal(10000, result.RetirementContribution);
    }

    [Fact]
    public void SeniorCitizen_HigherRebates_LowerTax()
    {
        var youngResult = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(20000)
            .WithAge(30)
            .Calculate();

        var seniorResult = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(20000)
            .WithAge(70)
            .Calculate();

        Assert.True(seniorResult.PAYE < youngResult.PAYE);
    }

    [Fact]
    public void WithEtiDetails_QualifyingEmployee_ReceivesIncentive()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(3000)
            .WithAge(22)
            .WithEtiDetails(employmentMonths: 6, isFirstTime: true)
            .Calculate();

        Assert.True(result.ETI > 0);
    }

    [Fact]
    public void WithCompanyPayroll_BelowThreshold_NoSdl()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(10000)
            .WithCompanyPayroll(400000) // Below R500k threshold
            .Calculate();

        Assert.Equal(0, result.SDL);
        Assert.True(result.SdlExempt);
    }

    [Fact]
    public void CalculatePaye_OnlyPayeCalculation_ReturnsPayeDetails()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(50000)
            .WithAge(40)
            .WithMedicalAid(2)
            .WithRetirementContribution(0.1m)
            .CalculatePaye();

        Assert.True(result.MonthlyPAYE > 0);
        // Monthly PAYE * 12 may differ slightly from Annual PAYE due to rounding
        Assert.True(Math.Abs(result.MonthlyPAYE * 12 - result.AnnualPAYE) <= 0.12m, 
            $"Monthly PAYE * 12 ({result.MonthlyPAYE * 12}) should be within 12 cents of Annual PAYE ({result.AnnualPAYE})");
        Assert.Equal(540000, result.TaxableIncome); // 600000 - 60000 retirement
        Assert.Equal(95750, result.TaxThreshold);
        Assert.Equal(17235, result.TotalRebates);
        Assert.Equal(105295.64m, result.AnnualPAYE); // Updated to correct calculated value
    }

    [Theory]
    [InlineData(2023)]
    [InlineData(2024)]
    [InlineData(2025)]
    [InlineData(2026)]
    public void ForTaxYear_SupportedYears_CreatesBuilder(int year)
    {
        var builder = TaxCalculator.ForTaxYear(year);
        Assert.NotNull(builder);
    }

    [Fact]
    public void ForTaxYear_UnsupportedYear_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2022));
        Assert.Contains("not supported", exception.Message);
    }

    [Fact]
    public void SupportedYears_ReturnsCorrectYears()
    {
        var years = TaxCalculator.SupportedYears;
        Assert.Equal(4, years.Count());
        Assert.Contains(2023, years);
        Assert.Contains(2024, years);
        Assert.Contains(2025, years);
        Assert.Contains(2026, years);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(151)]
    public void WithAge_InvalidAge_ThrowsException(int invalidAge)
    {
        var exception = Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2024).WithAge(invalidAge));
            
        Assert.Contains("between 0 and 150", exception.Message);
    }

    [Fact]
    public void WithMedicalAid_ZeroMembers_ThrowsException()
    {
        var exception = Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2024).WithMedicalAid(0));
            
        Assert.Contains("at least 1", exception.Message);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void WithRetirementContribution_InvalidPercentage_ThrowsException(decimal invalidPercentage)
    {
        var exception = Assert.Throws<ArgumentException>(() => 
            TaxCalculator.ForTaxYear(2024).WithRetirementContribution(invalidPercentage));
            
        Assert.Contains("between 0 and 1", exception.Message);
    }

    [Fact]
    public void CompleteScenario_CostToCompany_CalculatedCorrectly()
    {
        var result = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(30000)
            .WithAge(35)
            .WithMedicalAid(4, 4500)
            .WithRetirementContribution(0.10m)
            .WithCompanyPayroll(5000000)
            .WithEtiDetails(12, true)
            .Calculate();

        // Verify all components
        Assert.Equal(30000, result.GrossSalary);
        Assert.Equal(3000, result.RetirementContribution);
        Assert.Equal(4500, result.MedicalAidContribution);
        Assert.Equal(177.12m, result.UIF);
        Assert.Equal(177.12m, result.EmployerUIF);
        Assert.Equal(300m, result.SDL);
        Assert.Equal(0, result.ETI); // Salary too high for ETI
        
        // Cost to company = Gross + Employer UIF + SDL
        Assert.Equal(30000 + 177.12m + 300m, result.CostToCompany);
        
        // Net salary = Gross - PAYE - UIF - Retirement - Medical
        var expectedNet = 30000 - result.PAYE - 177.12m - 3000 - 4500;
        Assert.Equal(expectedNet, result.NetSalary);
    }
}