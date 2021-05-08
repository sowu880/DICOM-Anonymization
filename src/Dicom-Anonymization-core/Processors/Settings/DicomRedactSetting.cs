// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using De_Id_Function_Shared.Settings;
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Dicom.Anonymization.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dicom.Anonymization.Processors.Settings
{
    public class DicomRedactSetting : RedactSetting, IDicomAnonymizationSetting
    {
        public IDicomAnonymizationSetting CreateFromRuleSettings(string settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomRedactSetting>(settings);
            }
            catch (Exception ex)
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.InvalidRuleSettings, "Fail to parse redact setting", ex);
            }
        }

        public IDicomAnonymizationSetting CreateFromRuleSettings(JObject settings)
        {
            try
            {
                return settings.ToObject<DicomRedactSetting>();
            }
            catch (Exception ex)
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.InvalidRuleSettings, "Fail to parse redact setting", ex);
            }
        }

        public void Validate()
        {
        }
    }
}
