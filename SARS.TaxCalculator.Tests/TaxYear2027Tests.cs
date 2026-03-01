using System;
using System.Linq;
using Xunit;
using SARS.TaxCalculator;
using SARS.TaxCalculator.Calculators;
using SARS.TaxCalculator.Configuration;
using SARS.TaxCalculator.Models.Inputs;

namespace SARS.TaxCalculator.Tests;

/// <summary>
/// Tests for 2027 tax year (1 March 2026 - 28 February 2027)
/// Source: 2026 Budget Speech (25 February 2026)
/// Reference: https://www.sars.gov.za/tax-rates/income-tax/rates-of-tax-for-individuals/
/// </summary>
public class TaxYear2027Tests
{
    #region Configuration Tests

    [Fact]
    public void ForTaxYear2027_ValidConfiguration_ReturnsBuilder()
    {
        var builder = TaxCalculator.ForTaxYear(2027);
        Assert.NotNull(builder);
    }

    [Fact]
    public void TaxYear2027Configuration_ValidValues_CorrectDates()
    {
        var config = TaxYearData.GetConfiguration(2027);

        Assert.Equal(2027, config.Year);
        Assert.Equal(new DateTime(2026, 3, 1), config.StartDate);
        Assert.Equal(new DateTime(2027, 2, 28), config.EndDate);
    }

    [Fact]
    public void TaxYear2027_SupportedYears_Includes2027()
    {
        Assert.Contains(2027, TaxCalculator.SupportedYears);
    }

    #endregion

    #region Tax Bracket Tests

    [Fact]
    public void TaxYear2027_TaxBrackets_HasSevenBrackets()
    {
        var config = TaxYearData.GetConfiguration(2027);
        Assert.Equal(7, config.TaxBrackets.Count);
    }

    [Theory]
    [InlineData(0, 245100, 0, 18)]
    [InlineData(245101, 383100, 44118, 26)]
    [InlineData(383101, 530200, 79998, 31)]
    [InlineData(530201, 695800, 125599, 36)]
    [InlineData(695801, 887000, 185215, 39)]
    [InlineData(887001, 1878600, 259783, 41)]
    public void TaxYear2027_TaxBrackets_CorrectValues(
        int minIncome, int maxIncome, int baseTax, int rate)
    {
        var config = TaxYearData.GetConfiguration(2027);
        var bracket = config.TaxBrackets.First(b => b.MinIncome == minIncome);

        Assert.Equal((decimal)maxIncome, bracket.MaxIncome);
        Assert.Equal((decimal)baseTax, bracket.BaseTax);
        Assert.Equal((decimal)rate, bracket.Rate);
    }

    [Fact]
    public void TaxYear2027_TopBracket_NoMaxIncome()
    {
        var config = TaxYearData.GetConfiguration(2027);
        var topBracket = config.TaxBrackets.Last();

        Assert.Equal(1878601, topBracket.MinIncome);
        Assert.Null(topBracket.MaxIncome);
        Assert.Equal(666339, topBracket.BaseTax);
        Assert.Equal(45, topBracket.Rate);
    }

    [Fact]
    public void TaxYear2027_Brackets_DifferFromPreviousYear()
    {
        var config2026 = TaxYearData.GetConfiguration(2026);
        var config2027 = TaxYearData.GetConfiguration(2027);

        // Brackets were adjusted for inflation - they should differ
        Assert.NotEqual(config2026.TaxBrackets[0].MaxIncome, config2027.TaxBrackets[0].MaxIncome);
        Assert.Equal(237100m, config2026.TaxBrackets[0].MaxIncome); // 2026 value
        Assert.Equal(245100m, config2027.TaxBrackets[0].MaxIncome); // 2027 value
    }

    #endregion

    #region Tax Rebate Tests

    [Fact]
    public void TaxYear2027_PrimaryRebate_Is17820()
    {
        var config = TaxYearData.GetConfiguration(2027);
        var primary = config.TaxRebates.First(r => r.Type == Models.RebateType.Primary);

        Assert.Equal(17820, primary.Amount);
        Assert.Null(primary.MinAge);
    }

