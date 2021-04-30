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


        public void GenerateSettings()
        {
            
        }
    }
}
