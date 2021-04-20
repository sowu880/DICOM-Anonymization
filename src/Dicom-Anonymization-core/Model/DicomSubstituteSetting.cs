using De_Id_Function_Shared.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dicom.Anonymization.Model
{
    public class DicomSubstituteSetting
    {
        public string ReplaceWith { get; set; }

        public static DicomSubstituteSetting CreateFromJsonString(string settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomSubstituteSetting>(settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse redact setting", ex);
            }
        }

        public static DicomSubstituteSetting CreateFromJson(Dictionary<string, object> settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomSubstituteSetting>(JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse redact setting", ex);
            }
        }
    }
}
