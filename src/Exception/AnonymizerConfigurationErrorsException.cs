using System;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    public class AnonymizerConfigurationErrorsException : Exception
    {
        public AnonymizerConfigurationErrorsException(string message) : base(message)
        {
        }

        public AnonymizerConfigurationErrorsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
