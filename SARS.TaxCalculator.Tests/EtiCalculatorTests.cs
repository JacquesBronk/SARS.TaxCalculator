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
        // ETI Configuration as per April 2025 changes
        // Source: Employment Tax Incentive Act - Section 7
        // SARS ETI Guide (LAPD-ETI-G01) - Updated April 2025
        var config = new EtiConfiguration
        {
            MinAge = 18,
            MaxAge = 29,
            MaxQualifyingSalary = 7500,
            Bands = new List<EtiBand>
            {
                // Band 1: R0 - R2,499.99 - 60% of remuneration (capped at R2,500/R1,250)
                new EtiBand { MinSalary = 0, MaxSalary = 2499.99m, FirstYearAmount = 2500, SecondYearAmount = 1250 },
                // Band 2: R2,500 - R5,499.99 - Fixed amounts
                new EtiBand { MinSalary = 2500, MaxSalary = 5499.99m, FirstYearAmount = 1500, SecondYearAmount = 750 },
                // Band 3: R5,500 - R7,499.99 - Sliding scale reduction
                new EtiBand { MinSalary = 5500, MaxSalary = 7499.99m, FirstYearAmount = 1500, SecondYearAmount = 750, ReductionRate = 0.75m },
                // Band 4: R7,500+ - No ETI
                new EtiBand { MinSalary = 7500, MaxSalary = decimal.MaxValue, FirstYearAmount = 0, SecondYearAmount = 0 }
            }
        };
        _calculator = new EtiCalculator(config);
    }

    [Theory]
    [InlineData(1000, 6, 600)] // First year, low salary - 60% of R1,000 = R600
    [InlineData(2000, 6, 1200)] // First year, low salary - 60% of R2,000 = R1,200
    [InlineData(2499, 6, 1499)] // First year, band 1 max - 60% of R2,499 = R1,499.40 rounded
    [InlineData(1000, 18, 300)] // Second year, low salary - 30% of R1,000 = R300
    [InlineData(2000, 18, 600)] // Second year, low salary - 30% of R2,000 = R600
    [InlineData(3000, 6, 1500)] // First year, band 2 - fixed R1,500
    [InlineData(5000, 6, 1500)] // First year, band 2 - fixed R1,500
    [InlineData(5500, 6, 1500)] // First year, band 3 start - R1,500
    [InlineData(6000, 6, 1125)] // First year, band 3 - R1,500 - (R500 * 0.75) = R1,125
    [InlineData(7000, 6, 375)] // First year, band 3 - R1,500 - (R1,500 * 0.75) = R375
    [InlineData(7500, 6, 0)] // At R7,500, no ETI
    [InlineData(8000, 6, 0)] // Above R7,500, no ETI
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
        if (expectedEti > 0)
        {
            Assert.True(result.IsEligible);
        }
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

        // Employee in band 2 gets R1,500 for first year
        Assert.Equal(1500, result.Amount);
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
        // First employee: 60% of R2,000 = R1,200
        // Second employee: R1,500 (band 2)
        // Total: R1,200 + R1,500 = R2,700
        Assert.Equal(2700m, result.TotalEtiAmount);
        Assert.Equal(4, result.IndividualResults.Count);
    }

    [Theory]
    [InlineData(2000, 6, 160, 1200)] // Full hours - 60% of R2,000 = R1,200
    [InlineData(2000, 6, 80, 600)] // Half hours - 60% of R2,000 * 0.5 = R600
    [InlineData(2000, 6, 120, 900)] // 120 hours - 60% of R2,000 * 0.75 = R900
    [InlineData(3000, 6, 160, 1500)] // Full hours, band 2 - R1,500
    [InlineData(3000, 6, 80, 750)] // Half hours, band 2 - R1,500 * 0.5 = R750
    [InlineData(2000, 18, 160, 600)] // Second year, full hours - 30% of R2,000 = R600
    [InlineData(2000, 18, 80, 300)] // Second year, half hours - 30% of R2,000 * 0.5 = R300
    public void CalculateMonthly_WithHoursWorked_AppliesProration(
        decimal salary, int employmentMonths, decimal hoursWorked, decimal expectedEti)
    {
        var employee = new EtiEmployee
        {
            Age = 22,
            MonthlySalary = salary,
            EmploymentMonths = employmentMonths,
            IsFirstTimeEmployee = true,
            HoursWorkedInMonth = hoursWorked
        };

        var result = _calculator.CalculateMonthly(employee);

        Assert.Equal(expectedEti, result.Amount);
        Assert.True(result.IsEligible);
    }

    [Fact]
    public void CalculateMonthly_NoHoursSpecified_AssumesFull160Hours()
    {
        var employee = new EtiEmployee
        {
            Age = 22,
            MonthlySalary = 2000,
            EmploymentMonths = 6,
            IsFirstTimeEmployee = true,
            HoursWorkedInMonth = null // Not specified
        };

        var result = _calculator.CalculateMonthly(employee);

        // Should get full ETI amount - 60% of R2,000 = R1,200
        Assert.Equal(1200, result.Amount);
        Assert.True(result.IsEligible);
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

}
