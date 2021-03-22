using Dicom.Anonymization.AnonymizerConfigurations;
using Dicom.Anonymization.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
                var ruleByTag = _rulesByTag?.Where(r => (string.Equals(item.Tag.DictionaryEntry.Keyword, r.Tag?.DictionaryEntry.Keyword, StringComparison.CurrentCultureIgnoreCase) 
                || string.Equals(item.ValueRepresentation.Code, r.VR?.Code, StringComparison.InvariantCultureIgnoreCase))).FirstOrDefault();
                if (ruleByTag != null)
                {
                    string method = ruleByTag.Method.ToUpperInvariant();
                    if (!_processors.ContainsKey(method))
                    {
                        continue;
                    }

                    _processors[method].Process(dataset, item, ruleByTag.RuleSettings);
                    continue;
                }
            }
        }

        private void InitializeProcessors(AnonymizerConfigurationManager configurationManager)
        {
            _processors.Add(AnonymizerMethod.Redact.ToString().ToUpperInvariant(), new RedactProcessor(configurationManager.GetParameterConfiguration()));
            _processors.Add(AnonymizerMethod.Keep.ToString().ToUpperInvariant(), new KeepProcessor());
            _processors.Add(AnonymizerMethod.Perturb.ToString().ToUpperInvariant(), new PerturbProcessor());
            _processors.Add(AnonymizerMethod.Encrypt.ToString().ToUpperInvariant(), new EncryptionProcessor(configurationManager.GetParameterConfiguration()));
            _processors.Add(AnonymizerMethod.CryptoHash.ToString().ToUpperInvariant(), new CryptoHashProcessor(configurationManager.GetParameterConfiguration()));
            _processors.Add(AnonymizerMethod.DateShift.ToString().ToUpperInvariant(), new DateShiftProcessor(configurationManager.GetParameterConfiguration()));
        }
    }
}
