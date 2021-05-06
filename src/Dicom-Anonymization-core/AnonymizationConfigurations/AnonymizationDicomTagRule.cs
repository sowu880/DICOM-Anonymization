// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Dicom.Anonymization.Model;
using Dicom.Anonymization.Processors.Settings;
using EnsureThat;
using Newtonsoft.Json;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    public class AnonymizationDicomTagRule
    {

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

        public DicomVR VR { get; set; }

        public DicomTag Tag { get; set; }

        public DicomMaskedTag MaskedTag { get; set; }

        public string Method { get; set; }

        public bool IsVRRule { get; set; } = false;

        public bool IsMasked { get; set; } = false;

        public IDicomAnonymizationSetting RuleSetting { get; set; }

        public static AnonymizationDicomTagRule CreateAnonymizationDicomRule(Dictionary<string, object> rule, AnonymizationConfiguration configuration)
        {
            EnsureArg.IsNotNull(rule, nameof(rule));
            EnsureArg.IsNotNull(configuration, nameof(configuration));

            // Parse and validate method
            if (!rule.ContainsKey(Constants.MethodKey))
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.MissingConfigurationFields, "Missing method in rule config");
            }

            var method = rule[Constants.MethodKey].ToString();
            var supportedMethods = Enum.GetNames(typeof(AnonymizationMethod)).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            if (!supportedMethods.Contains(method))
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.UnsupportedAnonymizationRule, $"Anonymization method {method} not supported.");
            }

            // Parse and validate settings
            Dictionary<string, object> parameters = null;
            if (rule.ContainsKey(Constants.Parameters))
            {
                parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(rule[Constants.Parameters].ToString());
            }

            IDicomAnonymizationSetting ruleSetting = null;
            if (rule.ContainsKey(Constants.RuleSetting))
            {
                if (configuration.CustomizedSettings == null || !configuration.CustomizedSettings.ContainsKey(rule[Constants.RuleSetting].ToString()))
                {
                    throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.MissingConfigurationFields, $"Customized setting {rule[Constants.RuleSetting]} not defined");
                }

                var settings = configuration.CustomizedSettings[rule[Constants.RuleSetting].ToString()].ToString();
                ruleSetting = AnonymizationDefaultSettings.DicomSettingsMapping[method].CreateFromRuleSettings(settings);
                if (parameters != null)
                {
                    var newSettings = parameters.Concat(JsonConvert.DeserializeObject<Dictionary<string, object>>(settings).Where(x => !parameters.ContainsKey(x.Key))).ToDictionary(s => s.Key, s => s.Value);
                    ruleSetting = AnonymizationDefaultSettings.DicomSettingsMapping[method].CreateFromRuleSettings(settings);
                }

                ruleSetting.Validate();
            }
            else if (parameters != null)
            {
                ruleSetting = configuration.DefaultSettings.GetDefaultSetting(method);
                var defaultSetting = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(ruleSetting));
                var settings = parameters.Concat(defaultSetting.Where(x => !parameters.ContainsKey(x.Key))).ToDictionary(s => s.Key, s => s.Value);
                ruleSetting = AnonymizationDefaultSettings.DicomSettingsMapping[method].CreateFromRuleSettings(settings);
            }

            // Parse and validate tag
            if (rule.ContainsKey(Constants.TagKey))
            {
                var content = rule[Constants.TagKey].ToString();

                try
                {
                    var tag = DicomTag.Parse(content);
                    return new AnonymizationDicomTagRule(tag, method, ruleSetting);
                }
                catch (Exception)
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
    }
}
