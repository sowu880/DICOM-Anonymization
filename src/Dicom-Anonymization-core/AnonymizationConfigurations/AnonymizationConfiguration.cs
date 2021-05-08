// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    [DataContract]
    public class AnonymizationConfiguration
    {
        [DataMember(Name = "rules")]
        public Dictionary<string, object>[] DicomTagRules { get; set; }

        [DataMember(Name = "defaultSettings")]
        public AnonymizationDefaultSettings DefaultSettings { get; set; }

        [DataMember(Name = "customizedSettings")]
        public Dictionary<string, JObject> CustomizedSettings { get; set; }
    }
}
