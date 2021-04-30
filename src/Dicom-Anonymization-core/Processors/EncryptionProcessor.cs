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
    public class EncryptionProcessor : IAnonymizationProcessor
    {
        private readonly DicomEncryptionSetting _defaultSetting;

        public EncryptionProcessor(DicomEncryptionSetting defaultSetting)
        {
            EnsureArg.IsNotNull(defaultSetting, nameof(defaultSetting));

            _defaultSetting = defaultSetting;
        }

        public void Process(DicomDataset dicomDataset, DicomItem item, IDicomAnonymizationSetting settings = null)
        {
            var encryptSetting = settings == null ? _defaultSetting : settings;
            var key = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(((DicomEncryptionSetting)encryptSetting).EncryptKey) ? Guid.NewGuid().ToString("N") : ((DicomEncryptionSetting)encryptSetting).EncryptKey);
            var encoding = DicomEncoding.Default;
            try
            {
                if (item is DicomStringElement)
                {
                    var encryptedValues = ((DicomStringElement)item).Get<string[]>().Where(x => !string.IsNullOrEmpty(x)).Select(x => Convert.ToBase64String(EncryptFunction.EncryptContentWithAES(encoding.GetBytes(x), key)));
                    if (encryptedValues.Count() != 0)
                    {
                        dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, encryptedValues.ToArray());
                    }
                }
                else if (item is DicomOtherByte)
                {
                    var valueBytes = ((DicomOtherByte)item).Get<byte[]>();
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
            catch (Exception ex)
            {
                if (ex is DicomValidationException)
                {
                    throw new Exception($"Invalid encryption operation for item {item}", ex);
                }

                throw;
            }
        }
    }
}
