using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dicom.Anonymization.AnonymizerConfigurations
{
    public class AnonymizerRule
    {
        public DicomTag Tag { get; set; }

        public string Method { get; set; }

        public Dictionary<string, object> RuleSettings { get; set; }

        public AnonymizerRule(DicomTag tag, string method)
        {
            Tag = tag;
            Method = method;
        }
    }
}
