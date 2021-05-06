// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using De_Id_Function_Shared.Settings;
using Dicom.Anonymization.AnonymizationConfigurations;
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Dicom.Anonymization.Model;
using Newtonsoft.Json;

namespace Dicom.Anonymization.Processors.Settings
{
    public class DicomDateShiftSetting : DateShiftSetting, IDicomAnonymizationSetting
    {
        public DateShiftScope DateShiftScope { get; set; }

        public IDicomAnonymizationSetting CreateFromRuleSettings(string settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomDateShiftSetting>(settings);
            }
            catch (Exception ex)
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.InvalidRuleSettings, "Fail to parse dateshift setting", ex);
            }
        }

        public IDicomAnonymizationSetting CreateFromRuleSettings(Dictionary<string, object> settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomDateShiftSetting>(JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex)
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.InvalidRuleSettings, "Fail to parse dateshift setting", ex);
            }
        }

        public void Validate()
        {
        }
    }
}