    [Fact]
    public void TaxYear2027_SecondaryRebate_Is9765()
    {
        var config = TaxYearData.GetConfiguration(2027);
        var secondary = config.TaxRebates.First(r => r.Type == Models.RebateType.Secondary);

        Assert.Equal(9765, secondary.Amount);
        Assert.Equal(65, secondary.MinAge);
    }

    [Fact]
    public void TaxYear2027_TertiaryRebate_Is3249()
    {
        var config = TaxYearData.GetConfiguration(2027);
        var tertiary = config.TaxRebates.First(r => r.Type == Models.RebateType.Tertiary);

        Assert.Equal(3249, tertiary.Amount);
        Assert.Equal(75, tertiary.MinAge);
    }

    [Fact]
    public void TaxYear2027_Rebates_IncreasedFromPreviousYear()
    {
        var config2026 = TaxYearData.GetConfiguration(2026);
        var config2027 = TaxYearData.GetConfiguration(2027);

        // Primary: R17,235 -> R17,820
        Assert.Equal(17235, config2026.TaxRebates[0].Amount);
        Assert.Equal(17820, config2027.TaxRebates[0].Amount);

        // Secondary: R9,444 -> R9,765
        Assert.Equal(9444, config2026.TaxRebates[1].Amount);
        Assert.Equal(9765, config2027.TaxRebates[1].Amount);

        // Tertiary: R3,145 -> R3,249
        Assert.Equal(3145, config2026.TaxRebates[2].Amount);
        Assert.Equal(3249, config2027.TaxRebates[2].Amount);
    }

    #endregion

    #region Tax Threshold Tests

    [Fact]
    public void TaxYear2027_ThresholdUnder65_Is99000()
    {
        var config = TaxYearData.GetConfiguration(2027);
        var threshold = config.TaxThresholds.First(t => t.AppliesTo(30));

        Assert.Equal(99000, threshold.Amount);
    }

    [Fact]
    public void TaxYear2027_Threshold65To74_Is153250()
    {
        var config = TaxYearData.GetConfiguration(2027);
        var threshold = config.TaxThresholds.First(t => t.AppliesTo(65));

        Assert.Equal(153250, threshold.Amount);
    }

    [Fact]
    public void TaxYear2027_Threshold75Plus_Is171300()
    {
        var config = TaxYearData.GetConfiguration(2027);
        var threshold = config.TaxThresholds.First(t => t.AppliesTo(75));

        Assert.Equal(171300, threshold.Amount);
    }

    [Fact]
    public void TaxYear2027_Thresholds_IncreasedFromPreviousYear()
    {
        var config2026 = TaxYearData.GetConfiguration(2026);
        var config2027 = TaxYearData.GetConfiguration(2027);

        // Under 65: R95,750 -> R99,000
        Assert.Equal(95750, config2026.TaxThresholds[0].Amount);
        Assert.Equal(99000, config2027.TaxThresholds[0].Amount);

        // 65-74: R148,217 -> R153,250
        Assert.Equal(148217, config2026.TaxThresholds[1].Amount);
        Assert.Equal(153250, config2027.TaxThresholds[1].Amount);

        // 75+: R165,689 -> R171,300
        Assert.Equal(165689, config2026.TaxThresholds[2].Amount);
        Assert.Equal(171300, config2027.TaxThresholds[2].Amount);
    }

    [Fact]
    public void TaxYear2027_Thresholds_DerivedFromRebates()
    {
        var config = TaxYearData.GetConfiguration(2027);
        var primary = config.TaxRebates.First(r => r.Type == Models.RebateType.Primary).Amount;
        var secondary = config.TaxRebates.First(r => r.Type == Models.RebateType.Secondary).Amount;
        var tertiary = config.TaxRebates.First(r => r.Type == Models.RebateType.Tertiary).Amount;

        // Thresholds = total rebates / lowest rate (18%)
        Assert.Equal(primary / 0.18m, config.TaxThresholds[0].Amount);
        Assert.Equal((primary + secondary) / 0.18m, config.TaxThresholds[1].Amount);
        Assert.Equal((primary + secondary + tertiary) / 0.18m, config.TaxThresholds[2].Amount);
    }

