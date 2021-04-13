using De_Id_Function_Shared;
using Dicom.Anonymization.AnonymizationConfigurations;
using Dicom.Anonymization.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FellowOakDicom.IO;
using Dicom.IO.Buffer;

namespace Dicom.Anonymization.Processors
{
    public class CryptoHashProcessor : IAnonymizerProcessor
    {
        private readonly byte[] _cryptoHashKey;
        private readonly Func<string, string> _cryptoHashFunction;

        public CryptoHashProcessor(DicomCryptoHashSetting defaultSetting)
        {
            _cryptoHashKey = Encoding.UTF8.GetBytes(defaultSetting.CryptoHashKey);
        }

        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
            var encoding = Encoding.UTF8;
            if (item is DicomMultiStringElement)
            {
                var encryptedValues = ((DicomMultiStringElement)item).Get<string[]>().Select(GetCryptoHashString);
                dicomDataset.AddOrUpdate(item.Tag, encryptedValues.ToArray());
            }
            else if (item is DicomStringElement)
            {
                var value = ((DicomMultiStringElement)item).Get<string>();
                var encryptedValue = GetCryptoHashString(value);
                dicomDataset.AddOrUpdate(item.Tag, encryptedValue);
            }
            else if (item is DicomElement)
            {
                var valueBytes = ((DicomElement)item).Get<byte[]>();
                var encryptesBytes = CryptoHashFunction.ComputeHmacSHA256Hash(valueBytes, _cryptoHashKey);
                dicomDataset.AddOrUpdate(item.Tag, encryptesBytes);
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
                    element.Fragments.Add(new MemoryByteBuffer(CryptoHashFunction.ComputeHmacSHA256Hash(enumerator.Current.Data, _cryptoHashKey)));
                }

                dicomDataset.AddOrUpdate(element);
            }
            else
            {
                Console.WriteLine($"Invalid encryption operation for item {item}");
            }
        }

        public string GetCryptoHashString(string input)
        {
            var resultBytes = CryptoHashFunction.ComputeHmacSHA256Hash(Encoding.UTF8.GetBytes(input), _cryptoHashKey);
            return resultBytes == null ? null : string.Concat(resultBytes.Select(b => b.ToString("x2")));
        }
    }
}
