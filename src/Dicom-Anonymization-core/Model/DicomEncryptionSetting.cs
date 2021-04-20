using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Model
{
    public class DicomEncryptionSetting
    {
        public string EncryptKey { get; set; }

        public EncryptFunctionTypes EncryptFunction { get; set; }

        public static DicomEncryptionSetting CreateFromJsonString(string settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomEncryptionSetting>(settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse encrypt setting", ex);
            }
        }

        public static DicomEncryptionSetting CreateFromJson(Dictionary<string, object> settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomEncryptionSetting>(JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse encrypt setting", ex);
            }
        }
    }
}
