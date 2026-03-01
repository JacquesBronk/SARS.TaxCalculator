# Changelog

All notable changes to this project will be documented in this file.

## [2027.1.1] - 2026-03-01

### Added
- **Date-Aware ETI Configuration**: The 2026 tax year now supports mid-year ETI rate changes
  - New `ForPaymentDate(month, year)` fluent API method for date-aware ETI resolution
  - New `GetEtiConfigForDate(month, year)` method on `TaxYearConfiguration`
  - New `DatedEtiConfiguration` class for specifying period-based ETI configs
  - New `EtiConfigPeriods` property on `TaxYearConfiguration` for multiple ETI periods per year
  - March 2025 payroll uses OLD ETI rates (max salary R6,500, fixed R1,500/R750 bands)
  - April 2025+ payroll uses NEW ETI rates (max salary R7,500, 60%/30% bands, R2,500/R1,250 S8 cap)
  - Backward compatible: omitting `ForPaymentDate()` uses the default (NEW) rates
- **UseRemunerationPercentage Flag**: Replaced hardcoded magic-number check in `EtiCalculator` with explicit `UseRemunerationPercentage` property on `EtiBand`
- **Comprehensive Test Coverage**: 416 tests, 100% line coverage, 98.5% branch coverage
  - 18 new date-aware ETI tests (config resolution, fluent API, payslip calculator, theory tests)
  - 25 new rounding edge case tests (ETI truncation, midpoint behavior, zero inputs)
  - Additional validation and edge case tests for SDL, payslip, and fluent API

### Changed
- `PayslipCalculator` now resolves ETI config per payslip based on `PayMonth`/`PayYear`
- `EtiCalculator` Band 1 percentage logic driven by `UseRemunerationPercentage` flag instead of salary range check

## [2027.1.0] - 2026-02-28

### Added
- **Tax Year 2027 Support**: Full support for tax year 2027 (1 March 2026 - 28 February 2027)
- **Inflation-Adjusted Tax Brackets**: All 7 income tax brackets adjusted for 3.4% inflation per 2026 Budget Speech
  - First bracket: R0 - R245,100 (was R237,100)
  - Top bracket: R1,878,601+ (was R1,817,001+)
  - Source: https://www.sars.gov.za/tax-rates/income-tax/rates-of-tax-for-individuals/
- **Updated Tax Rebates**: All three rebates adjusted for inflation
  - Primary: R17,820 (was R17,235)
  - Secondary (65+): R9,765 (was R9,444)
  - Tertiary (75+): R3,249 (was R3,145)
- **Updated Tax Thresholds**: Derived from updated rebates
  - Under 65: R99,000 (was R95,750)
  - 65-74: R153,250 (was R148,217)
  - 75+: R171,300 (was R165,689)
- **Updated Medical Aid Credits**: Adjusted for inflation
  - Main member / first dependent: R376/month (was R364)
  - Additional dependents: R254/month (was R246)
  - Source: https://www.sars.gov.za/tax-rates/medical-tax-credit-rates/
- **Increased Retirement Deduction Cap**: Annual cap raised from R350,000 to R430,000
  - Source: 2026 Budget Speech (https://www.gov.za/2026BudgetSpeech)
- **Comprehensive 2027 Tests**: New test file with full coverage of all 2027 tax year changes

### Unchanged for 2027
- UIF: R17,712 monthly ceiling, 1% employee/employer rates
- SDL: 1% rate, R500,000 annual exemption threshold
- ETI: Same rules as 2026 (April 2025 changes continue - R2,500 max, R7,500 salary threshold)
- Tax rate percentages: 18%, 26%, 31%, 36%, 39%, 41%, 45% (only bracket boundaries adjusted)

## [2026.1.1] - 2025-01-07

### Changed
- **ETI April 2025 Changes**: Implemented Employment Tax Incentive changes effective 1 April 2025
  - Maximum ETI amount increased from R2,000 to R2,500 for employees working 160+ hours
  - Maximum qualifying salary threshold increased from R6,500 to R7,500
  - Added hours worked tracking and proration for employees working less than 160 hours
  - Updated salary bands: R0-R2,499.99 (60%/30%), R2,500-R5,499.99 (fixed), R5,500-R7,499.99 (sliding scale)
  - Source: https://www.sars.gov.za/latest-news/employment-tax-incentive-eti-changes-with-effect-from-1-april-2025/

### Added
- **Hours Worked Field**: Added `HoursWorkedInMonth` property to `EtiEmployee` model for ETI proration
- **ETI Proration Logic**: ETI amounts are now prorated for employees working less than 160 hours per month
- **Enhanced ETI Calculator**: Updated to handle percentage-based calculations for Band 1 (R0-R2,499.99)

### Updated
- **Test Coverage**: Updated all ETI tests to reflect new calculation rules and values
- **Documentation**: Updated code comments with SARS ETI Guide (LAPD-ETI-G01) citations

### Added
- **Tax Year 2026 Support**: Added full support for tax year 2026 (1 March 2025 - 28 February 2026)
- **SARS Compliance Badge**: Added SARS compliance verification and badge to README
- **Comprehensive SARS Citations**: Added official SARS source citations throughout codebase
- **Enhanced Package Metadata**: Added copyright, title, summary, and release notes to NuGet package
- **Source Link Support**: Added Source Link for better debugging experience
- **PackageReadmeFile**: Included README.md in NuGet package per best practices
- **Compliance Documentation**: Added detailed compliance review document (SARS_COMPLIANCE_REVIEW.md)

### Changed
- **Updated Examples**: All examples now use tax year 2026 by default
- **Enhanced README**: Added comprehensive SARS compliance section with official citations
- **Package Description**: Updated to highlight SARS compliance and tax year coverage (2023-2026)
- **Package Tags**: Added compliance, legislation, and ETI-specific tags for better discoverability

### Fixed
- **ETI Rounding Compliance**: Fixed ETI amounts to truncate to whole Rands per SARS validation rules
- **Special Economic Zone**: Fixed age restriction exemption for SEZ employees  
- **PAYE Monthly vs Annual**: Fixed test to accommodate expected rounding differences
- **ETI Rounding Test**: Updated expectations to match SARS truncation requirements

### Documentation
- **Official SARS Sources**: Added citations to Fourth Schedule Income Tax Act, Employment Tax Incentive Act
- **Validation Rules**: Documented SARS rounding rules with official reference links
- **Compliance Matrix**: Added rounding strategy table showing compliance for each component
- **Best Practices**: Updated NuGet package metadata following 2025 best practices

### Technical
- **Version Bump**: Updated package version to 2026.1.1
- **Copyright Notice**: Added proper copyright information
- **Repository Metadata**: Enhanced repository URL and project information
- **Symbol Packages**: Maintained symbol package generation for debugging

## [2025.1.0] - Previous Release

### Initial Features
- PAYE calculation with age-based rebates
- UIF calculation with R17,712 monthly ceiling
- SDL calculation with R500,000 exemption threshold
- ETI calculation for qualifying employees
- Medical aid credits calculation
- Retirement contribution handling
- Complete payslip generation
- Bulk calculation support
- Fluent API interface
- Multi-year support (2023-2025)
- .NET Standard 2.1 compatibility

---

**Note**: This project follows [Semantic Versioning](https://semver.org/) principles.
