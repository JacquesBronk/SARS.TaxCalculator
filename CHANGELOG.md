# Changelog

All notable changes to this project will be documented in this file.

## [2025.1.1] - 2025-01-07

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
- **Version Bump**: Updated package version to 2025.1.1
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