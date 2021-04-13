using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    [DataContract]
    public class TagRule
    {
        [DataMember(Name = "tag")]
        public string DicomTagValue { get; set; }

        [DataMember(Name = "VR")]
        public string DicomTagVR { get; set; }

        [DataMember(Name = "params")]
        public string RuleParameters { get; set; }

        [DataMember(Name = "setting")]
        public string RuleSetting { get; set; }
    }
}
