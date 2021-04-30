﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using De_Id_Function_Shared;
using Dicom.Anonymization.Processors.Settings;

namespace Dicom.Anonymization.Processors
{
    public class RedactProcessor : IAnonymizationProcessor
    {
        private DicomRedactSetting _defaultRedactFunction;

        public RedactProcessor(DicomRedactSetting defaultRedactSettings)
        {
            _defaultRedactFunction = defaultRedactSettings;
        }

        public void Process(DicomDataset dicomDataset, DicomItem item, IDicomAnonymizationSetting settings = null)
        {
            var redactSettings = settings == null ? _defaultRedactFunction : (DicomRedactSetting)settings;
            var redactFunction = new RedactFunction(redactSettings);

            var redactedValues = new List<string>() { };
            if (item.ValueRepresentation == DicomVR.AS)
            {
                var values = ((DicomAgeString)item).Get<string[]>();
                foreach (var value in values)
                {
                    var result = Utility.AgeToString(redactFunction.RedactAge(Utility.ParseAge(value)));
                    if (result != null)
                    {
                        redactedValues.Add(result);
                    }
                }
            }

            if (item.ValueRepresentation == DicomVR.DA)
            {
                var values = ((DicomDate)item).Get<string[]>();
                foreach (var value in values)
                {
                    var result = redactFunction.RedactDateTime(Utility.ParseDicomDate(value));
                    if (result != null)
                    {
                        redactedValues.Add(Utility.GenerateDicomDateString((DateTimeOffset)result));
                    }
                }
            }

            if (item.ValueRepresentation == DicomVR.DT)
            {
                var values = ((DicomDateTime)item).Get<string[]>();
                foreach (var value in values)
                {
                    var result = redactFunction.RedactDateTime(Utility.ParseDicomDateTime(value));
                    if (result != null)
                    {
                        redactedValues.Add(Utility.GenerateDicomDateTimeString(result));
                    }
                }
            }

            if (item.ValueRepresentation == DicomVR.SQ)
            {
                dicomDataset.AddOrUpdate<DicomDataset>(DicomVR.SQ, item.Tag);
                return;
            }

            if (redactedValues.Count() != 0)
            {
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, redactedValues.ToArray());
            }
            else
            {
                dicomDataset.AddOrUpdate<string>(item.ValueRepresentation, item.Tag, values: null);
            }
        }
    }
}
