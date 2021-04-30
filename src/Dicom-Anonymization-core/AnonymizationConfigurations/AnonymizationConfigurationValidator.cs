using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Dicom.Anonymization.Model;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    public class AnonymizationConfigurationValidator
    {
        public void Validate(AnonymizationConfiguration config)
        {
            if (config.DicomTagRules == null)
            {
                throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.MissingConfigurationFields, "The configuration is invalid, please specify any dicom rules");
            }

            var supportedMethods = Enum.GetNames(typeof(AnonymizationMethod)).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            foreach (var rule in config.DicomTagRules)
            {
                if (!rule.ContainsKey(Constants.TagKey) || !rule.ContainsKey(Constants.MethodKey))
                {
                    throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.MissingConfigurationFields, "Missing tag or method in dicom rule config.");
                }

                // Todo Grammar check on DICOM tag?

                // Method validate
                string method = rule[Constants.MethodKey].ToString();
                if (!supportedMethods.Contains(method))
                {
                    throw new AnonymizationConfigurationException(DicomAnonymizationErrorCode.UnsupportedAnonymizationRule, $"Anonymization method {method} not supported.");
                }


                // Should provide replacement value for substitute rule
                /*
                if (string.Equals(method, AnonymizerMethod.Substitute.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    SubstituteSetting.ValidateRuleSettings(rule);
                }

                if (string.Equals(method, AnonymizerMethod.Perturb.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    PerturbSetting.ValidateRuleSettings(rule);
                }
                if (string.Equals(method, AnonymizerMethod.Generalize.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    GeneralizeSetting.ValidateRuleSettings(rule);
                }
                */
            }

            // Check AES key size is valid (16, 24 or 32 bytes).
            /*
            if (!string.IsNullOrEmpty(config.ParameterConfiguration?.EncryptKey))
            {
                using Aes aes = Aes.Create();
                var encryptKeySize = Encoding.UTF8.GetByteCount(config.ParameterConfiguration.EncryptKey) * 8;
                if (!IsValidKeySize(encryptKeySize, aes.LegalKeySizes))
                {
                    throw new AnonymizerConfigurationErrorsException($"Invalid encrypt key size : {encryptKeySize} bits! Please provide key sizes of 128, 192 or 256 bits.");
                }
            }
            */
        }

        // The following method takes a bit length input and returns whether that length is a valid size
        // validSizes for AES: MinSize=128, MaxSize=256, SkipSize=64
        private bool IsValidKeySize(int bitLength, KeySizes[] validSizes)
        {
            if (validSizes == null)
            {
                return false;
            }

            for (int i = 0; i < validSizes.Length; i++)
            {
                for (int j = validSizes[i].MinSize; j <= validSizes[i].MaxSize; j += validSizes[i].SkipSize)
                {
                    if (j == bitLength)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
