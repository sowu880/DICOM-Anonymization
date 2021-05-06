// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using De_Id_Function_Shared;
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Dicom.Anonymization.Model;
using Dicom.Anonymization.Processors.Model;
using Dicom.Anonymization.Processors.Settings;
using Dicom.IO.Buffer;
using EnsureThat;

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

            if (!IsValidItemForCryptoHash(item))
            {
                throw new AnonymizationOperationException(DicomAnonymizationErrorCode.UnsupportedAnonymizationFunction, $"CryptoHash is not supported for {item.ValueRepresentation}");
            }

            var cryptoHashSetting = (DicomCryptoHashSetting)(settings ?? _defaultSetting);
            var cryptoHashKey = Encoding.UTF8.GetBytes(cryptoHashSetting.CryptoHashKey);

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

        public bool IsValidItemForCryptoHash(DicomItem item)
        {
            var supportedVR = Enum.GetNames(typeof(CryptoHashSupportedVR)).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            return supportedVR.Contains(item.ValueRepresentation.Code) || item is DicomFragmentSequence;
        }

        public string GetCryptoHashString(string input, byte[] cryptoHashKey)
        {
            var resultBytes = CryptoHashFunction.ComputeHmacSHA256Hash(Encoding.UTF8.GetBytes(input), cryptoHashKey);
            return resultBytes == null ? null : string.Concat(resultBytes.Select(b => b.ToString("x2")));
        }
    }
}