    #endregion

    #region Medical Aid Credit Tests

    [Fact]
    public void TaxYear2027_MedicalAidCredit_MainMember376()
    {
        var config = TaxYearData.GetConfiguration(2027);
        Assert.Equal(376, config.MedicalAidCredit.MainMemberCredit);
    }

    [Fact]
    public void TaxYear2027_MedicalAidCredit_FirstDependent376()
    {
        var config = TaxYearData.GetConfiguration(2027);
        Assert.Equal(376, config.MedicalAidCredit.FirstDependentCredit);
    }

    [Fact]
    public void TaxYear2027_MedicalAidCredit_AdditionalDependent254()
    {
        var config = TaxYearData.GetConfiguration(2027);
        Assert.Equal(254, config.MedicalAidCredit.AdditionalDependentCredit);
    }

    [Fact]
    public void TaxYear2027_MedicalAidCredit_IncreasedFromPreviousYear()
    {
        var config2026 = TaxYearData.GetConfiguration(2026);
        var config2027 = TaxYearData.GetConfiguration(2027);

        // R364 -> R376
        Assert.Equal(364, config2026.MedicalAidCredit.MainMemberCredit);
        Assert.Equal(376, config2027.MedicalAidCredit.MainMemberCredit);

        // R364 -> R376
        Assert.Equal(364, config2026.MedicalAidCredit.FirstDependentCredit);
        Assert.Equal(376, config2027.MedicalAidCredit.FirstDependentCredit);

        // R246 -> R254
        Assert.Equal(246, config2026.MedicalAidCredit.AdditionalDependentCredit);
        Assert.Equal(254, config2027.MedicalAidCredit.AdditionalDependentCredit);
    }

    [Theory]
    [InlineData(0, 376)]        // Main member only
    [InlineData(1, 752)]        // Main + 1 dependent (R376 + R376)
    [InlineData(2, 1006)]       // Main + 2 dependents (R376 + R376 + R254)
    [InlineData(3, 1260)]       // Main + 3 dependents (R376 + R376 + R254 + R254)
    public void TaxYear2027_MedicalAidCredit_CalculatesCorrectly(int dependents, decimal expectedMonthly)
    {
        var config = TaxYearData.GetConfiguration(2027);
        var monthlyCredit = config.MedicalAidCredit.CalculateMonthlyCredit(dependents);
        Assert.Equal(expectedMonthly, monthlyCredit);
    }

    #endregion

    #region Retirement Limits Tests

    [Fact]
    public void TaxYear2027_RetirementLimits_AnnualCapIs430000()
    {
        var config = TaxYearData.GetConfiguration(2027);
        Assert.Equal(430000, config.RetirementLimits.AnnualCap);
    }

    [Fact]
    public void TaxYear2027_RetirementLimits_MaxPercentageUnchanged()
    {
        var config = TaxYearData.GetConfiguration(2027);
        Assert.Equal(0.275m, config.RetirementLimits.MaxPercentage);
    }

    [Fact]
    public void TaxYear2027_RetirementLimits_CapIncreasedFromPreviousYear()
    {
        var config2026 = TaxYearData.GetConfiguration(2026);
        var config2027 = TaxYearData.GetConfiguration(2027);

        // R350,000 -> R430,000
        Assert.Equal(350000, config2026.RetirementLimits.AnnualCap);
        Assert.Equal(430000, config2027.RetirementLimits.AnnualCap);
    }

    #endregion

    #region UIF Tests

    [Fact]
    public void TaxYear2027_UifConfig_UnchangedFromPreviousYear()
    {
        var config2026 = TaxYearData.GetConfiguration(2026);
        var config2027 = TaxYearData.GetConfiguration(2027);

        Assert.Equal(config2026.UifConfig.EmployeeRate, config2027.UifConfig.EmployeeRate);
        Assert.Equal(config2026.UifConfig.EmployerRate, config2027.UifConfig.EmployerRate);
        Assert.Equal(config2026.UifConfig.MonthlyCeiling, config2027.UifConfig.MonthlyCeiling);
    }

