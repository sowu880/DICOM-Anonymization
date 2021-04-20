namespace Dicom.Anonymization.AnonymizationConfigurations
{
    public enum AnonymizationMethod
    {
        Redact,
        DateShift,
        CryptoHash,
        Keep,
        Perturb,
        Encrypt,
        Remove,
        RefreshUID,
        Substitute,
    }
}
