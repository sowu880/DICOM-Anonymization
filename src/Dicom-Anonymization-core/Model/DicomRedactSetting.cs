using De_Id_Function_Shared.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dicom.Anonymization.Model
{
    public class DicomRedactSetting : RedactSetting
    {
        public static DicomRedactSetting CreateFromJsonString(string settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomRedactSetting>(settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse redact setting", ex);
            }
        }

        public static DicomRedactSetting CreateFromJson(Dictionary<string, object> settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomRedactSetting>(JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse redact setting", ex);
            }
        }
    }
}
