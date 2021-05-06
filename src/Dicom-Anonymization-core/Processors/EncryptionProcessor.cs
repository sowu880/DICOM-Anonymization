// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
    public class EncryptionProcessor : IAnonymizationProcessor
    {
        private readonly DicomEncryptionSetting _defaultSetting;

        public EncryptionProcessor(DicomEncryptionSetting defaultSetting)
        {
            EnsureArg.IsNotNull(defaultSetting, nameof(defaultSetting));

            _defaultSetting = defaultSetting;
        }

        public void Process(DicomDataset dicomDataset, DicomItem item, DicomBasicInformation basicInfo = null, IDicomAnonymizationSetting settings = null)
        {
            EnsureArg.IsNotNull(dicomDataset, nameof(dicomDataset));
            EnsureArg.IsNotNull(item, nameof(item));

            if (!IsValidItemForEncrypt(item))
            {
                throw new AnonymizationOperationException(DicomAnonymizationErrorCode.UnsupportedAnonymizationFunction, $"Encrypt is not supported for {item.ValueRepresentation}");
            }

            var encryptSetting = (DicomEncryptionSetting)(settings ?? _defaultSetting);
            var key = Encoding.UTF8.GetBytes(string.IsNullOrEmpty(encryptSetting.EncryptKey) ? Guid.NewGuid().ToString("N") : encryptSetting.EncryptKey);
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
            }
            catch (Exception ex)
            {
                if (ex is DicomValidationException)
                {
                    throw new AnonymizationOperationException(DicomAnonymizationErrorCode.UnsupportedAnonymizationFunction, $"Encrypt is not supported for {item.ValueRepresentation}");
                }

                throw;
            }
        }

        public bool IsValidItemForEncrypt(DicomItem item)
        {
            var supportedVR = Enum.GetNames(typeof(EncryptSupportedVR)).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            return supportedVR.Contains(item.ValueRepresentation.Code) || item is DicomFragmentSequence;
        }
    }
}
