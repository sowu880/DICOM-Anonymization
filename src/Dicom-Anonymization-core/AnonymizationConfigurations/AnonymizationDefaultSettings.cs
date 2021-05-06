// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;
using Dicom.Anonymization.Processors.Settings;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    [DataContract]
    public class AnonymizationDefaultSettings
    {
        [DataMember(Name = "perturb")]
        public DicomPerturbSetting PerturbDefaultSetting { get; set; } = new DicomPerturbSetting();

        [DataMember(Name = "substitute")]
        public DicomSubstituteSetting SubstituteDefaultSetting { get; set; } = new DicomSubstituteSetting();

        [DataMember(Name = "dateshift")]
        public DicomDateShiftSetting DateShiftDefaultSetting { get; set; } = new DicomDateShiftSetting();

        [DataMember(Name = "encrypt")]
        public DicomEncryptionSetting EncryptDefaultSetting { get; set; } = new DicomEncryptionSetting();

        [DataMember(Name = "cryptohash")]
        public DicomCryptoHashSetting CryptoHashDefaultSetting { get; set; } = new DicomCryptoHashSetting();

        [DataMember(Name = "redact")]
        public DicomRedactSetting RedactDefaultSetting { get; set; } = new DicomRedactSetting();

        public static Dictionary<string, IDicomAnonymizationSetting> DicomSettingsMapping = new Dictionary<string, IDicomAnonymizationSetting>()
        {
            { "perturb", new DicomPerturbSetting() },
            { "substitute", new DicomSubstituteSetting() },
            { "dateshift", new DicomDateShiftSetting() },
            { "encrypt", new DicomEncryptionSetting() },
            { "cryptohash", new DicomCryptoHashSetting() },
            { "redact", new DicomRedactSetting() },
        };

        public IDicomAnonymizationSetting GetDefaultSetting(string method)
        {
            switch (method)
            {
                case "perturb":
                    return PerturbDefaultSetting;
                case "substitute":
                    return SubstituteDefaultSetting;
                case "dateshift":
                    return DateShiftDefaultSetting;
                case "encrypt":
                    return EncryptDefaultSetting;
                case "cryptohash":
                    return CryptoHashDefaultSetting;
                case "redact":
                    return RedactDefaultSetting;
                default:
                    return null;
            }
        }
    }
}