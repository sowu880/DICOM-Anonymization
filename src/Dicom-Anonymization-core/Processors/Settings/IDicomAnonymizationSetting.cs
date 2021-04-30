using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Processors.Settings
{
    public interface IDicomAnonymizationSetting
    {
        IDicomAnonymizationSetting CreateFromRuleSettings(Dictionary<string, object> settings);

        IDicomAnonymizationSetting CreateFromRuleSettings(string settings);
    }
}
