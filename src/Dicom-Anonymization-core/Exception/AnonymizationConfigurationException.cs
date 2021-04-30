using Dicom.Anonymization.Model;
using System;

namespace Dicom.Anonymization.AnonymizationConfigurations.Exceptions
{
    public class AnonymizationConfigurationException : DicomAnonymizationException
    {
        public AnonymizationConfigurationException(DicomAnonymizationErrorCode templateManagementErrorCode, string message)
            : base(templateManagementErrorCode, message)
        {
        }

        public AnonymizationConfigurationException(DicomAnonymizationErrorCode templateManagementErrorCode, string message, Exception innerException)
            : base(templateManagementErrorCode, message, innerException)
        {
        }
    }
}
