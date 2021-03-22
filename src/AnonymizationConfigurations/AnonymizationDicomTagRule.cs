using EnsureThat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Dicom.Anonymization.AnonymizerConfigurations
{
    public class AnonymizationDicomTagRule
    {
        public DicomVR VR { get; set; }

        public DicomTag Tag { get; set; }

        public string Method { get; set; }

        public Dictionary<string, object> RuleSettings { get; set; }

        public bool IsVRRule { get { return VR == null;  } }

        public static AnonymizationDicomTagRule CreateAnonymizationDICOMRule(Dictionary<string, object> config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (!config.ContainsKey(Constants.TagKey))
            {
                throw new ArgumentException("Missing tag in rule config");
            }

            if (!config.ContainsKey(Constants.MethodKey))
            {
                throw new ArgumentException("Missing method in rule config");
            }

            TagRule tagRule;
            try
            {
                tagRule = JsonConvert.DeserializeObject<TagRule>(config[Constants.TagKey].ToString());
            }
            catch (JsonException innerException)
            {
                throw new JsonException($"Failed to parse configuration file", innerException);
            }

            var method = config[Constants.MethodKey].ToString();

            if (!string.IsNullOrEmpty(tagRule.DicomTagValue))
            {
                var tag = DicomTag.Parse(tagRule.DicomTagValue);
                return new AnonymizationDicomTagRule(tag, method, config);
            }
            else if (!string.IsNullOrEmpty(tagRule.DicomTagVR))
            {
                var vr = DicomVR.Parse(tagRule.DicomTagVR.ToString());
                return new AnonymizationDicomTagRule(vr, method, config);
            }

            throw new AnonymizerConfigurationErrorsException("Invaid tag in rule config");
        }

        public AnonymizationDicomTagRule(DicomTag tag, string method, Dictionary<string, object> config)
        {
            EnsureArg.IsNotNull(tag, nameof(tag));
            EnsureArg.IsNotNull(method, nameof(method));

            Tag = tag;
            Method = method;
            RuleSettings = config;
        }

        public AnonymizationDicomTagRule(DicomVR vr, string method, Dictionary<string, object> config)
        {
            EnsureArg.IsNotNull(vr, nameof(vr));
            EnsureArg.IsNotNull(method, nameof(method));

            VR = vr;
            Method = method;
            RuleSettings = config;
        }
    }
}
