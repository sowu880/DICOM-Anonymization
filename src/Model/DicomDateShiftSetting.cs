using De_Id_Function_Shared.Settings;
using Dicom.Anonymization.AnonymizerConfigurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Model
{
    public class DicomDateShiftSetting : DateShiftSetting
    {
        public DateShiftScope DateShiftScope { get; set; }

        public static DicomDateShiftSetting CreateFromJsonString(string settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomDateShiftSetting>(settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse dateshift setting", ex);
            }
        }

        public static DicomDateShiftSetting CreateFromJson(Dictionary<string, object> settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomDateShiftSetting>(JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse dateshift setting", ex);
            }
        }
    }
}
