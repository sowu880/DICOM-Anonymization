using Dicom.Anonymization.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Dicom.Anonymization.Processors.Settings;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    [DataContract]
    public class AnonymizationDefaultSettings
    {
        [DataMember(Name = "perturb")]
        public DicomPerturbSetting PerturbDefaultSetting { get; set; }

        [DataMember(Name = "substitute")]
        public DicomSubstituteSetting SubstituteDefaultSetting { get; set; }

        [DataMember(Name = "dateshift")]
        public DicomDateShiftSetting DateShiftDefaultSetting { get; set; }

        [DataMember(Name = "encrypt")]
        public DicomEncryptionSetting EncryptDefaultSetting { get; set; }

        [DataMember(Name = "cryptohash")]
        public DicomCryptoHashSetting CryptoHashDefaultSetting { get; set; }

        [DataMember(Name = "redact")]
        public DicomRedactSetting RedactDefaultSetting { get; set; }

        public static Dictionary<string, IDicomAnonymizationSetting> DicomSettingsMapping = new Dictionary<string, IDicomAnonymizationSetting>()
        {
            { "perturb", new DicomCryptoHashSetting() },
            { "substitute", new DicomSubstituteSetting() },
            { "dateshift", new DicomDateShiftSetting() },
            { "encrypt", new DicomEncryptionSetting() },
            { "cryptohash", new DicomCryptoHashSetting() },
            { "perturb", new DicomCryptoHashSetting() },
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