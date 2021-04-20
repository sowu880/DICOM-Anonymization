using De_Id_Function_Shared;
using Dicom.Anonymization.Model;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using FellowOakDicom.IO;
using System.Text;

namespace Dicom.Anonymization.Processors
{
    public class PerturbProcessor : IAnonymizationProcessor
    {
        public PerturbProcessor(DicomPerturbSetting defaultSetting = null)
        {
            DefaultSetting = defaultSetting;
        }

        public DicomPerturbSetting DefaultSetting { get; set; }

        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
            var perturbSetting = settings == null ? DefaultSetting : CreatePertubSettings(settings);
            var perturbedValues = new List<decimal>() { };

            if (item.ValueRepresentation == DicomVR.AS)
            {
                var values = ((DicomAgeString)item).Get<string[]>().Select(Utility.ParseAge).Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.Select(Utility.AgeToString).ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.DS)
            {
                var values = ((DicomDecimalString)item).Get<decimal[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.FL)
            {
                var values = ((DicomFloatingPointSingle)item).Get<float[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.OF)
            {
                var values = ((DicomOtherFloat)item).Get<float[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.FD)
            {
                var values = ((DicomFloatingPointSingle)item).Get<double[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.OD)
            {
                var values = ((DicomFloatingPointSingle)item).Get<double[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.IS)
            {
                var values = ((DicomIntegerString)item).Get<int[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.SL)
            {
                var values = ((DicomSignedLong)item).Get<int[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.SS)
            {
                var values = ((DicomSignedShort)item).Get<short[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.US)
            {
                var values = ((DicomUnsignedShort)item).Get<ushort[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.OW)
            {
                if (item is DicomOtherWordFragment)
                {
                    Console.WriteLine($"Invalid perturb operation for item {item}");
                }
                else
                {
                    var values = ((DicomOtherWord)item).Get<ushort[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                    dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
                }
            }
            else if (item.ValueRepresentation == DicomVR.UL)
            {
                var values = ((DicomUnsignedLong)item).Get<uint[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.OL)
            {
                var values = ((DicomOtherLong)item).Get<uint[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.UV)
            {
                var values = ((DicomUnsignedVeryLong)item).Get<ulong[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.OV)
            {
                var values = ((DicomOtherVeryLong)item).Get<ulong[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.SV)
            {
                var values = ((DicomSignedVeryLong)item).Get<long[]>().Select(x => PerturbFunction.Perturb(x, perturbSetting));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.ToArray());
            }
            else
            {
                throw new Exception($"Invalid perturb operation for item {item}");
            }
        }

        public PerturbSetting CreatePertubSettings(Dictionary<string, object> ruleSettings)
        {
            EnsureArg.IsNotNull(ruleSettings);

            var roundTo = 2;
            if (ruleSettings.ContainsKey(RuleKeys.RoundTo))
            {
                roundTo = Convert.ToInt32(ruleSettings.GetValueOrDefault(RuleKeys.RoundTo)?.ToString());
            }

            double span = 0;
            if (ruleSettings.ContainsKey(RuleKeys.Span))
            {
                span = Convert.ToDouble(ruleSettings.GetValueOrDefault(RuleKeys.Span)?.ToString());
            }

            var rangeType = PerturbRangeType.Fixed;
            if (string.Equals(
                PerturbRangeType.Proportional.ToString(),
                ruleSettings.GetValueOrDefault(RuleKeys.RangeType)?.ToString(),
                StringComparison.InvariantCultureIgnoreCase))
            {
                rangeType = PerturbRangeType.Proportional;
            }

            return new PerturbSetting
            {
                Span = span,
                RangeType = rangeType,
                RoundTo = roundTo,
            };
        }
    }
}
