using De_Id_Function_Shared.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dicom.Anonymization.Processors.Settings
{
    public class DicomRedactSetting : RedactSetting, IDicomAnonymizationSetting
    {
        public IDicomAnonymizationSetting CreateFromRuleSettings(string settings)
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

        public IDicomAnonymizationSetting CreateFromRuleSettings(Dictionary<string, object> settings)
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
