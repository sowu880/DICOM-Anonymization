using De_Id_Function_Shared.Settings;
using Dicom.Anonymization.AnonymizationConfigurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Dicom.Anonymization.Model;

namespace Dicom.Anonymization.Processors.Settings
{
    public class DicomDateShiftSetting : DateShiftSetting, IDicomAnonymizationSetting
    {
        public DateShiftScope DateShiftScope { get; set; }

        public IDicomAnonymizationSetting CreateFromRuleSettings(string settings)
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

        public IDicomAnonymizationSetting CreateFromRuleSettings(Dictionary<string, object> settings)
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
