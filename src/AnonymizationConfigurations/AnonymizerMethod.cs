namespace Dicom.Anonymization.AnonymizationConfigurations
{
    public enum AnonymizerMethod
    {
        Redact,
        DateShift,
        CryptoHash,
        Keep,
        Replace,
        Perturb,
        Encrypt,
    }
}
