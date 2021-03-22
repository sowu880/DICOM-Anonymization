// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using De_Id_Function_Shared;
using De_Id_Function_Shared.Settings;
using Dicom.Anonymization.AnonymizerConfigurations;

namespace Dicom.Anonymization.Processors
{
    public class DateShiftProcessor : IAnonymizerProcessor
    {
        public DateShiftProcessor(ParameterConfiguration parameter)
        {
            DateShiftFunction = new DateShiftFunction(new DateShiftSetting()
            {
                DateShiftRange = parameter.DateShiftRange,
                DateShiftKey = parameter.DateShiftKey,
            });
            DateShiftScope = parameter.DateShiftScope;
        }

        public DateShiftScope DateShiftScope { get; set; }

        public DateShiftFunction DateShiftFunction { get; set; }

        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
            if (DateShiftScope == DateShiftScope.StudyInstance)
            {
                DateShiftFunction.DateShiftKeyPrefix = dicomDataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            }
            else if (DateShiftScope == DateShiftScope.SeriesInstance)
            {
                DateShiftFunction.DateShiftKeyPrefix = dicomDataset.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
            }
            else if (DateShiftScope == DateShiftScope.SopInstance)
            {
                DateShiftFunction.DateShiftKeyPrefix = dicomDataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);
            }

            if (item.ValueRepresentation == DicomVR.DA)
            {
                var values = ((DicomDate)item).Get<string[]>().Select(x => DateShiftFunction.ShiftDateTime(Utility.ParseDicomDate(x))).Where(x => !DateTimeUtility.IndicateAgeOverThreshold(x));
                dicomDataset.AddOrUpdate(item.Tag, values.Select(x => Utility.GenerateDicomDateString(x)).ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.DT)
            {
                var results = new List<string>();
                var values = ((DicomDate)item).Get<string[]>();
                foreach (var value in values)
                {
                    var dateObject = Utility.ParseDicomDateTime(value);
                    dateObject.DateValue = DateShiftFunction.ShiftDateTime(dateObject.DateValue);
                    if (!DateTimeUtility.IndicateAgeOverThreshold(dateObject.DateValue))
                    {
                        results.Add(Utility.GenerateDicomDateTimeString(dateObject));
                    }
                }

                if (results.Count != 0)
                {
                    dicomDataset.AddOrUpdate(item.Tag, results.ToArray());
                }
            }
            else
            {
                throw new Exception($"Dateshift is not supported for {item.ValueRepresentation}");
            }
        }
    }
}
