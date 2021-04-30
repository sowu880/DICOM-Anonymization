// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using De_Id_Function_Shared.Settings;
using Dicom.Anonymization.AnonymizationConfigurations;
using Dicom.Anonymization.Model;
using Dicom.Anonymization.Processors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FellowOakDicom.IO;

namespace Dicom.Anonymization
{
    public class AnonymizationEngine
    {
        private readonly Dictionary<string, IAnonymizationProcessor> _processors = new Dictionary<string, IAnonymizationProcessor> { };
        private readonly AnonymizationDicomTagRule[] _rulesByTag;
        private readonly AnonymizationDefaultSettings _defaultSettings;

        public AnonymizationEngine(string configFilePath = "configuration-sample.json")
        {
            var configurationManager = AnonymizationConfigurationManager.CreateFromConfigurationFile(configFilePath);
            InitializeProcessors(configurationManager);
            _rulesByTag = configurationManager.DicomTagRules;
            // _rulesByVR = configurationManager.DicomVRRules;
        }

        public AnonymizationEngine(AnonymizationConfigurationManager configurationManager)
        {
            _processors = new Dictionary<string, IAnonymizationProcessor>();

            InitializeProcessors(configurationManager);

            _rulesByTag = configurationManager.DicomTagRules;
        }

        public void Anonymize(DicomDataset dataset)
        {
            var curDataset = dataset.ToArray();
            foreach (var item in curDataset)
            {
                var ruleByTag = _rulesByTag?.Where(r => string.Equals(item.Tag.DictionaryEntry.Keyword, r.Tag?.DictionaryEntry.Keyword, StringComparison.CurrentCultureIgnoreCase)
                || string.Equals(item.ValueRepresentation.Code, r.VR?.Code, StringComparison.InvariantCultureIgnoreCase) 
                || (r.IsMasked && r.MaskedTag.IsMatch(item.Tag))).FirstOrDefault();
                if (ruleByTag != null)
                {
                    if (item.ValueRepresentation == DicomVR.SQ)
                    {
                        if (!string.Equals(ruleByTag.Method, "remove", StringComparison.InvariantCultureIgnoreCase) && !string.Equals(ruleByTag.Method, "redact", StringComparison.InvariantCultureIgnoreCase))
                        {
                            throw new Exception($"Invalid {ruleByTag.Method} operation for item {item}");
                        }
                    }

                    string method = ruleByTag.Method.ToUpperInvariant();
                    if (!_processors.ContainsKey(method))
                    {
                        continue;
                    }

                    string originalValue;
                    if (item is DicomElement)
                    {
                        originalValue = ((DicomElement)item).Get<string>();
                    }
                    else
                    {

                        originalValue = "sequence";
                    }

                    Console.WriteLine(item.Tag.ToString(),item.ValueRepresentation);
                    _processors[method].Process(dataset, item, ruleByTag.RuleSetting);
                    // Console.WriteLine("{0,-15}{1,-40}{2,-15}{3,-50}{4,-75}","("+string.Format("{0,4:X4}",item.Tag.Group)+","+string.Format("{0,4:X4}", item.Tag.Element)+")", item.Tag.DictionaryEntry.Name, method, originalValue, dataset.GetSingleValueOrDefault<string>(item.Tag, string.Empty));
                    continue;
                }
            }
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
