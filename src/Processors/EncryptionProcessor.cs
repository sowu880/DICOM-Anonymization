using De_Id_Function_Shared;
using Dicom.Anonymization.AnonymizerConfigurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dicom.Anonymization.Processors
{
    public class EncryptionProcessor : IAnonymizerProcessor
    {
        private readonly byte[] _key;

        public EncryptionProcessor(ParameterConfiguration parameter)
        {
            _key = Encoding.UTF8.GetBytes(parameter.EncryptKey);
        }

        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
            var encoding = DicomEncoding.Default;
            if (item is DicomMultiStringElement)
            {
                var encryptedValues = ((DicomMultiStringElement)item).Get<string[]>().Select(x => Convert.ToBase64String(EncryptFunction.EncryptContentWithAES(encoding.GetBytes(x), _key)));
                dicomDataset.AddOrUpdate(item.Tag, encryptedValues.ToArray());
            }
            else if (item is DicomStringElement)
            {
                var value = ((DicomMultiStringElement)item).Get<string>();
                var encryptedValue = Convert.ToBase64String(EncryptFunction.EncryptContentWithAES(encoding.GetBytes(value), _key));
                dicomDataset.AddOrUpdate(item.Tag, encryptedValue);
            }
            else
            {
                var valueBytes = ((DicomElement)item).Get<byte[]>();
                var encryptesBytes = EncryptFunction.EncryptContentWithAES(valueBytes, _key);
                dicomDataset.AddOrUpdate(item.Tag, encryptesBytes);
            }
        }
    }
}
