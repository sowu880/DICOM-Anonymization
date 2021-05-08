// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Dicom.Anonymization.Processors.Settings
{
    public interface IDicomAnonymizationSetting
    {
        IDicomAnonymizationSetting CreateFromRuleSettings(string settings);

        IDicomAnonymizationSetting CreateFromRuleSettings(JObject settings);

        void Validate();
    }
}
