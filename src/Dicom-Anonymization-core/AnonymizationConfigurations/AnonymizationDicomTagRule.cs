using Dicom.Anonymization.Model;
using EnsureThat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    public class AnonymizationDicomTagRule
    {
        public DicomVR VR { get; set; }

        public DicomTag Tag { get; set; }

        public DicomMaskedTag MaskedTag { get; set; }

        public string Method { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public Dictionary<string, object> RuleSetting { get; set; }

        public bool IsVRRule { get; set; } = false;

        public bool IsMasked { get; set; } = false;

        public static AnonymizationDicomTagRule CreateAnonymizationDICOMRule(Dictionary<string, object> config, Dictionary<string, object> allSettings)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (!config.ContainsKey(Constants.MethodKey))
            {
                throw new ArgumentException("Missing method in rule config");
            }

            Dictionary<string, object> parameters = null;
            if (config.ContainsKey(Constants.Parameters))
            {
                parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(config[Constants.Parameters].ToString());
            }

            Dictionary<string, object> ruleSetting = null;
            if (config.ContainsKey(Constants.RuleSetting))
            {
                ruleSetting = JsonConvert.DeserializeObject<Dictionary<string, object>>(allSettings[config[Constants.RuleSetting].ToString()].ToString());
            }

            if (parameters != null)
            {
                ruleSetting = ruleSetting == null ? parameters : parameters.Concat(ruleSetting.Where(x => !parameters.ContainsKey(x.Key))).ToDictionary(s => s.Key, s => s.Value);
            }

            var method = config[Constants.MethodKey].ToString();

            if (config.ContainsKey(Constants.TagKey))
            {
                var content = config[Constants.TagKey].ToString();

                try
                {
                    var tag = DicomMaskedTag.Parse(content);
                    return new AnonymizationDicomTagRule(tag, method, ruleSetting);
                }
                catch (Exception ex)
                {
                    try
                    {
                        var tag = DicomTag.Parse(content);
                        return new AnonymizationDicomTagRule(tag, method, ruleSetting);
                    }
                    catch
                    {
                        try
                        {
                            var vr = DicomVR.Parse(content);
                            return new AnonymizationDicomTagRule(vr, method, ruleSetting);
                        }
                        catch
                        {
                            try
                            {
                                var dicomTags = new DicomTag(0, 0);
                                DicomTag tag = (DicomTag)dicomTags.GetType().GetField(content).GetValue(dicomTags);
                                return new AnonymizationDicomTagRule(tag, method, ruleSetting);
                            }
                            catch
                            {
                                throw new Exception("Failed to parse rules");
                            }
                        }
                    }
                }
            }

            throw new AnonymizationConfigurationErrorsException("Invaid tag in rule config");
        }

        public AnonymizationDicomTagRule(DicomTag tag, string method, Dictionary<string, object> ruleSetting)
        {
            EnsureArg.IsNotNull(tag, nameof(tag));
            EnsureArg.IsNotNull(method, nameof(method));

            Tag = tag;
            Method = method;
            RuleSetting = ruleSetting;
        }

        public AnonymizationDicomTagRule(DicomMaskedTag tag, string method, Dictionary<string, object> ruleSetting)
        {
            EnsureArg.IsNotNull(tag, nameof(tag));
            EnsureArg.IsNotNull(method, nameof(method));

            MaskedTag = tag;
            Method = method;
            RuleSetting = ruleSetting;
            IsMasked = true;
        }

        public AnonymizationDicomTagRule(DicomVR vr, string method, Dictionary<string, object> ruleSetting)
        {
            EnsureArg.IsNotNull(vr, nameof(vr));
            EnsureArg.IsNotNull(method, nameof(method));

            VR = vr;
            Method = method;
            RuleSetting = ruleSetting;
            IsVRRule = true;
        }
    }
}
