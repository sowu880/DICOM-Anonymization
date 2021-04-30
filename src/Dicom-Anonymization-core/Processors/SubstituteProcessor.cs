// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using Dicom.Anonymization.Processors.Settings;

namespace Dicom.Anonymization.Processors
{
    public class SubstituteProcessor : IAnonymizationProcessor
    {
        private DicomSubstituteSetting _defaultSubstituteSetting;

        public SubstituteProcessor(DicomSubstituteSetting defaultSubstituteSettings)
        {
            _defaultSubstituteSetting = defaultSubstituteSettings;
        }

        public void Process(DicomDataset dicomDataset, DicomItem item, IDicomAnonymizationSetting settings = null)
        {
            var substituteSetting = settings == null ? _defaultSubstituteSetting : (DicomSubstituteSetting)settings;

            if (item is DicomOtherByte || item is DicomSequence || item is DicomFragmentSequence)
            {
                throw new Exception($"Invalid perturb operation for item {item}");
            }

            try
            {
                if (item is DicomOtherWord)
                {
                    dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, ushort.Parse(substituteSetting.ReplaceWith));
                }
                else if (item is DicomOtherLong)
                {
                    dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, uint.Parse(substituteSetting.ReplaceWith));
                }
                else if (item is DicomOtherDouble)
                {
                    dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, double.Parse(substituteSetting.ReplaceWith));
                }
                else if (item is DicomOtherFloat)
                {
                    dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, float.Parse(substituteSetting.ReplaceWith));
                }
                else
                {
                    dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, substituteSetting.ReplaceWith);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid replace value", ex);
            }
        }
    }
}
