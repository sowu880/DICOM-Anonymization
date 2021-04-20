using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dicom.Anonymization.AnonymizationConfigurations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dicom.Anonymization
{
    public sealed class AnonymizationConfigurationManager
    {
        private readonly AnonymizationConfiguration _configuration;

        public AnonymizationDicomTagRule[] DicomTagRules { get; private set; } = null;

        // public AnonymizationRule[] DicomVRRules { get; private set; } = null;

        public AnonymizationConfigurationManager(AnonymizationConfiguration configuration)
        {
            configuration.GenerateSettings();
            _configuration = configuration;
            DicomTagRules = _configuration.DicomTagRules.Select(entry => AnonymizationDicomTagRule.CreateAnonymizationDICOMRule(entry, _configuration.AllSettings)).ToArray();
            // DicomVRRules = _configuration.DicomVRRules.Select(entry => AnonymizationDicomTagRule.CreateAnonymizationFhirPathRule(entry)).ToArray();
        }

        public static AnonymizationConfigurationManager CreateFromSettingsInJson(string settingsInJson)
        {
            try
            {
                JsonLoadSettings settings = new JsonLoadSettings
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
                };
                var token = JToken.Parse(settingsInJson, settings);
                var configuration = token.ToObject<AnonymizationConfiguration>();
                return new AnonymizationConfigurationManager(configuration);
            }
            catch (JsonException innerException)
            {
                throw new JsonException($"Failed to parse configuration file", innerException);
            }
        }

        public static AnonymizationConfigurationManager CreateFromConfigurationFile(string configFilePath)
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
