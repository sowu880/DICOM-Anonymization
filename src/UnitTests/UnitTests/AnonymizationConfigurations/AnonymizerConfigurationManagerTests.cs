using Dicom.Anonymization;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Newtonsoft.Json;
using System.IO;

namespace UnitTests.AnonymizationConfigurations
{
    public class AnonymizerConfigurationManagerTests
    {
        public AnonymizerConfigurationManagerTests()
        {
        }

        public static IEnumerable<object[]> GetInvalidConfigsForRuleParsing()
        {
            //yield return new object[] { "./TestConfigurations/configuration-miss-rules.json" };
            //yield return new object[] { "./TestConfigurations/configuration-unsupported-method.json" };
            yield return new object[] { "./TestConfigurations/configuration-invalid-DicomTag.json" };
        }

        public static IEnumerable<object[]> GetInvalidConfigsForJsonParsing()
        {
            yield return new object[] { "./TestConfigurations/configuration-invalid-parameters.json" };
        }

        public static IEnumerable<object[]> GetValidConfigs()
        {
            yield return new object[] { "./TestConfigurations/configuration-test-sample.json" };
        }

        [Fact]
        public void GivenANotExistConfig_WhenCreateAnonymizerConfigurationManager_ExceptionShouldBeThrown()
        {
            string configFilePath = "notExist";
            Assert.Throws<IOException>(() => AnonymizationConfigurationManager.CreateFromConfigurationFile(configFilePath));
        }

        [Theory]
        [MemberData(nameof(GetInvalidConfigsForRuleParsing))]
        public void GivenAnInvalidConfigForRuleParsing_WhenCreateAnonymizerConfigurationManager_ExceptionShouldBeThrown(string configFilePath)
        {
            Assert.Throws<AnonymizationConfigurationException>(() => AnonymizationConfigurationManager.CreateFromConfigurationFile(configFilePath));
        }

        [Theory]
        [MemberData(nameof(GetInvalidConfigsForJsonParsing))]
        public void GivenAnInvalidConfigForJsonParsing_WhenCreateAnonymizerConfigurationManager_ExceptionShouldBeThrown(string configFilePath)
        {
            Assert.Throws<JsonException>(() => AnonymizationConfigurationManager.CreateFromConfigurationFile(configFilePath));
        }

        [Theory]
        [MemberData(nameof(GetValidConfigs))]
        public void GivenAValidConfig_WhenCreateAnonymizerConfigurationManager_ConfigurationShouldBeLoaded(string configFilePath)
        {
            var configurationManager = AnonymizationConfigurationManager.CreateFromConfigurationFile(configFilePath);
            var dicomRules = configurationManager.DicomTagRules;
            Assert.True(dicomRules.Any());

            Assert.Single(configurationManager.DicomTagRules.Where(r => "(0008,0050)".Equals(r.Tag?.ToString())));
            Assert.Single(configurationManager.DicomTagRules.Where(r => "DA".Equals(r.VR?.ToString())));

            var settings = configurationManager.GetDefaultSettings();
            Assert.True(settings != null);

        }

        [Theory]
        [InlineData("abc123")]
        [InlineData("foldername")]
        [InlineData("filename")]
        public void GivenADateShiftKeyPrefix_WhenSet_DateShiftKeyPrefixShouldBeSetCorrectly(string dateShiftKeyPrefix)
        {
            /*
            var configFilePath = "./TestConfigurations/configuration-test-sample.json";
            var configurationManager = AnonymizationConfigurationManager.CreateFromConfigurationFile(configFilePath);
            configurationManager.SetDateShiftKeyPrefix(dateShiftKeyPrefix);

            Assert.Equal(dateShiftKeyPrefix, configurationManager.GetParameterConfiguration().DateShiftKeyPrefix);
            */
        }
    }
}
