using Dicom;
using Dicom.Anonymization.AnonymizationConfigurations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;

namespace UnitTests.AnonymizationConfigurations
{
    public class AnonymizationDicomTagRuleUnitTests
    {
        private AnonymizationConfiguration _configuration;

        public AnonymizationDicomTagRuleUnitTests()
        {
            var content = File.ReadAllText("AnonymizationConfigurations/settings.json");
            JsonLoadSettings settings = new JsonLoadSettings
            {
                DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error,
            };
            var token = JToken.Parse(content, settings);
            _configuration = token.ToObject<AnonymizationConfiguration>();
        }


        public static IEnumerable<object[]> GetDicomConfigs()
        {
            yield return new object[] { new Dictionary<string, object>() { { "tag", "(0040,1001)" }, { "method", "redact" } }, null, new DicomTag(0x0040, 0x1001), null, "redact", false, false, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "00401001" }, { "method", "redact" } }, null, new DicomTag(0x0040, 0x1001), null, "redact", false, false, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "0040,1001" }, { "method", "redact" } }, null, new DicomTag(0x0040, 0x1001), null, "redact", false, false, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "DA" }, { "method", "dateshift" } }, DicomVR.DA, null, null, "dateshift", true, false, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "(0040,xx01)" }, { "method", "keep" } }, null, null, DicomMaskedTag.Parse("(0040,xx01)"), "keep", false, true, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "PatientName" }, { "method", "encrypt" } }, null, new DicomTag(0x0010, 0x0010), null, "encrypt", false, false, null };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "PatientName" }, { "method", "perturb" }, { "setting", "perturbCustomerSetting" } }, null, new DicomTag(0x0010, 0x0010), null, "perturb", false, false, new Dictionary<string, object>() { { "span", "1" }, { "roundTo", 2 }, { "rangeType", "Proportional" }, { "distribution", "Uniform" } } };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "PatientName" }, { "method", "perturb" }, { "params",   "{roundTo : 3}" } }, null, new DicomTag(0x0010, 0x0010), null, "perturb", false, false, new Dictionary<string, object>() { { "roundTo", 3 } } };
            yield return new object[] { new Dictionary<string, object>() { { "tag", "PatientName" }, { "method", "perturb" }, { "params", "{roundTo : 3}" }, { "setting", "perturbCustomerSetting" } }, null, new DicomTag(0x0010, 0x0010), null, "perturb", false, false, new Dictionary<string, object>() { { "span", "1" }, { "roundTo", 3 }, { "rangeType", "Proportional" }, { "distribution", "Uniform" } } };
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
        public void GivenADicomRule_WhenCreateDicomRule_DicomRuleShouldBeCreateCorrectly(Dictionary<string, object> config, DicomVR vr, DicomTag tag, DicomMaskedTag maskedTag, string method, bool isVRRule, bool isMasked, Dictionary<string, object> ruleSettings )
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
