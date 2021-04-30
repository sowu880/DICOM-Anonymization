using De_Id_Function_Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Dicom.Anonymization.Model;

namespace Dicom.Anonymization.Processors.Settings
{
    public class DicomCryptoHashSetting : IDicomAnonymizationSetting
    {
        public string CryptoHashKey { get; set; }

        public CryptoHashFunctionTypes CryptoHashFunction { get; set; }

        public IDicomAnonymizationSetting CreateFromRuleSettings(string settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomCryptoHashSetting>(settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse cryptohash setting", ex);
            }
        }

        public IDicomAnonymizationSetting CreateFromRuleSettings(Dictionary<string, object> settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomCryptoHashSetting>(JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse cryptohash setting", ex);
            }
        }
    }
}
