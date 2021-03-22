namespace Dicom.Anonymization.AnonymizerConfigurations
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
