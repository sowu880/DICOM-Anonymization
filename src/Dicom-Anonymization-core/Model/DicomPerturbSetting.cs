using De_Id_Function_Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Model
{
    public class DicomPerturbSetting : PerturbSetting
    {
        public PerturbDistribution Distribution { get; set; }

        public static DicomPerturbSetting CreateFromJsonString(string settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomPerturbSetting>(settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse perturb setting", ex);
            }
        }

        public static DicomPerturbSetting CreateFromJson(Dictionary<string, object> settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<DicomPerturbSetting>(JsonConvert.SerializeObject(settings));
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to parse perturb setting", ex);
            }
        }
    }
}
