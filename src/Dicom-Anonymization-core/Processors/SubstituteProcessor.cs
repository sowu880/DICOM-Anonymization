using Dicom.Anonymization.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Processors
{
    public class SubstituteProcessor : IAnonymizationProcessor
    {
        public SubstituteProcessor(DicomSubstituteSetting defaultSubstituteSettings)
        {
            DefaultSubstituteSetting = defaultSubstituteSettings;
        }

        public DicomSubstituteSetting DefaultSubstituteSetting { get; set; }

        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
            DicomSubstituteSetting substituteSetting = DefaultSubstituteSetting;
            if (settings != null)
            {
                substituteSetting = DicomSubstituteSetting.CreateFromJson(settings);
            }

            dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, substituteSetting.ReplaceWith);
        }
    }
}
