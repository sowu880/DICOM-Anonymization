// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Dicom.Anonymization.AnonymizationConfigurations;
using Dicom.Anonymization.Model;
using Dicom.Anonymization.Processors;
using Microsoft.Extensions.Logging;

namespace Dicom.Anonymization
{
    public class AnonymizationEngine
    {
        private readonly Dictionary<string, IAnonymizationProcessor> _processors = new Dictionary<string, IAnonymizationProcessor> { };
        private readonly AnonymizationDicomTagRule[] _rulesByTag;
        private readonly AnonymizationDefaultSettings _defaultSettings;
        private readonly ILogger _logger = AnonymizerLogging.CreateLogger<AnonymizationEngine>();

        public AnonymizationEngine(string configFilePath = "configuration-sample.json")
            : this(AnonymizationConfigurationManager.CreateFromConfigurationFile(configFilePath))
        {
        }

        public AnonymizationEngine(AnonymizationConfigurationManager configurationManager)
        {
            _defaultSettings = configurationManager.GetDefaultSettings();
            InitializeProcessors(configurationManager);
            _rulesByTag = configurationManager.DicomTagRules;

            _logger.LogDebug("AnonymizerEngine initialized successfully");
        }

        public void Anonymize(DicomDataset dataset)
        {
            var basicInfo = ExtractBasicInformation(dataset);
            var curDataset = dataset.ToArray();
            foreach (var item in curDataset)
            {
                var ruleByTag = _rulesByTag?.Where(r => string.Equals(item.Tag.DictionaryEntry.Keyword, r.Tag?.DictionaryEntry.Keyword, StringComparison.CurrentCultureIgnoreCase)
                || string.Equals(item.ValueRepresentation.Code, r.VR?.Code, StringComparison.InvariantCultureIgnoreCase)
                || (r.IsMasked && r.MaskedTag.IsMatch(item.Tag))).FirstOrDefault();
                if (ruleByTag != null)
                {
                    string method = ruleByTag.Method.ToUpperInvariant();
                    if (!_processors.ContainsKey(method))
                    {
                        continue;
                    }

                    try
                    {
                        _processors[method].Process(dataset, item, basicInfo, ruleByTag.RuleSetting);
                        _logger.LogDebug("{0,-15}{1,-40}{2,-15}{3,-50}{4,-75}", "(" + string.Format("{0,4:X4}", item.Tag.Group) + "," + string.Format("{0,4:X4}", item.Tag.Element) + ")", item.Tag.DictionaryEntry.Name, method, item is DicomElement ? ((DicomElement)item).Get<string>() : "sequence", dataset.GetSingleValueOrDefault<string>(item.Tag, string.Empty));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Fail to anonymize Item {item.Tag.DictionaryEntry.Name} using {method} method. The original value will be kept.", ex);
                    }

                    continue;
                }
            }
        }

        private DicomBasicInformation ExtractBasicInformation(DicomDataset dataset)
        {
            var basicInfo = new DicomBasicInformation
            {
                StudyInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty),
                SopInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, string.Empty),
                SeriesInstanceUID = dataset.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty),
            };
            return basicInfo;
        }

        private void InitializeProcessors(AnonymizationConfigurationManager configurationManager)
        {
            _processors.Add(AnonymizationMethod.Redact.ToString().ToUpperInvariant(), new RedactProcessor(_defaultSettings.RedactDefaultSetting));
            _processors.Add(AnonymizationMethod.Keep.ToString().ToUpperInvariant(), new KeepProcessor());
            _processors.Add(AnonymizationMethod.Remove.ToString().ToUpperInvariant(), new RemoveProcessor());
            _processors.Add(AnonymizationMethod.RefreshUID.ToString().ToUpperInvariant(), new RefreshUIDProcessor());
            _processors.Add(AnonymizationMethod.Substitute.ToString().ToUpperInvariant(), new SubstituteProcessor(_defaultSettings.SubstituteDefaultSetting));
            _processors.Add(AnonymizationMethod.Perturb.ToString().ToUpperInvariant(), new PerturbProcessor(_defaultSettings.PerturbDefaultSetting));
            _processors.Add(AnonymizationMethod.Encrypt.ToString().ToUpperInvariant(), new EncryptionProcessor(_defaultSettings.EncryptDefaultSetting));
            _processors.Add(AnonymizationMethod.CryptoHash.ToString().ToUpperInvariant(), new CryptoHashProcessor(_defaultSettings.CryptoHashDefaultSetting));
            _processors.Add(AnonymizationMethod.DateShift.ToString().ToUpperInvariant(), new DateShiftProcessor(_defaultSettings.DateShiftDefaultSetting));
        }
    }
}
