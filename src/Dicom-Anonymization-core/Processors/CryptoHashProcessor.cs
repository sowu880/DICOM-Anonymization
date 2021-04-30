using De_Id_Function_Shared;
using Dicom.Anonymization.AnonymizationConfigurations;
using Dicom.Anonymization.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FellowOakDicom.IO;
using Dicom.IO.Buffer;
using EnsureThat;
using Dicom.Anonymization.Processors.Settings;

namespace Dicom.Anonymization.Processors
{
    public class CryptoHashProcessor : IAnonymizationProcessor
    {
        private DicomCryptoHashSetting _defaultSetting;

        public CryptoHashProcessor(DicomCryptoHashSetting defaultSetting)
        {
            EnsureArg.IsNotNull(defaultSetting, nameof(defaultSetting));

            _defaultSetting = defaultSetting;
        }

        public void Process(DicomDataset dicomDataset, DicomItem item, IDicomAnonymizationSetting settings = null)
        {
            EnsureArg.IsNotNull(dicomDataset, nameof(dicomDataset));
            EnsureArg.IsNotNull(item, nameof(item));

            IsValidItemForCryptoHash(item);

            var cryptoHashSetting = settings == null ? _defaultSetting : settings;
            var cryptoHashKey = Encoding.UTF8.GetBytes(((DicomCryptoHashSetting)cryptoHashSetting).CryptoHashKey);

            var encoding = Encoding.UTF8;
            if (item is DicomStringElement)
            {
                var encryptedValues = ((DicomStringElement)item).Get<string[]>().Select(x => GetCryptoHashString(x, cryptoHashKey));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, encryptedValues.ToArray());
            }
            else if (item is DicomOtherByte)
            {
                var valueBytes = ((DicomOtherByte)item).Get<byte[]>();
                var encryptesBytes = CryptoHashFunction.ComputeHmacSHA256Hash(valueBytes, cryptoHashKey);
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, encryptesBytes);
            }
            else if (item is DicomFragmentSequence)
            {
                List<byte[]> results = new List<byte[]>();
                var enumerator = ((DicomFragmentSequence)item).GetEnumerator();

                var element = item.ValueRepresentation == DicomVR.OW
                    ? (DicomFragmentSequence)new DicomOtherWordFragment(item.Tag)
                    : new DicomOtherByteFragment(item.Tag);

                while (enumerator.MoveNext())
                {
                    element.Fragments.Add(new MemoryByteBuffer(CryptoHashFunction.ComputeHmacSHA256Hash(enumerator.Current.Data, cryptoHashKey)));
                }

                dicomDataset.AddOrUpdate(element);
            }
        }

        public void IsValidItemForCryptoHash(DicomItem item)
        {
            if (item.ValueRepresentation.IsString)
            {
                if (item.ValueRepresentation == DicomVR.UI)
                {
                    throw new Exception($"Invalid crypto hash operation for item {item}.");
                }

                if (item.ValueRepresentation.MaximumLength < 64 && item.ValueRepresentation.MaximumLength > 0)
                {
                    throw new Exception($"Invalid crypto hash operation for item {item}");
                }
            }
            else if (item.ValueRepresentation != DicomVR.OB && !(item is DicomFragmentSequence))
            {
                throw new Exception($"Invalid crypto hash operation for item {item}");
            }
        }

        public string GetCryptoHashString(string input, byte[] cryptoHashKey)
        {
            var resultBytes = CryptoHashFunction.ComputeHmacSHA256Hash(Encoding.UTF8.GetBytes(input), cryptoHashKey);
            return resultBytes == null ? null : string.Concat(resultBytes.Select(b => b.ToString("x2")));
        }
    }
}