    [Fact]
    public void TaxYear2027_UifConfig_CorrectValues()
    {
        var config = TaxYearData.GetConfiguration(2027);

        Assert.Equal(0.01m, config.UifConfig.EmployeeRate);
        Assert.Equal(0.01m, config.UifConfig.EmployerRate);
        Assert.Equal(17712, config.UifConfig.MonthlyCeiling);
    }

    #endregion

    #region SDL Tests

    [Fact]
    public void TaxYear2027_SdlConfig_UnchangedFromPreviousYear()
    {
        var config2026 = TaxYearData.GetConfiguration(2026);
        var config2027 = TaxYearData.GetConfiguration(2027);

        Assert.Equal(config2026.SdlConfig.Rate, config2027.SdlConfig.Rate);
        Assert.Equal(config2026.SdlConfig.ExemptionThreshold, config2027.SdlConfig.ExemptionThreshold);
    }

    [Fact]
    public void TaxYear2027_SdlConfig_CorrectValues()
    {
        var config = TaxYearData.GetConfiguration(2027);

        Assert.Equal(0.01m, config.SdlConfig.Rate);
        Assert.Equal(500000, config.SdlConfig.ExemptionThreshold);
    }

    #endregion

    #region ETI Tests

    [Fact]
    public void TaxYear2027_EtiConfig_SameAs2026()
    {
        var config2026 = TaxYearData.GetConfiguration(2026);
        var config2027 = TaxYearData.GetConfiguration(2027);

        Assert.Equal(config2026.EtiConfig.MinAge, config2027.EtiConfig.MinAge);
        Assert.Equal(config2026.EtiConfig.MaxAge, config2027.EtiConfig.MaxAge);
        Assert.Equal(config2026.EtiConfig.MaxQualifyingSalary, config2027.EtiConfig.MaxQualifyingSalary);
        Assert.Equal(config2026.EtiConfig.Bands.Count, config2027.EtiConfig.Bands.Count);

        for (int i = 0; i < config2026.EtiConfig.Bands.Count; i++)
        {
            Assert.Equal(config2026.EtiConfig.Bands[i].MinSalary, config2027.EtiConfig.Bands[i].MinSalary);
            Assert.Equal(config2026.EtiConfig.Bands[i].MaxSalary, config2027.EtiConfig.Bands[i].MaxSalary);
            Assert.Equal(config2026.EtiConfig.Bands[i].FirstYearAmount, config2027.EtiConfig.Bands[i].FirstYearAmount);
            Assert.Equal(config2026.EtiConfig.Bands[i].SecondYearAmount, config2027.EtiConfig.Bands[i].SecondYearAmount);
        }
    }

    [Fact]
    public void TaxYear2027_EtiConfig_CorrectValues()
    {
        var config = TaxYearData.GetConfiguration(2027);

        Assert.Equal(18, config.EtiConfig.MinAge);
        Assert.Equal(29, config.EtiConfig.MaxAge);
        Assert.Equal(7500, config.EtiConfig.MaxQualifyingSalary);
        Assert.Equal(4, config.EtiConfig.Bands.Count);
    }

    #endregion

    #region PAYE Calculation Tests

    [Fact]
    public void TaxYear2027_BasicCalculation_ReturnsExpectedResult()
    {
        var result = TaxCalculator
            .ForTaxYear(2027)
            .WithGrossSalary(25000)
            .WithAge(35)
            .WithMedicalAid(2, 3500)
            .WithRetirementContribution(0.075m)
            .Calculate();

        Assert.Equal(25000, result.GrossSalary);
        Assert.Equal(35, result.Age);
        Assert.Equal(2027, result.TaxYear);
        Assert.True(result.NetSalary > 0);
        Assert.True(result.PAYE > 0);
        Assert.Equal(177.12m, result.UIF); // Capped at ceiling
        Assert.Equal(1875m, result.RetirementContribution); // 7.5% of 25000
        Assert.Equal(3500, result.MedicalAidContribution);
        Assert.Equal(752m, result.MedicalAidTaxCredit); // R376 + R376 for 2 members
    }

