using System;
using System.Collections.Generic;
using Xunit;
using SARS.TaxCalculator.Calculators;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Inputs;

namespace SARS.TaxCalculator.Tests;

public class EtiCalculatorTests
{
    private readonly EtiCalculator _calculator;

    public EtiCalculatorTests()
    {
        var config = new EtiConfiguration
        {
            MinAge = 18,
            MaxAge = 29,
            MaxQualifyingSalary = 7500,
            Bands = new List<EtiBand>
            {
                new EtiBand { MinSalary = 0, MaxSalary = 2000, FirstYearAmount = 1500, SecondYearAmount = 750 },
                new EtiBand { MinSalary = 2001, MaxSalary = 4500, FirstYearAmount = 1500, SecondYearAmount = 750, ReductionRate = 0.5m },
                new EtiBand { MinSalary = 4501, MaxSalary = 6500, FirstYearAmount = 750, SecondYearAmount = 375, ReductionRate = 0.25m },
                new EtiBand { MinSalary = 6501, MaxSalary = 7500, FirstYearAmount = 0, SecondYearAmount = 0 }
            }
        };
        _calculator = new EtiCalculator(config);
    }

    [Theory]
    [InlineData(2000, 6, 1500)] // First year, low salary
    [InlineData(2000, 18, 750)] // Second year, low salary
    [InlineData(3000, 6, 1000)] // First year, mid-salary with reduction
    [InlineData(5000, 6, 625)] // First year, higher salary with reduction
    [InlineData(7000, 6, 0)] // Above R6500, no ETI
    public void CalculateMonthly_VariousSalariesAndMonths_ReturnsCorrectEti(
        decimal salary, int employmentMonths, decimal expectedEti)
    {
        var employee = new EtiEmployee
        {
            Age = 22,
            MonthlySalary = salary,
            EmploymentMonths = employmentMonths,
            IsFirstTimeEmployee = true
        };

        var result = _calculator.CalculateMonthly(employee);

        Assert.Equal(expectedEti, result.Amount);
        Assert.True(result.IsEligible);
    }

    [Theory]
    [InlineData(17, "Age 17 is outside eligible range (18-29)")]
    [InlineData(30, "Age 30 is outside eligible range (18-29)")]
    [InlineData(35, "Age 35 is outside eligible range (18-29)")]
    public void CalculateMonthly_InvalidAge_ReturnsIneligible(int age, string expectedReason)
    {
        var employee = new EtiEmployee
        {
            Age = age,
            MonthlySalary = 3000,
            EmploymentMonths = 6,
            IsFirstTimeEmployee = true
        };

        var result = _calculator.CalculateMonthly(employee);

        Assert.Equal(0, result.Amount);
        Assert.False(result.IsEligible);
        Assert.Equal(expectedReason, result.IneligibilityReason);
    }

    [Fact]
    public void CalculateMonthly_SalaryTooHigh_ReturnsIneligible()
    {
        var employee = new EtiEmployee
        {
            Age = 22,
            MonthlySalary = 8000,
            EmploymentMonths = 6,
            IsFirstTimeEmployee = true
        };

        var result = _calculator.CalculateMonthly(employee);

        Assert.Equal(0, result.Amount);
        Assert.False(result.IsEligible);
        Assert.Contains("exceeds maximum qualifying salary", result.IneligibilityReason);
    }

    [Fact]
    public void CalculateMonthly_EmploymentPeriodExpired_ReturnsIneligible()
    {
        var employee = new EtiEmployee
        {
            Age = 22,
            MonthlySalary = 3000,
            EmploymentMonths = 25,
            IsFirstTimeEmployee = false
        };

        var result = _calculator.CalculateMonthly(employee);

        Assert.Equal(0, result.Amount);
        Assert.False(result.IsEligible);
        Assert.Contains("ETI period has expired", result.IneligibilityReason);
    }

    [Fact]
    public void CalculateMonthly_SpecialEconomicZone_IgnoresAgeRestriction()
    {
        var employee = new EtiEmployee
        {
            Age = 35, // Over 29
            MonthlySalary = 3000,
            EmploymentMonths = 6,
            IsFirstTimeEmployee = true,
            WorksInSpecialEconomicZone = true
        };

        var result = _calculator.CalculateMonthly(employee);

        Assert.True(result.Amount > 0);
        Assert.True(result.IsEligible);
    }

    [Fact]
    public void CalculateBulk_MultipleEmployees_ReturnsCorrectSummary()
    {
        var employees = new[]
        {
            new EtiEmployee { Age = 22, MonthlySalary = 2000, EmploymentMonths = 6, IsFirstTimeEmployee = true },
            new EtiEmployee { Age = 25, MonthlySalary = 5000, EmploymentMonths = 6, IsFirstTimeEmployee = true },
            new EtiEmployee { Age = 35, MonthlySalary = 3000, EmploymentMonths = 6, IsFirstTimeEmployee = true }, // Ineligible
            new EtiEmployee { Age = 28, MonthlySalary = 8000, EmploymentMonths = 6, IsFirstTimeEmployee = true }  // Salary too high
        };

        var result = _calculator.CalculateBulk(employees);

        Assert.Equal(4, result.TotalEmployees);
        Assert.Equal(2, result.EligibleEmployees);
        Assert.Equal(2125m, result.TotalEtiAmount); // 1500 + 625 (truncated)
        Assert.Equal(4, result.IndividualResults.Count);
    }

    [Fact]
    public void CalculateMonthly_NullEmployee_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => _calculator.CalculateMonthly(null!));
    }

    [Fact]
    public void CalculateBulk_NullList_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => _calculator.CalculateBulk(null!));
    }

    [Fact]
    public void Constructor_NullConfig_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new EtiCalculator(null!));
    }

    [Theory]
    [InlineData(2500, 6, 1250)] // (1500 - (500 * 0.5)) = 1250.5 truncated to 1250
    [InlineData(3500, 6, 750)]  // (1500 - (1500 * 0.5)) = 750.5 truncated to 750
    [InlineData(4000, 6, 500)]  // (1500 - (2000 * 0.5)) = 500.5 truncated to 500
    public void CalculateMonthly_SalaryReduction_AppliesCorrectly(
        decimal salary, int months, decimal expectedEti)
    {
        var employee = new EtiEmployee
        {
            Age = 22,
            MonthlySalary = salary,
            EmploymentMonths = months,
            IsFirstTimeEmployee = true
        };

        var result = _calculator.CalculateMonthly(employee);

        Assert.Equal(expectedEti, result.Amount);
    }
}
