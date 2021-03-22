using System;

namespace Dicom.Anonymization.AnonymizerConfigurations
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