    [Fact]
    public void TaxYear2027_IncomeBelow99000_NoPaye()
    {
        // R99,000 annual = R8,250 monthly - below threshold, no PAYE
        var result = TaxCalculator
            .ForTaxYear(2027)
            .WithGrossSalary(8000)
            .WithAge(30)
            .Calculate();

        Assert.Equal(0, result.PAYE);
    }

    [Fact]
    public void TaxYear2027_IncomeAtThreshold_NoPaye()
    {
        // Exactly at threshold R99,000 annual = R8,250 monthly
        var result = TaxCalculator
            .ForTaxYear(2027)
            .WithAnnualGrossSalary(99000)
            .WithAge(30)
            .Calculate();

        Assert.Equal(0, result.PAYE);
    }

    [Fact]
    public void TaxYear2027_SeniorThreshold65_NoPaye()
    {
        // R153,250 annual threshold for 65+
        var result = TaxCalculator
            .ForTaxYear(2027)
            .WithAnnualGrossSalary(153250)
            .WithAge(65)
            .Calculate();

        Assert.Equal(0, result.PAYE);
    }

    [Fact]
    public void TaxYear2027_SeniorThreshold75_NoPaye()
    {
        // R171,300 annual threshold for 75+
        var result = TaxCalculator
            .ForTaxYear(2027)
            .WithAnnualGrossSalary(171300)
            .WithAge(75)
            .Calculate();

        Assert.Equal(0, result.PAYE);
    }

    [Fact]
    public void TaxYear2027_LowerPaye_DueToBracketAdjustment()
    {
        // Same salary should produce lower PAYE in 2027 due to inflation adjustment
        var result2026 = TaxCalculator
            .ForTaxYear(2026)
            .WithGrossSalary(30000)
            .WithAge(35)
            .Calculate();

        var result2027 = TaxCalculator
            .ForTaxYear(2027)
            .WithGrossSalary(30000)
            .WithAge(35)
            .Calculate();

        Assert.True(result2027.PAYE < result2026.PAYE,
            $"2027 PAYE (R{result2027.PAYE}) should be less than 2026 PAYE (R{result2026.PAYE}) due to bracket adjustment");
    }

    [Fact]
    public void TaxYear2027_HigherNetSalary_DueToBracketAdjustment()
    {
        // Same salary should produce higher net in 2027
        var result2026 = TaxCalculator
            .ForTaxYear(2026)
            .WithGrossSalary(30000)
            .WithAge(35)
            .Calculate();

        var result2027 = TaxCalculator
            .ForTaxYear(2027)
            .WithGrossSalary(30000)
            .WithAge(35)
            .Calculate();

        Assert.True(result2027.NetSalary > result2026.NetSalary,
            $"2027 Net (R{result2027.NetSalary}) should be more than 2026 Net (R{result2026.NetSalary})");
    }

    [Fact]
    public void TaxYear2027_FirstBracket_CalculatesCorrectly()
    {
        // R200,000 annual is in first bracket: 18% of R200,000 = R36,000 gross tax
        // Less primary rebate R17,820 = R18,180 annual PAYE
        var result = TaxCalculator
            .ForTaxYear(2027)
            .WithAnnualGrossSalary(200000)
            .WithAge(30)
            .Calculate();

        Assert.Equal(18180, result.AnnualPAYE);
        Assert.Equal(1515m, result.PAYE); // R18,180 / 12
    }

    [Fact]
    public void TaxYear2027_SecondBracket_CalculatesCorrectly()
    {
        // R300,000 annual is in second bracket:
        // SARS formula: R44,118 + 26% of (R300,000 - R245,100) = R44,118 + R14,274 = R58,392
        // Less primary rebate R17,820 = R40,572 annual PAYE
        var result = TaxCalculator
            .ForTaxYear(2027)
            .WithAnnualGrossSalary(300000)
            .WithAge(30)
            .Calculate();

        Assert.Equal(40572m, result.AnnualPAYE);
    }

