using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    public class AnonymizationRule
    {
        public DicomTag Tag { get; set; }

        public string Method { get; set; }

        public Dictionary<string, object> RuleSettings { get; set; }

        public AnonymizationRule(DicomTag tag, string method)
        {
            Tag = tag;
            Method = method;
        }
    }
}
