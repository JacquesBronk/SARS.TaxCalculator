using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SARS.TaxCalculator.Calculators;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Inputs;

namespace SARS.TaxCalculator.Tests;

public class PerformanceAndStressTests
{
    [Fact]
    public void EtiCalculator_LargeEmployeeList_HandlesEfficiently()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new EtiCalculator(config.EtiConfig);

        // Create a large list of employees with various scenarios
        var employees = new List<EtiEmployee>();
        for (int i = 0; i < 1000; i++)
        {
            employees.Add(new EtiEmployee
            {
                Age = 18 + (i % 12), // Ages 18-29
                MonthlySalary = 2000 + (i % 4000), // Varying salaries
                EmploymentMonths = (i % 24) + 1, // 1-24 months
                IsFirstTimeEmployee = i % 2 == 0,
                WorksInSpecialEconomicZone = i % 10 == 0
            });
        }

        var result = calculator.CalculateBulk(employees);

        Assert.NotNull(result);
        Assert.Equal(1000, result.TotalEmployees);
        Assert.True(result.EligibleEmployees > 0);
        Assert.True(result.TotalEtiAmount >= 0);
        Assert.Equal(1000, result.IndividualResults.Count);
    }

    [Fact]
    public void SdlCalculator_LargeSalaryList_HandlesEfficiently()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new SdlCalculator(config.SdlConfig);

        // Create a large list of salaries
        var salaries = new List<decimal>();
        for (int i = 0; i < 5000; i++)
        {
            salaries.Add(5000 + (i * 100)); // Varying salaries from 5k to 505k
        }

        var result = calculator.CalculateBulk(salaries);

        Assert.NotNull(result);
        Assert.Equal(5000, result.EmployeeCount);
        Assert.True(result.TotalPayroll > 0);
        Assert.True(result.TotalSdl >= 0);
        Assert.Equal(5000, result.IndividualContributions.Count());
    }

    [Fact]
    public void TaxCalculator_ExtremeValues_HandlesRobustly()
    {
        // Test with very high salary
        var highSalaryResult = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(500000) // R500k monthly
            .WithAge(45)
            .WithMedicalAid(10, 15000) // High medical aid
            .WithRetirementContribution(0.275m) // Maximum retirement
            .WithCompanyPayroll(50000000) // Very high company payroll
            .Calculate();

        Assert.NotNull(highSalaryResult);
        Assert.Equal(500000, highSalaryResult.GrossSalary);
        Assert.True(highSalaryResult.PAYE > 0);
        Assert.Equal(177.12m, highSalaryResult.UIF); // Should be capped
        Assert.True(highSalaryResult.SDL > 0);
        Assert.True(highSalaryResult.NetSalary > 0);

        // Test with minimum wage
        var minWageResult = TaxCalculator
            .ForTaxYear(2024)
            .WithGrossSalary(3500) // Near minimum wage
            .WithAge(19) // Young worker
            .WithEtiDetails(3, true, false)
            .Calculate();

        Assert.NotNull(minWageResult);
        Assert.Equal(3500, minWageResult.GrossSalary);
        Assert.True(minWageResult.ETI > 0); // Should get ETI
        Assert.True(minWageResult.NetSalary > 0);
    }

    [Fact]
    public void PayeCalculator_MaximumRetirementScenarios_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        // Test scenarios at retirement contribution limits
        var scenarios = new[]
        {
            new { Income = 500000m, Retirement = 137500m }, // 27.5% of 500k
            new { Income = 1000000m, Retirement = 275000m }, // 27.5% of 1M
            new { Income = 2000000m, Retirement = 350000m }, // Capped at 350k
            new { Income = 200000m, Retirement = 55000m }, // 27.5% of 200k
        };

        foreach (var scenario in scenarios)
        {
            var result = calculator.CalculatePayeWithRetirement(
                scenario.Income, scenario.Retirement, 35, 0);
            var resultWithoutRetirement = calculator.CalculateAnnualPaye(scenario.Income, 35, 0);

            Assert.True(result < resultWithoutRetirement,
                $"PAYE with retirement should be less for income {scenario.Income}");
        }
    }

    [Fact]
    public void AllCalculators_ZeroInputs_HandleGracefully()
    {
        var config = TaxYearData.GetConfiguration(2024);

        // PAYE Calculator with zero income
        var payeCalc = new PayeCalculator(config);
        Assert.Equal(0, payeCalc.CalculateAnnualPaye(0, 30));
        Assert.Equal(0, payeCalc.CalculateMonthlyPaye(0, 30));

        // UIF Calculator with zero income
        var uifCalc = new UifCalculator(config.UifConfig);
        var uifResult = uifCalc.CalculateMonthly(0);
        Assert.Equal(0, uifResult.EmployeeAmount);
        Assert.Equal(0, uifResult.EmployerAmount);
        Assert.False(uifResult.CeilingApplied);

        // SDL Calculator with zero income
        var sdlCalc = new SdlCalculator(config.SdlConfig);
        var sdlResult = sdlCalc.CalculateMonthly(0, 1000000);
        Assert.Equal(0, sdlResult.Amount);
        Assert.False(sdlResult.IsExempt); // Not exempt due to high payroll
    }

    [Fact]
    public void TaxCalculator_AllTaxYears_HandleConsistently()
    {
        var supportedYears = TaxCalculator.SupportedYears;

        foreach (var year in supportedYears)
        {
            var result = TaxCalculator
                .ForTaxYear(year)
                .WithGrossSalary(25000)
                .WithAge(35)
                .Calculate();

            Assert.NotNull(result);
            Assert.Equal(year, result.TaxYear);
            Assert.Equal(25000, result.GrossSalary);
            Assert.Equal(35, result.Age);
            Assert.True(result.NetSalary > 0);
        }
    }

    [Fact]
    public void EtiCalculator_EdgeCaseCombinations_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new EtiCalculator(config.EtiConfig);

        var edgeCases = new[]
        {
            // Exact age boundaries
            new EtiEmployee { Age = 18, MonthlySalary = 2000, EmploymentMonths = 1, IsFirstTimeEmployee = true },
            new EtiEmployee { Age = 29, MonthlySalary = 2000, EmploymentMonths = 1, IsFirstTimeEmployee = true },
            
            // Exact salary boundaries
            new EtiEmployee { Age = 25, MonthlySalary = 7500, EmploymentMonths = 1, IsFirstTimeEmployee = true },
            new EtiEmployee { Age = 25, MonthlySalary = 7499, EmploymentMonths = 1, IsFirstTimeEmployee = true },
            
            // Employment period boundaries
            new EtiEmployee { Age = 25, MonthlySalary = 3000, EmploymentMonths = 24, IsFirstTimeEmployee = false },
            new EtiEmployee { Age = 25, MonthlySalary = 3000, EmploymentMonths = 23, IsFirstTimeEmployee = false },
            
            // SEZ cases
            new EtiEmployee { Age = 35, MonthlySalary = 3000, EmploymentMonths = 12, IsFirstTimeEmployee = true, WorksInSpecialEconomicZone = true },
            new EtiEmployee { Age = 35, MonthlySalary = 3000, EmploymentMonths = 12, IsFirstTimeEmployee = true, WorksInSpecialEconomicZone = false },
        };

        foreach (var employee in edgeCases)
        {
            var result = calculator.CalculateMonthly(employee);
            Assert.NotNull(result);
            Assert.NotNull(result.Employee);
        }
    }

    [Fact]
    public void PayeCalculator_VariousAgeGroups_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new PayeCalculator(config);

        var ageGroups = new[] { 25, 45, 65, 75, 85 };

        foreach (var age in ageGroups)
        {
            var result = calculator.CalculateAnnualPaye(300000, age, 2);
            Assert.True(result >= 0);

            var monthlyResult = calculator.CalculateMonthlyPaye(25000, age, 2);
            Assert.True(monthlyResult >= 0);
        }
    }

    [Fact]
    public void SdlCalculator_VariousPayrollScenarios_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new SdlCalculator(config.SdlConfig);

        var payrollScenarios = new[] { 400000m, 500000m, 500001m, 1000000m, 10000000m };

        foreach (var payroll in payrollScenarios)
        {
            var result = calculator.CalculateMonthly(25000, payroll);
            Assert.NotNull(result);
            Assert.True(result.Amount >= 0);

            if (payroll <= 500000)
                Assert.True(result.IsExempt);
            else
                Assert.False(result.IsExempt);
        }
    }

    [Fact]
    public void UifCalculator_VariousIncomeScenarios_HandlesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2024);
        var calculator = new UifCalculator(config.UifConfig);

        var incomeScenarios = new[] { 0m, 5000m, 10000m, 17712m, 17713m, 25000m, 50000m };

        foreach (var income in incomeScenarios)
        {
            var result = calculator.CalculateMonthly(income);
            Assert.NotNull(result);
            Assert.True(result.EmployeeAmount >= 0);
            Assert.True(result.EmployerAmount >= 0);
            Assert.Equal(result.EmployeeAmount, result.EmployerAmount);

            if (income > 17712)
                Assert.True(result.CeilingApplied);
            else
                Assert.False(result.CeilingApplied);
        }
    }
}
