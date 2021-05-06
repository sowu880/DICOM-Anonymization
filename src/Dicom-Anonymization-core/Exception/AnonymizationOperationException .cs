using Dicom.Anonymization.Model;
using System;

namespace Dicom.Anonymization.AnonymizationConfigurations.Exceptions
{
    public class AnonymizationOperationException : DicomAnonymizationException
    {
        public AnonymizationOperationException(DicomAnonymizationErrorCode templateManagementErrorCode, string message)
            : base(templateManagementErrorCode, message)
        {
        }

        public AnonymizationOperationException(DicomAnonymizationErrorCode templateManagementErrorCode, string message, Exception innerException)
            : base(templateManagementErrorCode, message, innerException)
        {
        }
    }
}
