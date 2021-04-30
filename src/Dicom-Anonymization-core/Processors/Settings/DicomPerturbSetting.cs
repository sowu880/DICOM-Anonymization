using De_Id_Function_Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Dicom.Anonymization.Model;

namespace Dicom.Anonymization.Processors.Settings
{
    public class DicomPerturbSetting : PerturbSetting, IDicomAnonymizationSetting
    {
        public PerturbDistribution Distribution { get; set; }

        public IDicomAnonymizationSetting CreateFromRuleSettings(string settings)
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

        public IDicomAnonymizationSetting CreateFromRuleSettings(Dictionary<string, object> settings)
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
