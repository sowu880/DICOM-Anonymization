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
    public class EncryptionProcessor : IAnonymizationProcessor
    {
        private readonly DicomEncryptionSetting _defaultSetting;

        public EncryptionProcessor(DicomEncryptionSetting defaultSetting)
        {
            _defaultSetting = defaultSetting;
        }

        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
            var dateShiftSetting = settings == null ? _defaultSetting : DicomEncryptionSetting.CreateFromJson(settings);
            var key = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(dateShiftSetting.EncryptKey) ? Guid.NewGuid().ToString("N") : dateShiftSetting.EncryptKey);
            var encoding = DicomEncoding.Default;
            if (item is DicomMultiStringElement)
            {
                var encryptedValues = ((DicomMultiStringElement)item).Get<string[]>().Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToBase64String(EncryptFunction.EncryptContentWithAES(encoding.GetBytes(x), key)));
                if (encryptedValues.Count() != 0)
                {
                    dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, encryptedValues.ToArray());
                }
            }
            else if (item is DicomStringElement)
            {
                var value = ((DicomMultiStringElement)item).Get<string>();
                if (!string.IsNullOrEmpty(value))
                {
                    var encryptedValue = Convert.ToBase64String(EncryptFunction.EncryptContentWithAES(encoding.GetBytes(value), key));
                    dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, encryptedValue);
                }
            }
            else if (item is DicomElement)
            {
                var valueBytes = ((DicomElement)item).Get<byte[]>();
                var encryptesBytes = EncryptFunction.EncryptContentWithAES(valueBytes, key);
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
                    element.Fragments.Add(new MemoryByteBuffer(EncryptFunction.EncryptContentWithAES(enumerator.Current.Data, key)));
                }

                dicomDataset.AddOrUpdate(element);
            }
            else
            {
                throw new Exception($"Invalid encryption operation for item {item}");
            }
        }
    }
}
