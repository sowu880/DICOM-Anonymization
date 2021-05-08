﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Dicom.Anonymization.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dicom.Anonymization.Processors.Settings
{
    public class DicomCryptoHashSetting : IDicomAnonymizationSetting
    {
        public string CryptoHashKey { get; set; } = string.Empty;

        public CryptoHashFunctionTypes CryptoHashFunction { get; set; } = CryptoHashFunctionTypes.Sha256;

        public IDicomAnonymizationSetting CreateFromRuleSettings(string settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomCryptoHashSetting>(settings);
            }
            catch (Exception ex)
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.InvalidRuleSettings, "Fail to parse cryptohash setting", ex);
            }
        }

        public IDicomAnonymizationSetting CreateFromRuleSettings(JObject settings)
        {
            try
            {
                return settings.ToObject<DicomCryptoHashSetting>();
            }
            catch (Exception ex)
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.InvalidRuleSettings, "Fail to parse cryptohash setting", ex);
            }
        }

        public void Validate()
        {
        }
    }
}
