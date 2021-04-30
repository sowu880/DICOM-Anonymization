using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Dicom.Anonymization.Model;

namespace Dicom.Anonymization.Processors.Settings
{
    public class DicomEncryptionSetting: IDicomAnonymizationSetting
    {
        public string EncryptKey { get; set; }

        public EncryptFunctionTypes EncryptFunction { get; set; }

        public IDicomAnonymizationSetting CreateFromRuleSettings(string settings)
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

        public IDicomAnonymizationSetting CreateFromRuleSettings(Dictionary<string, object> settings)
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
