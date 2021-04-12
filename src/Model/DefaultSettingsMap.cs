using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Model
{
    public static class DefaultSettingsMap
    {
        public static Dictionary<string, string> DefaultSettings = new Dictionary<string, string>()
        {
            { "perturb", "perturbDefaultSetting" },
            { "redact", "redactDefaultSetting" },
            { "encrypt", "encryptDefaultSetting" },
            { "cryptohash", "cryptoHashDefaultSetting" },
            { "dateshift", "dateShiftDefaultSetting" },
        };
    }
}
