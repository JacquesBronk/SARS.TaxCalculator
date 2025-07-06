using System;
using SARS.TaxCalculator;
using SARS.TaxCalculator.Calculators;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Inputs;
using SARS.TaxCalculator.Models.Results;

namespace SARS.TaxCalculator.Sample;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("SARS Tax Calculator Sample Application");
        Console.WriteLine("=====================================\n");

        // Example 1: Basic tax calculation using fluent API
        Example1_BasicCalculation();
        
        // Example 2: Different age groups
        Example2_AgeGroupComparison();
        
        // Example 3: ETI calculation
        Example3_EtiCalculation();
        
        // Example 4: Complete payslip
        Example4_CompletePayslip();
        
        // Example 5: Bulk calculations
        Example5_BulkCalculations();

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static void Example1_BasicCalculation()
    {
        Console.WriteLine("Example 1: Basic Tax Calculation");
        Console.WriteLine("--------------------------------");

        var result = TaxCalculator
            .ForTaxYear(2026)
            .WithGrossSalary(25000)
            .WithAge(35)
            .WithMedicalAid(3, 3500)
            .WithRetirementContribution(0.075m)
            .Calculate();

        Console.WriteLine($"Gross Monthly Salary: R{result.GrossSalary:N2}");
        Console.WriteLine($"PAYE: R{result.PAYE:N2}");
        Console.WriteLine($"UIF: R{result.UIF:N2}");
        Console.WriteLine($"Medical Aid: R{result.MedicalAidContribution:N2}");
        Console.WriteLine($"Medical Tax Credit: R{result.MedicalAidTaxCredit:N2}");
        Console.WriteLine($"Retirement: R{result.RetirementContribution:N2}");
        Console.WriteLine($"Net Salary: R{result.NetSalary:N2}");
        Console.WriteLine($"Cost to Company: R{result.CostToCompany:N2}");
        Console.WriteLine();
    }

    static void Example2_AgeGroupComparison()
    {
        Console.WriteLine("Example 2: Age Group Tax Comparison");
        Console.WriteLine("-----------------------------------");

        var salary = 30000m;
        var ages = new[] { 30, 65, 75 };
        var ageGroups = new[] { "Under 65", "65-74", "75+" };

        for (int i = 0; i < ages.Length; i++)
        {
            var result = TaxCalculator
                .ForTaxYear(2026)
                .WithGrossSalary(salary)
                .WithAge(ages[i])
                .Calculate();

            Console.WriteLine($"{ageGroups[i]} (Age {ages[i]}):");
            Console.WriteLine($"  PAYE: R{result.PAYE:N2}");
            Console.WriteLine($"  Net Salary: R{result.NetSalary:N2}");
        }
        Console.WriteLine();
    }

    static void Example3_EtiCalculation()
    {
        Console.WriteLine("Example 3: ETI (Employment Tax Incentive)");
        Console.WriteLine("-----------------------------------------");

        var salaries = new[] { 2000m, 3500m, 5000m, 7000m };
        
        foreach (var salary in salaries)
        {
            var result = TaxCalculator
                .ForTaxYear(2026)
                .WithGrossSalary(salary)
                .WithAge(22)
                .WithEtiDetails(employmentMonths: 6, isFirstTime: true)
                .Calculate();

            Console.WriteLine($"Salary R{salary:N2}: ETI = R{result.ETI:N2}");
        }
        Console.WriteLine();
    }

    static void Example4_CompletePayslip()
    {
        Console.WriteLine("Example 4: Complete Payslip Calculation");
        Console.WriteLine("---------------------------------------");

        var config = TaxYearData.GetConfiguration(2026);
        var payslipCalculator = new PayslipCalculator(config);

        var input = new PayslipInput
        {
            EmployeeId = "EMP001",
            EmployeeName = "John Smith",
            Age = 32,
            GrossSalary = 45000,
            IsAnnualSalary = false,
            PayMonth = 12,
            PayYear = 2026,
            MedicalAidMembers = 2,
            MedicalAidContribution = 3200,
            RetirementContribution = 4500,
            EmployerRetirementContribution = 4500,
            CompanyAnnualPayroll = 10000000,
            IsEtiEligible = false
        };

        var payslip = payslipCalculator.Calculate(input);

        Console.WriteLine($"Employee: {payslip.Employee.Name}");
        Console.WriteLine($"Period: {payslip.Period.Month}/{payslip.Period.Year}");
        Console.WriteLine("\nEarnings:");
        Console.WriteLine($"  Basic Salary: R{payslip.Earnings.BasicSalary:N2}");
        Console.WriteLine("\nDeductions:");
        Console.WriteLine($"  PAYE: R{payslip.Deductions.PAYE:N2}");
        Console.WriteLine($"  UIF: R{payslip.Deductions.UIF:N2}");
        Console.WriteLine($"  Retirement: R{payslip.Deductions.RetirementContribution:N2}");
        Console.WriteLine($"  Medical Aid: R{payslip.Deductions.MedicalAidContribution:N2}");
        Console.WriteLine($"  Total Deductions: R{payslip.Deductions.TotalDeductions:N2}");
        Console.WriteLine("\nEmployer Contributions:");
        Console.WriteLine($"  UIF: R{payslip.EmployerContributions.UIF:N2}");
        Console.WriteLine($"  SDL: R{payslip.EmployerContributions.SDL:N2}");
        Console.WriteLine($"  Retirement: R{payslip.EmployerContributions.RetirementContribution:N2}");
        Console.WriteLine("\nSummary:");
        Console.WriteLine($"  Net Pay: R{payslip.Summary.NetPay:N2}");
        Console.WriteLine($"  Cost to Company: R{payslip.Summary.CostToCompany:N2}");
        Console.WriteLine();
    }

    static void Example5_BulkCalculations()
    {
        Console.WriteLine("Example 5: Bulk Salary Calculations");
        Console.WriteLine("-----------------------------------");

        var salaries = new[] { 15000m, 25000m, 35000m, 50000m, 75000m, 100000m };
        
        Console.WriteLine("Monthly Salary | PAYE      | UIF     | Net Salary");
        Console.WriteLine("---------------|-----------|---------|------------");

        foreach (var salary in salaries)
        {
            var result = TaxCalculator
                .ForTaxYear(2026)
                .WithGrossSalary(salary)
                .WithAge(35)
                .Calculate();

            Console.WriteLine($"R{salary,12:N0} | R{result.PAYE,8:N2} | R{result.UIF,6:N2} | R{result.NetSalary,10:N2}");
        }

        Console.WriteLine("\nNote: UIF is capped at R177.12 for salaries above R17,712");
        Console.WriteLine();
    }
}