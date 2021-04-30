﻿using Dicom.Anonymization.Model;
using EnsureThat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Dicom.Anonymization.Processors.Settings;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    public class AnonymizationDicomTagRule
    {
        public DicomVR VR { get; set; }

        public DicomTag Tag { get; set; }

        public DicomMaskedTag MaskedTag { get; set; }

        public string Method { get; set; }

        public bool IsVRRule { get; set; } = false;

        public bool IsMasked { get; set; } = false;

        public IDicomAnonymizationSetting RuleSetting { get; set; }

        public static AnonymizationDicomTagRule CreateAnonymizationDicomRule(Dictionary<string, object> config, AnonymizationConfiguration configuration)
        {
            EnsureArg.IsNotNull(config, nameof(config));

            if (!config.ContainsKey(Constants.MethodKey))
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.MissingConfigurationFields, "Missing method in rule config");
            }

            Dictionary<string, object> parameters = null;
            if (config.ContainsKey(Constants.Parameters))
            {
                //parameters = JToken.Parse(config[Constants.Parameters].ToString()).ToObject<Dictionary<string, object>>();
                parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(config[Constants.Parameters].ToString());
                // parameters = (Dictionary<string, object>)config[Constants.Parameters];
            }

            var method = config[Constants.MethodKey].ToString();


            IDicomAnonymizationSetting ruleSetting = null;
            if (config.ContainsKey(Constants.RuleSetting))
            {
                if (configuration.CustomizedSettings == null || !configuration.CustomizedSettings.ContainsKey(config[Constants.RuleSetting].ToString()))
                {
                    throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.MissingConfigurationFields, $"Customized setting {config[Constants.RuleSetting]} not defined");
                }

                var settings = configuration.CustomizedSettings[config[Constants.RuleSetting].ToString()].ToString();
                if (parameters != null)
                {
                    settings = parameters.Concat(JsonConvert.DeserializeObject<Dictionary<string, object>>(settings).Where(x => !parameters.ContainsKey(x.Key))).ToDictionary(s => s.Key, s => s.Value).ToString();
                }

                ruleSetting = AnonymizationDefaultSettings.DicomSettingsMapping[method].CreateFromRuleSettings(settings);
            }
            else if (parameters != null)
            {
                ruleSetting = configuration.DefaultSettings.GetDefaultSetting(method);
                var defaultSetting = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(ruleSetting));
                var settings = parameters.Concat(defaultSetting.Where(x => !parameters.ContainsKey(x.Key))).ToDictionary(s => s.Key, s => s.Value).ToString();
                ruleSetting = AnonymizationDefaultSettings.DicomSettingsMapping[method].CreateFromRuleSettings(settings);
            }


            if (config.ContainsKey(Constants.TagKey))
            {
                var content = config[Constants.TagKey].ToString();

                try
                {
                    var tag = DicomTag.Parse(content);
                    return new AnonymizationDicomTagRule(tag, method, ruleSetting);
                }
                catch (Exception ex)
                {
                    try
                    {
                        var tag = DicomMaskedTag.Parse(content);
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
                                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.InvalidConfigurationValues, "Invaid tag in rule config");
                            }
                        }
                    }
                }
            }
            else
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.MissingConfigurationFields, "Missing tag in rule config");
            }
        }

        public AnonymizationDicomTagRule(DicomTag tag, string method, IDicomAnonymizationSetting ruleSetting)
        {
            EnsureArg.IsNotNull(tag, nameof(tag));
            EnsureArg.IsNotNull(method, nameof(method));

            Tag = tag;
            Method = method;
            RuleSetting = ruleSetting;
        }

        public AnonymizationDicomTagRule(DicomMaskedTag tag, string method, IDicomAnonymizationSetting ruleSetting)
        {
            EnsureArg.IsNotNull(tag, nameof(tag));
            EnsureArg.IsNotNull(method, nameof(method));

            MaskedTag = tag;
            Method = method;
            RuleSetting = ruleSetting;
            IsMasked = true;
        }

        public AnonymizationDicomTagRule(DicomVR vr, string method, IDicomAnonymizationSetting ruleSetting)
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