    [Fact]
    public void TaxYear2027_TopBracket_CalculatesCorrectly()
    {
        // R2,000,000 annual is in top bracket:
        // SARS formula: R666,339 + 45% of (R2,000,000 - R1,878,600) = R666,339 + R54,630 = R720,969
        // Less primary rebate R17,820 = R703,149 annual PAYE
        var result = TaxCalculator
            .ForTaxYear(2027)
            .WithAnnualGrossSalary(2000000)
            .WithAge(30)
            .Calculate();

        Assert.Equal(703149m, result.AnnualPAYE);
    }

    [Fact]
    public void TaxYear2027_WithMedicalAid_ReducesPaye()
    {
        var withoutMedical = TaxCalculator
            .ForTaxYear(2027)
            .WithGrossSalary(30000)
            .WithAge(35)
            .Calculate();

        var withMedical = TaxCalculator
            .ForTaxYear(2027)
            .WithGrossSalary(30000)
            .WithAge(35)
            .WithMedicalAid(3, 3500)
            .Calculate();

        Assert.True(withMedical.PAYE < withoutMedical.PAYE);
        Assert.Equal(1006m, withMedical.MedicalAidTaxCredit); // R376 + R376 + R254
    }

    [Fact]
    public void TaxYear2027_WithRetirement_ReducesPaye()
    {
        var withoutRetirement = TaxCalculator
            .ForTaxYear(2027)
            .WithGrossSalary(30000)
            .WithAge(35)
            .Calculate();

        var withRetirement = TaxCalculator
            .ForTaxYear(2027)
            .WithGrossSalary(30000)
            .WithAge(35)
            .WithRetirementContribution(0.10m)
            .Calculate();

        Assert.True(withRetirement.PAYE < withoutRetirement.PAYE);
    }

    [Fact]
    public void TaxYear2027_RetirementCap_AppliesNewLimit()
    {
        // Annual salary of R2,000,000, 27.5% = R550,000 but capped at R430,000
        // The cap is applied internally in PAYE calculation, producing lower tax
        // compared to a scenario without retirement
        var withRetirement = TaxCalculator
            .ForTaxYear(2027)
            .WithAnnualGrossSalary(2000000)
            .WithAge(35)
            .WithRetirementContribution(0.275m)
            .CalculatePaye();

        var withoutRetirement = TaxCalculator
            .ForTaxYear(2027)
            .WithAnnualGrossSalary(2000000)
            .WithAge(35)
            .CalculatePaye();

        // Retirement deduction should reduce PAYE
        Assert.True(withRetirement.AnnualPAYE < withoutRetirement.AnnualPAYE);

        // Verify the cap config is R430,000
        var config = TaxYearData.GetConfiguration(2027);
        Assert.Equal(430000, config.RetirementLimits.AnnualCap);
    }

    [Fact]
    public void TaxYear2027_RetirementCap_DiffersFrom2026()
    {
        // Same salary, same contribution % - 2027 allows higher deduction due to R430k cap
        // This means 2027 PAYE should be lower than 2026 PAYE
        var result2026 = TaxCalculator
            .ForTaxYear(2026)
            .WithAnnualGrossSalary(2000000)
            .WithAge(35)
            .WithRetirementContribution(0.275m)
            .CalculatePaye();

        var result2027 = TaxCalculator
            .ForTaxYear(2027)
            .WithAnnualGrossSalary(2000000)
            .WithAge(35)
            .WithRetirementContribution(0.275m)
            .CalculatePaye();

        // 2027 PAYE should be lower due to both higher retirement cap AND inflation-adjusted brackets
        Assert.True(result2027.AnnualPAYE < result2026.AnnualPAYE,
            $"2027 PAYE (R{result2027.AnnualPAYE}) should be less than 2026 PAYE (R{result2026.AnnualPAYE})");
    }

    #endregion

    #region ETI Calculation Tests

