using De_Id_Function_Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dicom.Anonymization.Model
{
    public class DicomCryptoHashSetting
    {
        public string CryptoHashKey { get; set; }

        public CryptoHashFunctionTypes CryptoHashFunction { get; set; }

        public static DicomCryptoHashSetting CreateFromJsonString(string settings)
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

        public static DicomCryptoHashSetting CreateFromJson(Dictionary<string, object> settings)
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
