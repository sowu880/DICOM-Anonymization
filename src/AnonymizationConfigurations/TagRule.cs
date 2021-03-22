using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Dicom.Anonymization.AnonymizerConfigurations
{
    [DataContract]
    public class TagRule
    {
        [DataMember(Name = "name")]
        public string DicomTagName { get; set; }

        [DataMember(Name = "value")]
        public string DicomTagValue { get; set; }

        [DataMember(Name = "VR")]
        public string DicomTagVR { get; set; }
    }
}
