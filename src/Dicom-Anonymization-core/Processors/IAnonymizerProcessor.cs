// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Dicom.Anonymization.Processors.Settings;

namespace Dicom.Anonymization.Processors
{
    public interface IAnonymizationProcessor
    {
        public void Process(DicomDataset dicomDataset, DicomItem item, IDicomAnonymizationSetting settings = null);
    }
}
