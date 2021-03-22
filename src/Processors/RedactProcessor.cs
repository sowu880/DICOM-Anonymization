// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using De_Id_Function_Shared;
using De_Id_Function_Shared.Model;
using De_Id_Function_Shared.Settings;
using Dicom;
using Dicom.Anonymization.AnonymizerConfigurations;

namespace Dicom.Anonymization.Processors
{
    public class RedactProcessor : IAnonymizerProcessor
    {
        public RedactProcessor(ParameterConfiguration parameter)
        {
            RedactFunction = new RedactFunction(new RedactSetting()
            {
                EnablePartialAgeForRedact = parameter.EnablePartialAgesForRedact,
                EnablePartialDatesForRedact = parameter.EnablePartialDatesForRedact,
            });
        }

        public RedactFunction RedactFunction { get; set; }

        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
            // var values = Utility.SplitValues(Encoding.UTF8.GetString(((DicomElement)item).Buffer.Data));

            var redactedValues = new List<string>() { };
            if (item.ValueRepresentation == DicomVR.AS)
            {
                var values = ((DicomAgeString)item).Get<string[]>();
                foreach (var value in values)
                {
                    var result = Utility.AgeToString(RedactFunction.RedactAge(Utility.ParseAge(value)));
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
                    var result = RedactFunction.RedactDateTime(Utility.ParseDicomDate(value));
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
                    var result = RedactFunction.RedactDateTime(Utility.ParseDicomDateTime(value));
                    if (result != null)
                    {
                        redactedValues.Add(Utility.GenerateDicomDateTimeString(result));
                    }
                }
            }

            dicomDataset.Remove(item.Tag);

            if (redactedValues.Count() != 0)
            {
                dicomDataset.AddOrUpdate(item.Tag, redactedValues.ToArray());
            }

        }
    }
}
