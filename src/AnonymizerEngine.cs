// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using De_Id_Function_Shared.Settings;
using Dicom.Anonymization.AnonymizerConfigurations;
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
    public class AnonymizerEngine
    {
        private readonly Dictionary<string, IAnonymizerProcessor> _processors = new Dictionary<string, IAnonymizerProcessor> { };
        private readonly AnonymizationDicomTagRule[] _rulesByTag;
        private readonly AnonymizerRule[] _rulesByVR;

        public AnonymizerEngine(string configFilePath = "configuration-sample.json")
        {
            var configurationManager = AnonymizerConfigurationManager.CreateFromConfigurationFile(configFilePath);
            InitializeProcessors(configurationManager);
            _rulesByTag = configurationManager.DicomTagRules;
            // _rulesByVR = configurationManager.DicomVRRules;
        }

        public AnonymizerEngine(AnonymizerConfigurationManager configurationManager)
        {
            _processors = new Dictionary<string, IAnonymizerProcessor>();

            InitializeProcessors(configurationManager);

            _rulesByTag = configurationManager.DicomTagRules;
        }

        public void Anonymize(DicomDataset dataset)
        {
            var curDataset = dataset.ToArray();
            foreach (var item in curDataset)
            {
                if (item.ValueRepresentation == DicomVR.SQ)
                {
                    foreach (var subItem in ((DicomSequence)item).Items)
                    {
                        Anonymize(subItem);
                    }
                }
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

                    string originalValue;
                    if (item is DicomElement)
                    {
                        originalValue = ((DicomElement)item).Get<string>();
                    }
                    else
                    {
                        List<string> results = new List<string>();
                        var values = ((DicomFragmentSequence)item).Fragments;
                        foreach (var value in values)
                        {
                            var result = Convert.ToBase64String(value.Data);
                            results.Add(result);
                        }

                        originalValue = "fragments";
                    }

                    _processors[method].Process(dataset, item, ruleByTag.RuleSetting);
                    Console.WriteLine("{0,-15}{1,-40}{2,-15}{3,-30}{4,-75}","("+string.Format("{0,4:X4}",item.Tag.Group)+","+string.Format("{0,4:X4}", item.Tag.Element)+")", item.Tag.DictionaryEntry.Name,method,originalValue, dataset.GetSingleValueOrDefault<string>(item.Tag, ""));
                    continue;
                }
            }
        }

        private void InitializeProcessors(AnonymizerConfigurationManager configurationManager)
        {
            _processors.Add(AnonymizerMethod.Redact.ToString().ToUpperInvariant(), new RedactProcessor(DicomRedactSetting.CreateFromJsonString(configurationManager.GetSettings().GetValueOrDefault(Constants.RedactDefaultSetting).ToString())));
            _processors.Add(AnonymizerMethod.Keep.ToString().ToUpperInvariant(), new KeepProcessor());
            _processors.Add(AnonymizerMethod.Perturb.ToString().ToUpperInvariant(), new PerturbProcessor(DicomPerturbSetting.CreateFromJsonString(configurationManager.GetSettings().GetValueOrDefault(Constants.PerturbDefaultSetting).ToString())));
            _processors.Add(AnonymizerMethod.Encrypt.ToString().ToUpperInvariant(), new EncryptionProcessor(DicomEncryptionSetting.CreateFromJsonString(configurationManager.GetSettings().GetValueOrDefault(Constants.EncryptDefaultSetting).ToString())));
            _processors.Add(AnonymizerMethod.CryptoHash.ToString().ToUpperInvariant(), new CryptoHashProcessor(DicomCryptoHashSetting.CreateFromJsonString(configurationManager.GetSettings().GetValueOrDefault(Constants.CryptoHashDefaultSetting).ToString())));
            _processors.Add(AnonymizerMethod.DateShift.ToString().ToUpperInvariant(), new DateShiftProcessor(DicomDateShiftSetting.CreateFromJsonString(configurationManager.GetSettings().GetValueOrDefault(Constants.DateShiftDefaultSetting).ToString())));
        }
    }
}
