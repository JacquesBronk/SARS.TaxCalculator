# SARS.TaxCalculator

A comprehensive South African SARS tax calculation NuGet package supporting PAYE, UIF, SDL, ETI and complete payslip calculations for tax years 2023-2027.

[![NuGet](https://img.shields.io/nuget/v/SARS.TaxCalculator.svg)](https://www.nuget.org/packages/SARS.TaxCalculator/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![SARS Compliant](https://img.shields.io/badge/SARS-Compliant-green.svg)](https://www.sars.gov.za)
[![Test Coverage](https://img.shields.io/badge/Coverage-100%25-brightgreen.svg)](https://github.com/JacquesBronk/SARS.TaxCalculator)
[![Tests](https://img.shields.io/badge/Tests-416%20Passing-brightgreen.svg)](https://github.com/JacquesBronk/SARS.TaxCalculator)

## Features

- ✅ **PAYE Calculation** - With age-based rebates and medical aid credits
- ✅ **UIF Calculation** - Employee and employer contributions with R17,712 monthly ceiling
- ✅ **SDL Calculation** - Skills Development Levy with R500,000 annual exemption
- ✅ **ETI Calculation** - Employment Tax Incentive with April 2025 changes (R2,500 max, hours proration)
- ✅ **Medical Aid Credits** - R376 main/first dependent, R254 additional dependents (2027)
- ✅ **Retirement Deductions** - Max 27.5% of taxable income, R430,000 annual cap (2027)
- ✅ **Complete Payslips** - Full gross-to-net calculations
- ✅ **Fluent API** - Intuitive, chainable interface
- ✅ **Multi-year Support** - Tax years 2023, 2024, 2025, 2026, and 2027
- ✅ **SARS Compliant** - Fully compliant with official SARS legislation and rounding rules
- ✅ **Date-Aware ETI** - Handles mid-year ETI rate changes (e.g., March vs April 2025)
- ✅ **Comprehensive Testing** - 100% line coverage with 416 passing tests
- ✅ **Bulk Processing** - Calculate multiple employees efficiently
- ✅ **.NET Standard 2.1** - Compatible with .NET Core 3.0+, .NET 5+, .NET 8+, and .NET Framework 4.8+

## Installation

```bash
dotnet add package SARS.TaxCalculator
```

Or via Package Manager:
```powershell
Install-Package SARS.TaxCalculator
```

## Quick Start

```csharp
using SARS.TaxCalculator;

var result = TaxCalculator
    .ForTaxYear(2027)
    .WithGrossSalary(25000)
    .WithAge(35)
    .WithMedicalAid(3, 3500)
    .WithRetirementContribution(0.075m)
    .Calculate();

Console.WriteLine($"Monthly PAYE: R{result.PAYE:N2}");
Console.WriteLine($"Net Salary: R{result.NetSalary:N2}");
```

## Tax Tables (2027)

### Income Tax Brackets
| Annual Income | Tax Rate |
|--------------|----------|
| R0 - R245,100 | 18% |
| R245,101 - R383,100 | R44,118 + 26% |
| R383,101 - R530,200 | R79,998 + 31% |
| R530,201 - R695,800 | R125,599 + 36% |
| R695,801 - R887,000 | R185,215 + 39% |
| R887,001 - R1,878,600 | R259,783 + 41% |
| R1,878,601+ | R666,339 + 45% |

*Source: [SARS Tax Rates for Individuals](https://www.sars.gov.za/tax-rates/income-tax/rates-of-tax-for-individuals/) - Published 25 February 2026*

### Tax Rebates
- Primary (all ages): R17,820
- Secondary (65+): R9,765
- Tertiary (75+): R3,249

### Tax Thresholds
- Under 65: R99,000
- 65-74: R153,250
- 75+: R171,300

## Detailed Examples

### Basic PAYE Calculation

```csharp
var result = TaxCalculator
    .ForTaxYear(2027)
    .WithGrossSalary(30000)
    .WithAge(40)
    .Calculate();

// Access results
decimal monthlyPaye = result.PAYE;
decimal netSalary = result.NetSalary;
```

### With Medical Aid and Retirement

```csharp
var result = TaxCalculator
    .ForTaxYear(2027)
    .WithGrossSalary(45000)
    .WithAge(35)
    .WithMedicalAid(4, 5000)  // 4 members, R5000/month contribution
    .WithRetirementContribution(0.10m)  // 10% of gross
    .Calculate();
```

### ETI Calculation

```csharp
var result = TaxCalculator
    .ForTaxYear(2027)
    .WithGrossSalary(3500)
    .WithAge(22)
    .WithEtiDetails(
        employmentMonths: 6,
        isFirstTime: true,
        inSez: false)
    .Calculate();

decimal etiAmount = result.ETI;  // Employment Tax Incentive
```

### Date-Aware ETI (Mid-Year Rate Changes)

The 2026 tax year spans a mid-year ETI rate change effective 1 April 2025. Use `ForPaymentDate()` to get the correct ETI rates for a specific payroll month:

```csharp
// March 2025: OLD ETI rates (max salary R6,500)
var marchResult = TaxCalculator
    .ForTaxYear(2026)
    .WithGrossSalary(7000)
    .WithAge(22)
    .WithEtiDetails(employmentMonths: 6)
    .ForPaymentDate(3, 2025)
    .Calculate();
// marchResult.ETI == 0 (R7,000 exceeds old R6,500 threshold)

// April 2025: NEW ETI rates (max salary R7,500)
var aprilResult = TaxCalculator
    .ForTaxYear(2026)
    .WithGrossSalary(7000)
    .WithAge(22)
    .WithEtiDetails(employmentMonths: 6)
    .ForPaymentDate(4, 2025)
    .Calculate();
// aprilResult.ETI == 375 (eligible under new R7,500 threshold)
```

### Complete Payslip

```csharp
var config = TaxYearData.GetConfiguration(2027);
var calculator = new PayslipCalculator(config);

var payslip = calculator.Calculate(new PayslipInput
{
    EmployeeId = "EMP001",
    EmployeeName = "John Doe",
    Age = 35,
    GrossSalary = 35000,
    MedicalAidMembers = 2,
    MedicalAidContribution = 3000,
    RetirementContribution = 3500,
    CompanyAnnualPayroll = 5000000
});

// Detailed breakdown available
Console.WriteLine($"PAYE: R{payslip.Deductions.PAYE:N2}");
Console.WriteLine($"UIF: R{payslip.Deductions.UIF:N2}");
Console.WriteLine($"Net Pay: R{payslip.Summary.NetPay:N2}");
```

### Bulk Calculations

```csharp
var inputs = new List<PayslipInput>
{
    new() { EmployeeId = "001", GrossSalary = 25000, Age = 30 },
    new() { EmployeeId = "002", GrossSalary = 35000, Age = 45 },
    new() { EmployeeId = "003", GrossSalary = 50000, Age = 55 }
};

var bulkResult = calculator.CalculateBulk(inputs);
Console.WriteLine($"Total Payroll: R{bulkResult.Summary.TotalGrossEarnings:N2}");
Console.WriteLine($"Total PAYE: R{bulkResult.Summary.TotalPAYE:N2}");
```

## API Reference

### TaxCalculator (Fluent API)

```csharp
TaxCalculator
    .ForTaxYear(int year)
    .WithGrossSalary(decimal amount)
    .WithAnnualGrossSalary(decimal amount)
    .WithAge(int age)
    .WithMedicalAid(int members, decimal contribution = 0)
    .WithRetirementContribution(decimal percentage)
    .WithRetirementContributionAmount(decimal amount)
    .WithCompanyPayroll(decimal annualPayroll)
    .WithEtiDetails(int months, bool firstTime, bool inSez)
    .ForPaymentDate(int month, int year)
    .Calculate()
    .CalculatePaye()
```

### Individual Calculators

- `PayeCalculator` - PAYE tax calculations
- `UifCalculator` - UIF contributions
- `SdlCalculator` - Skills Development Levy
- `EtiCalculator` - Employment Tax Incentive
- `PayslipCalculator` - Complete payslip generation

## Requirements

- .NET Standard 2.1 compatible runtime:
  - .NET Core 3.0 or later
  - .NET 5.0 or later
  - .NET Framework 4.8 or later (Windows only)
  - Xamarin.iOS 12.16+
  - Xamarin.Android 10.0+
  - Unity 2021.2+

## Configuration

### Supported Tax Years
- 2023 (1 March 2022 - 28 February 2023)
- 2024 (1 March 2023 - 29 February 2024)
- 2025 (1 March 2024 - 28 February 2025)
- 2026 (1 March 2025 - 28 February 2026)
- 2027 (1 March 2026 - 28 February 2027)

### Key Limits
- UIF Monthly Ceiling: R17,712
- SDL Exemption: R500,000 annual payroll
- Retirement Deduction: 27.5% of income, max R430,000/year (2027)
- ETI Maximum: R2,500/month (first year), R1,250/month (second year) for 160+ hours
- ETI Salary Threshold: R7,500/month (effective April 2025)
- ETI Age Range: 18-29 (except SEZ employees)

## SARS Compliance

This package is **fully compliant** with South African Revenue Service (SARS) legislation and tax code requirements:

### ✅ **Verified Compliance Areas**
- **Tax Calculations**: Based on official SARS tax brackets and rates
- **Rounding Rules**: Implements SARS-specific rounding per validation rules
- **ETI Calculations**: Compliant with Employment Tax Incentive Act 2013
- **PAYE Processing**: Follows Fourth Schedule Income Tax Act procedures
- **Special Economic Zone**: Proper age exemptions implemented

### 📋 **SARS Sources & Citations**
- [SARS Tax Rates for Individuals](https://www.sars.gov.za/tax-rates/income-tax/rates-of-tax-for-individuals/)
- [SARS Medical Tax Credit Rates](https://www.sars.gov.za/tax-rates/medical-tax-credit-rates/)
- [Employment Tax Incentive Guidelines](https://www.sars.gov.za/types-of-tax/pay-as-you-earn/employment-tax-incentive-eti/)
- [SARS Validation Rules 2025](https://www.sars.gov.za/guide-for-validation-rules-applicable-to-reconciliation-declarations-2025/)
- [2026 Budget Speech](https://www.gov.za/2026BudgetSpeech)
- Fourth Schedule to Income Tax Act, 1962
- Employment Tax Incentive Act, 2013

### 🔄 **Rounding Strategy**
| Component | Method | Compliance |
|-----------|--------|------------|
| PAYE Tax | Round to cents | ✅ SARS validation rules |
| UIF/SDL | Round to cents | ✅ SARS validation rules |
| ETI | Truncate to Rands | ✅ "Cents must be dropped off" |

For detailed compliance documentation, see [SARS_COMPLIANCE_REVIEW.md](SARS_COMPLIANCE_REVIEW.md).

## Testing & Quality Assurance

The package includes **comprehensive test coverage** ensuring reliability and correctness:

### 📊 **Test Coverage Metrics**
- **Line Coverage: 100%** (1209 out of 1209 lines)
- **Branch Coverage: 98.5%** (203 out of 206 branches)
- **Method Coverage: 100%** (235 out of 235 methods)
- **Total Tests: 416** across all components

### 🧪 **Testing Categories**
- **Unit Tests**: All core calculation logic
- **Integration Tests**: End-to-end calculation workflows
- **Edge Case Tests**: Boundary conditions and limits
- **Exception Handling**: Input validation and error scenarios
- **Performance Tests**: Large-scale bulk calculations (1000+ employees)
- **Compliance Tests**: SARS validation rule verification

### 🎯 **Key Test Areas**
- ✅ PAYE calculations across all tax brackets and age groups
- ✅ UIF ceiling application and rounding validation
- ✅ SDL exemption thresholds and payroll scenarios
- ✅ ETI eligibility matrix (age, salary, employment duration, SEZ)
- ✅ Date-aware ETI resolution (mid-year rate changes, old vs new rates)
- ✅ Medical aid credit calculations for all member configurations
- ✅ Retirement contribution limits and deduction rules
- ✅ Tax threshold applications for different age groups
- ✅ Fluent API validation and error handling
- ✅ Bulk processing with thousands of records
- ✅ Cross-tax-year consistency validation

### 🚀 **Running Tests**
```bash
# Run all tests
dotnet test

# Run with coverage report
dotnet test --collect:"XPlat Code Coverage"

# Performance test suite
dotnet test --filter "Category=Performance"
```

The extensive test suite ensures that all SARS compliance requirements are met and calculations remain accurate across all supported scenarios.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ⚠️ Legal Disclaimer

**IMPORTANT: This software is provided for informational and educational purposes only.**

While every effort has been made to ensure accuracy and compliance with SARS regulations, the author makes **no warranties or guarantees** regarding the correctness, completeness, or reliability of the calculations. 

**Users are solely responsible for:**
- Verifying all calculations independently
- Ensuring compliance with current tax regulations
- Consulting with qualified tax professionals for official tax advice

**The author shall not be held liable for any errors, omissions, or damages arising from the use of this software.**

This package is provided under the MIT License for educational and development purposes. Always consult with qualified tax professionals and verify calculations with official SARS resources before making financial decisions.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For issues and feature requests, please use the [GitHub issues](https://github.com/JacquesBronk/SARS.TaxCalculator/issues) page.

## Author

**Jacques Bronkhorst** - [GitHub](https://github.com/JacquesBronk)