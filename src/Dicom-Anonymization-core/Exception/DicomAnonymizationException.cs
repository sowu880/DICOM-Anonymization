using Dicom.Anonymization.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.AnonymizationConfigurations.Exceptions
{
    public class DicomAnonymizationException : Exception
    {
        public DicomAnonymizationException()
        {
        }

        public DicomAnonymizationException(string message)
            : base(message)
        {
        }

        public DicomAnonymizationException(DicomAnonymizationErrorCode templateManagementErrorCode, string message)
            : base(message)
        {
            TemplateManagementErrorCode = templateManagementErrorCode;
        }

        public DicomAnonymizationException(DicomAnonymizationErrorCode templateManagementErrorCode, string message, Exception innerException)
            : base(message, innerException)
        {
            TemplateManagementErrorCode = templateManagementErrorCode;
        }

        public DicomAnonymizationErrorCode TemplateManagementErrorCode { get; }
    }
}
