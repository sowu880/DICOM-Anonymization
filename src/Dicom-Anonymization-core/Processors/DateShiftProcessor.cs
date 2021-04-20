// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using De_Id_Function_Shared;
using De_Id_Function_Shared.Settings;
using Dicom.Anonymization.AnonymizationConfigurations;
using Dicom.Anonymization.Model;

namespace Dicom.Anonymization.Processors
{
    public class DateShiftProcessor : IAnonymizationProcessor
    {
        public DateShiftProcessor(DicomDateShiftSetting defaultSetting)
        {
            DefaultDateShiftFunction = CreateDateShiftFunction(defaultSetting);
            DefaultDateShiftScope = defaultSetting.DateShiftScope;
        }

        public DateShiftScope DefaultDateShiftScope { get; set; }

        public DicomDateShiftSetting DefaultSetting { get; set; }

        public DateShiftFunction DefaultDateShiftFunction { get; set; }

        public DateShiftFunction CreateDateShiftFunction(DicomDateShiftSetting setting)
        {
            return new DateShiftFunction(new DateShiftSetting()
            {
                DateShiftRange = setting.DateShiftRange,
                DateShiftKey = setting.DateShiftKey,
            });
        }

        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
            DateShiftFunction dateShiftFunction = DefaultDateShiftFunction;
            DateShiftScope dateShiftScope = DefaultDateShiftScope;
            if (settings != null)
            {
                var dateShiftSetting = DicomDateShiftSetting.CreateFromJson(settings);
                dateShiftFunction = CreateDateShiftFunction(dateShiftSetting);
                dateShiftScope = dateShiftSetting.DateShiftScope;
            }

            if (dateShiftScope == DateShiftScope.StudyInstance)
            {
                dateShiftFunction.DateShiftKeyPrefix = dicomDataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            }
            else if (dateShiftScope == DateShiftScope.SeriesInstance)
            {
                dateShiftFunction.DateShiftKeyPrefix = dicomDataset.GetSingleValue<string>(DicomTag.SeriesInstanceUID);
            }
            else if (dateShiftScope == DateShiftScope.SopInstance)
            {
                dateShiftFunction.DateShiftKeyPrefix = dicomDataset.GetSingleValue<string>(DicomTag.SOPInstanceUID);
            }

            if (item.ValueRepresentation == DicomVR.DA)
            {
                var values = ((DicomDate)item).Get<string[]>().Select(x => dateShiftFunction.ShiftDateTime(Utility.ParseDicomDate(x))).Where(x => !DateTimeUtility.IndicateAgeOverThreshold(x));
                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, values.Select(x => Utility.GenerateDicomDateString(x)).ToArray());
            }
            else if (item.ValueRepresentation == DicomVR.DT)
            {
                var results = new List<string>();
                var values = (item as DicomDateTime).Get<string[]>().ToList();
                foreach (var value in values)
                {
                    var dateObject = Utility.ParseDicomDateTime(value);
                    dateObject.DateValue = dateShiftFunction.ShiftDateTime(dateObject.DateValue);
                    if (!DateTimeUtility.IndicateAgeOverThreshold(dateObject.DateValue))
                    {
                        results.Add(Utility.GenerateDicomDateTimeString(dateObject));
                    }

                    results.Add(value);
                }

                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, results.ToArray());
            }
            else
            {
                throw new Exception($"Dateshift is not supported for {item.ValueRepresentation}");
            }
        }
    }
}
