// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Dicom.Anonymization.Model;
using Dicom.Anonymization.Processors.Settings;
using EnsureThat;

namespace Dicom.Anonymization.Processors
{
    public class RemoveProcessor : IAnonymizationProcessor
    {
        public void Process(DicomDataset dicomDataset, DicomItem item, DicomBasicInformation basicInfo = null, IDicomAnonymizationSetting settings = null)
        {
            EnsureArg.IsNotNull(dicomDataset, nameof(dicomDataset));
            EnsureArg.IsNotNull(item, nameof(item));

            dicomDataset.Remove(item.Tag);
        }
    }
}
