using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    [DataContract]
    public class AnonymizationConfiguration
    {
        [DataMember(Name = "regenerateID")]
        public bool RegenerateID { get; set; }

        [DataMember(Name = "rules")]
        public Dictionary<string, object>[] DicomTagRules { get; set; }

        [DataMember(Name = "defaultSettings")]
        public Dictionary<string, object> DefaultSettings { get; set; }

        [DataMember(Name = "customizedSettings")]
        public Dictionary<string, object> CustomizedSettings { get; set; }

        public Dictionary<string, object> AllSettings { get; set; } = new Dictionary<string, object>() { };

        // Static default crypto hash key to provide a same default key for all engine instances
        private static readonly Lazy<string> s_defaultCryptoKey = new Lazy<string>(() => Guid.NewGuid().ToString("N"));

        public void GenerateSettings()
        {
            if (DefaultSettings != null)
            {
                AllSettings = DefaultSettings;
            }

            if (CustomizedSettings != null)
            {
                AllSettings = AllSettings.Concat(CustomizedSettings.Where(x => !DefaultSettings.Contains(x))).ToDictionary(s => s.Key, s => s.Value);
            }
        }
    }
}