    [Fact]
    public void TaxYear2027_EtiCalculation_EligibleEmployee()
    {
        var result = TaxCalculator
            .ForTaxYear(2027)
            .WithGrossSalary(3500)
            .WithAge(22)
            .WithEtiDetails(employmentMonths: 6, isFirstTime: true)
            .Calculate();

        Assert.True(result.ETI > 0);
    }

    [Fact]
    public void TaxYear2027_EtiCalculation_SameAs2026ForSameInput()
    {
        var result2026 = TaxCalculator
            .ForTaxYear(2026)
            .WithGrossSalary(3500)
            .WithAge(22)
            .WithEtiDetails(employmentMonths: 6, isFirstTime: true)
            .Calculate();

        var result2027 = TaxCalculator
            .ForTaxYear(2027)
            .WithGrossSalary(3500)
            .WithAge(22)
            .WithEtiDetails(employmentMonths: 6, isFirstTime: true)
            .Calculate();

        Assert.Equal(result2026.ETI, result2027.ETI);
    }

    #endregion

    #region Payslip Calculation Tests

    [Fact]
    public void TaxYear2027_Payslip_CalculatesCorrectly()
    {
        var config = TaxYearData.GetConfiguration(2027);
        var calculator = new PayslipCalculator(config);

        var input = new PayslipInput
        {
            EmployeeId = "EMP001",
            EmployeeName = "Test Employee",
            Age = 35,
            GrossSalary = 35000,
            MedicalAidMembers = 2,
            MedicalAidContribution = 3000,
            RetirementContribution = 3500,
            CompanyAnnualPayroll = 5000000
        };

        var payslip = calculator.Calculate(input);

        Assert.True(payslip.Summary.NetPay > 0);
        Assert.True(payslip.Deductions.PAYE > 0);
        Assert.Equal(177.12m, payslip.Deductions.UIF); // Salary above ceiling
        Assert.Equal(350m, payslip.EmployerContributions.SDL); // 1% of R35,000
    }

    [Fact]
    public void TaxYear2027_Payslip_MedicalAidCredit_UsesNewRates()
    {
        var config = TaxYearData.GetConfiguration(2027);
        var calculator = new PayslipCalculator(config);

        var input = new PayslipInput
        {
            EmployeeId = "EMP001",
            EmployeeName = "Test Employee",
            Age = 35,
            GrossSalary = 35000,
            MedicalAidMembers = 3, // Main + 2 dependents
            CompanyAnnualPayroll = 1000000
        };

        var payslip = calculator.Calculate(input);

        // R376 (main) + R376 (first dep) + R254 (additional) = R1,006
        Assert.Equal(1006m, payslip.Deductions.MedicalAidTaxCredit);
    }

    #endregion

    #region Age Group Comparison Tests

    [Theory]
    [InlineData(30, 99000)]     // Under 65 threshold
    [InlineData(65, 153250)]    // 65-74 threshold
    [InlineData(75, 171300)]    // 75+ threshold
    public void TaxYear2027_IncomeAtThreshold_NoPaye_ByAge(int age, decimal annualThreshold)
    {
        var result = TaxCalculator
            .ForTaxYear(2027)
            .WithAnnualGrossSalary(annualThreshold)
            .WithAge(age)
            .Calculate();

        Assert.Equal(0, result.PAYE);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(65)]
    [InlineData(75)]
    public void TaxYear2027_SeniorRebates_ReducePaye(int age)
    {
        var result = TaxCalculator
            .ForTaxYear(2027)
            .WithGrossSalary(50000)
            .WithAge(age)
            .Calculate();

        // Older taxpayers should pay less PAYE due to additional rebates
        if (age >= 65)
        {
            var resultYounger = TaxCalculator
                .ForTaxYear(2027)
                .WithGrossSalary(50000)
                .WithAge(30)
                .Calculate();

            Assert.True(result.PAYE < resultYounger.PAYE,
                $"Age {age} PAYE (R{result.PAYE}) should be less than age 30 PAYE (R{resultYounger.PAYE})");
        }
    }

    #endregion
}
