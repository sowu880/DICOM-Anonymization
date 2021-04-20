using System;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    public class AnonymizationConfigurationErrorsException : Exception
    {
        public AnonymizationConfigurationErrorsException(string message) : base(message)
        {
        }

        public AnonymizationConfigurationErrorsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
