using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Model
{
    public enum DicomAnonymizationErrorCode
    {
        MissingConfigurationFields,
        InvalidConfigurationValues,
        UnsupportedAnonymizationRule,
        MissingRuleSettings,
        InvalidRuleSettings,

        UnsupportedAnonymizationFunction,
    }
}
