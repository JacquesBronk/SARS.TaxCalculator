---
name: Bug Report
about: Create a report to help us improve tax calculation accuracy
title: '[BUG] '
labels: bug
assignees: JacquesBronk
---

## 🐛 Bug Description
A clear and concise description of what the bug is.

## 🔢 Tax Calculation Details
**Tax Year:** (e.g., 2024, 2025, 2026)
**Calculation Type:** (e.g., PAYE, UIF, SDL, ETI)
**Employee Details:**
- Age: 
- Gross Salary: 
- Medical Aid Members: 
- Other relevant details: 

## 📊 Expected vs Actual Results
**Expected Result:** What you expected to happen
**Actual Result:** What actually happened
**Difference:** Amount/percentage difference

## 🔁 Steps to Reproduce
```csharp
// Provide the exact code that produces the bug
var result = TaxCalculator
    .ForTaxYear(2026)
    .WithGrossSalary(25000)
    .WithAge(35)
    .Calculate();

// Expected: R X
// Actual: R Y
```

## 🏛️ SARS Compliance
**Official SARS Reference:** (link to relevant SARS documentation)
**Manual Calculation:** (if you've verified against SARS resources)

## 🖥️ Environment
- **Library Version:** (e.g., 2025.1.1)
- **Framework:** (e.g., .NET 8, .NET Framework 4.8)
- **OS:** (e.g., Windows 11, Ubuntu 22.04)

## 📎 Additional Context
Add any other context about the problem here, such as:
- Screenshots of SARS calculators
- Links to official tax tables
- Similar issues you've found

## ⚠️ Impact Assessment
- [ ] Critical - Affects tax compliance
- [ ] High - Incorrect calculations
- [ ] Medium - Performance or usability issue  
- [ ] Low - Minor inconvenience

---
**Note:** For tax calculation bugs, please include official SARS references to help verify the correct calculation method.