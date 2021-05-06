using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Dicom.Anonymization.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace Dicom.Anonymization.AnonymizationConfigurations
{
    [DataContract]
    public class AnonymizationConfiguration
    {
        [DataMember(Name = "rules")]
        public Dictionary<string, object>[] DicomTagRules { get; set; }

        [DataMember(Name = "defaultSettings")]
        public AnonymizationDefaultSettings DefaultSettings { get; set; }

        [DataMember(Name = "customizedSettings")]
        public Dictionary<string, object> CustomizedSettings { get; set; }

        public void Validate(AnonymizationConfiguration config)
        {
            if (config.DicomTagRules == null)
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.MissingConfigurationFields, "The configuration is invalid, please specify any dicom rules");
            }

            var supportedMethods = Enum.GetNames(typeof(AnonymizationMethod)).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            foreach (var rule in config.DicomTagRules)
            {
                if (!rule.ContainsKey(Constants.TagKey) || !rule.ContainsKey(Constants.MethodKey))
                {
                    throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.MissingConfigurationFields, "Missing tag or method in dicom rule config.");
                }

                // Todo Grammar check on DICOM tag?

                // Method validate
                string method = rule[Constants.MethodKey].ToString();
                if (!supportedMethods.Contains(method))
                {
                    throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.UnsupportedAnonymizationRule, $"Anonymization method {method} not supported.");
                }

                
            }
        }
    }
}
