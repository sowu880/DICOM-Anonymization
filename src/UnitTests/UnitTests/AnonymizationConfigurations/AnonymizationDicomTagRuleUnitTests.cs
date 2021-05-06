// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using De_Id_Function_Shared;
using Dicom;
using Dicom.Anonymization.AnonymizationConfigurations;
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Dicom.Anonymization.Model;
using Dicom.Anonymization.Processors.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace UnitTests.AnonymizationConfigurations
{
    public class AnonymizationDicomTagRuleUnitTests
    {
        private AnonymizationConfiguration _configuration;

        public AnonymizationDicomTagRuleUnitTests()
        {
            var content = File.ReadAllText("AnonymizationConfigurations/settings.json");
            _configuration = JsonConvert.DeserializeObject<AnonymizationConfiguration>(content);
        }

        public static IEnumerable<object[]> GetDicomConfigs()
        {
            yield return new object[] { new Dictionary<string, object>() { { "tag", "(0040,1001)" }, { "method", "redact" } }, null, new DicomTag(0x0040, 0x1001), null, "redact", false, false, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "00401001" }, { "method", "redact" } }, null, new DicomTag(0x0040, 0x1001), null, "redact", false, false, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "0040,1001" }, { "method", "redact" } }, null, new DicomTag(0x0040, 0x1001), null, "redact", false, false, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "DA" }, { "method", "dateshift" } }, DicomVR.DA, null, null, "dateshift", true, false, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "(0040,xx01)" }, { "method", "keep" } }, null, null, DicomMaskedTag.Parse("(0040,xx01)"), "keep", false, true, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "PatientName" }, { "method", "encrypt" } }, null, new DicomTag(0x0010, 0x0010), null, "encrypt", false, false, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "PatientName" }, { "method", "perturb" }, { "setting", "perturbCustomerSetting" } }, null, new DicomTag(0x0010, 0x0010), null, "perturb", false, false, new DicomPerturbSetting { Span = 1, RoundTo = 2, RangeType = PerturbRangeType.Fixed, Distribution = PerturbDistribution.Uniform } };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "PatientName" }, { "method", "perturb" }, { "params",   "{roundTo : 3}" } }, null, new DicomTag(0x0010, 0x0010), null, "perturb", false, false, new DicomPerturbSetting { Span = 1, RoundTo = 3, RangeType = PerturbRangeType.Proportional, Distribution = PerturbDistribution.Uniform } };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "PatientName" }, { "method", "perturb" }, { "params", "{roundTo : 3}" }, { "setting", "perturbCustomerSetting" } }, null, new DicomTag(0x0010, 0x0010), null, "perturb", false, false, new DicomPerturbSetting { Span = 1, RoundTo = 3, RangeType = PerturbRangeType.Fixed, Distribution = PerturbDistribution.Uniform } };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "PatientName" }, { "method", "perturb" }, { "params", "{roundTo : 3}" } }, null, new DicomTag(0x0010, 0x0010), null, "perturb", false, false, new DicomPerturbSetting { Span = 1, RoundTo = 3, RangeType = PerturbRangeType.Proportional, Distribution = PerturbDistribution.Uniform } };
        }

        public static IEnumerable<object[]> GetInvalidConfigs()
        {
            yield return new object[] { new Dictionary<string, object>() { { "tag", "(0040)" }, { "method", "redact" } } };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "DD" }, { "method", "dateshift" } } };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "(0040,xx01)" } } };
            yield return new object[] { new Dictionary<string, object>() { { "method", "encrypt" } } };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "PatientName" }, { "method", "perturb" }, { "setting", "CustomerSetting" } } };
        }

        [Theory]
        [MemberData(nameof(GetDicomConfigs))]
        public void GivenADicomRule_WhenCreateDicomRule_DicomRuleShouldBeCreateCorrectly(Dictionary<string, object> config, DicomVR vr, DicomTag tag, DicomMaskedTag maskedTag, string method, bool isVRRule, bool isMasked, IDicomAnonymizationSetting ruleSettings )
        {
            var rule = AnonymizationDicomTagRule.CreateAnonymizationDicomRule(config, _configuration);

            Assert.Equal(vr, rule.VR);
            Assert.Equal(tag, rule.Tag);
            Assert.Equal(maskedTag?.ToString(), rule.MaskedTag?.ToString());
            Assert.Equal(method, rule.Method);
            Assert.Equal(isVRRule, rule.IsVRRule);
            Assert.Equal(isMasked, rule.IsMasked);
            Assert.Equal(ruleSettings?.ToString(), rule.RuleSetting?.ToString());
        }

        [Theory]
        [MemberData(nameof(GetInvalidConfigs))]
        public void GivenAnInvalidDicomRule_WhenCreateDicomRule_ExceptionWillBeThrown(Dictionary<string, object> config)
        {
            Assert.Throws<AnonymizationConfigurationException>(() => AnonymizationDicomTagRule.CreateAnonymizationDicomRule(config, _configuration));
        }
    }
}
