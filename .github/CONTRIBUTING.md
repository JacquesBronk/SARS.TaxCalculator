# Contributing to SARS.TaxCalculator

Thank you for your interest in contributing to the SARS Tax Calculator! This project helps South African businesses and developers with accurate tax calculations.

## 🚨 Important Notice

This library performs **critical tax calculations** that affect real financial decisions. All contributions must maintain the highest standards of accuracy and compliance with SARS legislation.

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022, VS Code, or JetBrains Rider
- Git for version control
- Basic understanding of South African tax legislation

### Setting Up Development Environment

1. **Fork the repository**
   ```bash
   # Clone your fork
   git clone https://github.com/YOUR_USERNAME/SARS.TaxCalculator.git
   cd SARS.TaxCalculator
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Run tests to ensure everything works**
   ```bash
   dotnet test
   ```

4. **Build the project**
   ```bash
   dotnet build
   ```

## Branch Protection Rules

The `main` branch is protected with the following rules:

### Required Status Checks
- ✅ **Build and Test** - All tests must pass
- ✅ **Code Quality Analysis** - No XML documentation warnings
- ✅ **Security Scan** - No security vulnerabilities
- ✅ **98%+ Test Coverage** - Maintain high test coverage

### Pull Request Requirements
- ✅ **Require pull request reviews** - At least 1 approval required
- ✅ **Require review from code owners** - [@JacquesBronk](https://github.com/JacquesBronk)
- ✅ **Dismiss stale reviews** - New commits dismiss old approvals
- ✅ **Require up-to-date branches** - Branch must be current with main
- ✅ **Require conversation resolution** - All comments must be resolved

### Restrictions
- ✅ **Restrict pushes** - Direct pushes to main are blocked
- ✅ **Include administrators** - Rules apply to all users
- ✅ **Allow force pushes** - Disabled for safety
- ✅ **Allow deletions** - Disabled to protect main branch

## Development Workflow

### 1. Create a Feature Branch
```bash
git checkout main
git pull origin main
git checkout -b feature/your-feature-name
```

### 2. Make Your Changes
- Follow existing code style and conventions
- Add comprehensive tests for new functionality
- Update XML documentation for all public members
- Verify SARS compliance for any tax calculation changes

### 3. Test Your Changes
```bash
# Run all tests
dotnet test

# Check test coverage (should be 98%+)
dotnet test --collect:"XPlat Code Coverage"

# Verify no XML documentation warnings
dotnet build --verbosity normal

# Check formatting
dotnet format --verify-no-changes
```

### 4. Commit Your Changes
```bash
git add .
git commit -m "feat: add new ETI calculation feature

- Implement new ETI rate for 2026
- Add comprehensive tests covering edge cases
- Update documentation with SARS citations
- Maintain 98%+ test coverage"
```

### 5. Push and Create Pull Request
```bash
git push origin feature/your-feature-name
```

Then create a Pull Request through GitHub's interface.

## Contribution Guidelines

### Code Standards

1. **SARS Compliance First**
   - All tax calculations must comply with current SARS legislation
   - Include official SARS citations for calculation methods
   - Test against official SARS examples where available

2. **Testing Requirements**
   - Minimum 98% line coverage
   - Test all edge cases and boundary conditions
   - Include performance tests for bulk operations
   - Add compliance verification tests

3. **Documentation Standards**
   - Comprehensive XML documentation for all public members
   - Include `<summary>`, `<remarks>`, `<example>`, and `<value>` tags
   - Document SARS-specific business rules in remarks
   - Provide practical examples in code samples

4. **Code Quality**
   - Follow existing naming conventions
   - Use meaningful variable and method names
   - Keep methods focused and single-purpose
   - Maintain consistent formatting

### Types of Contributions

#### 🐛 Bug Fixes
- Fix calculation errors or edge cases
- Improve error handling
- Address performance issues
- **Must include:** Test case demonstrating the bug and fix

#### ✨ New Features
- Add support for new tax years
- Implement new SARS requirements
- Add calculation optimizations
- **Must include:** Comprehensive tests and documentation

#### 📚 Documentation
- Improve XML documentation
- Update README or guides
- Add code examples
- **Must include:** Clear, accurate information

#### 🧪 Tests
- Add missing test coverage
- Improve test scenarios
- Add performance benchmarks
- **Must include:** Clear test descriptions and assertions

### SARS Compliance Requirements

When making changes that affect tax calculations:

1. **Research Phase**
   - Review current SARS legislation
   - Check official SARS websites for updates
   - Verify calculation methods against SARS examples

2. **Implementation Phase**
   - Include official SARS citations in code comments
   - Follow SARS rounding rules precisely
   - Handle all edge cases per SARS guidelines

3. **Testing Phase**
   - Create tests based on SARS examples
   - Test boundary conditions (age limits, salary caps, etc.)
   - Verify rounding behavior matches SARS requirements

4. **Documentation Phase**
   - Document business rules in XML comments
   - Include links to relevant SARS resources
   - Explain any SARS-specific behavior

## Review Process

### Pull Request Review Criteria

Reviewers will check:

1. ✅ **Functionality** - Does it work correctly?
2. ✅ **SARS Compliance** - Does it follow tax regulations?
3. ✅ **Test Coverage** - Are there comprehensive tests?
4. ✅ **Documentation** - Is it well documented?
5. ✅ **Performance** - Is it efficient?
6. ✅ **Security** - Are there any vulnerabilities?

### Getting Your PR Approved

1. **Address all feedback** - Respond to review comments
2. **Keep discussions focused** - Stay on topic
3. **Be responsive** - Reply to feedback promptly
4. **Test thoroughly** - Ensure all tests pass
5. **Update documentation** - Keep docs current

## Release Process

### Version Numbering
We use calendar versioning: `YYYY.MAJOR.MINOR`
- `YYYY` - Current year
- `MAJOR` - Major features or SARS regulation changes
- `MINOR` - Bug fixes and minor improvements

### Release Workflow
1. Create a release branch from main
2. Update version in project file
3. Update CHANGELOG.md
4. Create release tag
5. Automated workflows handle NuGet publishing

## Getting Help

### Resources
- 📖 [SARS Official Website](https://www.sars.gov.za)
- 📋 [Tax Rates for Individuals](https://www.sars.gov.za/tax-rates/income-tax/rates-of-tax-for-individuals/)
- 🏛️ [Employment Tax Incentive](https://www.sars.gov.za/types-of-tax/pay-as-you-earn/employment-tax-incentive-eti/)
- 📊 [Project Documentation](./README.md)

### Support Channels
- 🐛 [GitHub Issues](https://github.com/JacquesBronk/SARS.TaxCalculator/issues) - Bug reports and feature requests
- 💬 [GitHub Discussions](https://github.com/JacquesBronk/SARS.TaxCalculator/discussions) - Questions and general discussion

## Code of Conduct

### Our Standards
- **Professional** - Maintain professional communication
- **Respectful** - Respect different viewpoints and experiences
- **Constructive** - Provide helpful, actionable feedback
- **Accurate** - Prioritize accuracy in tax calculations
- **Collaborative** - Work together for the benefit of users

### Enforcement
Instances of abusive, harassing, or otherwise unacceptable behavior may be reported by contacting the project maintainer.

---

## ⚠️ Legal Disclaimer

**This software is provided for informational and educational purposes only. Contributors and users are responsible for verifying all calculations and ensuring compliance with current tax regulations. Always consult with qualified tax professionals for official tax advice.**

Thank you for contributing to SARS.TaxCalculator! 🇿🇦