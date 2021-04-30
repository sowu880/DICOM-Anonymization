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
            DicomTagRules = _configuration.DicomTagRules?.Select(entry => AnonymizationDicomTagRule.CreateAnonymizationDicomRule(entry, _configuration)).ToArray();
            // DicomVRRules = _configuration.DicomVRRules.Select(entry => AnonymizationDicomTagRule.CreateAnonymizationFhirPathRule(entry)).ToArray();
        }

        public static AnonymizationConfigurationManager CreateFromSettingsInJson(string settingsInJson)
        {
            try
            {
                var configuration = JsonConvert.DeserializeObject<AnonymizationConfiguration>(settingsInJson);
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

        public AnonymizationDefaultSettings GetDefaultSettings()
        {
            return _configuration.DefaultSettings;
        }

    }
}
