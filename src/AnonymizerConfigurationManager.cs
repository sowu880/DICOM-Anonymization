using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dicom.Anonymization.AnonymizerConfigurations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dicom.Anonymization
{
    public sealed class AnonymizerConfigurationManager
    {
        private readonly AnonymizerConfiguration _configuration;

        public AnonymizationDicomTagRule[] DicomTagRules { get; private set; } = null;

        // public AnonymizerRule[] DicomVRRules { get; private set; } = null;

        public AnonymizerConfigurationManager(AnonymizerConfiguration configuration)
        {
            configuration.GenerateSettings();
            _configuration = configuration;
            DicomTagRules = _configuration.DicomTagRules.Select(entry => AnonymizationDicomTagRule.CreateAnonymizationDICOMRule(entry, _configuration.AllSettings)).ToArray();
            // DicomVRRules = _configuration.DicomVRRules.Select(entry => AnonymizationDicomTagRule.CreateAnonymizationFhirPathRule(entry)).ToArray();
        }

        public static AnonymizerConfigurationManager CreateFromSettingsInJson(string settingsInJson)
        {
            try
            {
                JsonLoadSettings settings = new JsonLoadSettings
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
                };
                var token = JToken.Parse(settingsInJson, settings);
                var configuration = token.ToObject<AnonymizerConfiguration>();
                return new AnonymizerConfigurationManager(configuration);
            }
            catch (JsonException innerException)
            {
                throw new JsonException($"Failed to parse configuration file", innerException);
            }
        }

        public static AnonymizerConfigurationManager CreateFromConfigurationFile(string configFilePath)
        {
            try
            {
                var content = File.ReadAllText(configFilePath);

                return CreateFromSettingsInJson(content);
            }
            catch (IOException innerException)
            {
                throw new IOException($"Failed to read configuration file {configFilePath}", innerException);
            }
        }

        public Dictionary<string, object> GetSettings()
        {
            return _configuration.AllSettings;
        }

    }
}
