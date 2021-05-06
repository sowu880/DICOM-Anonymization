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
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Dicom.Anonymization.Model;
using Dicom.Anonymization.Processors.Model;
using Dicom.Anonymization.Processors.Settings;
using EnsureThat;

namespace Dicom.Anonymization.Processors
{
    public class DateShiftProcessor : IAnonymizationProcessor
    {
        private DicomDateShiftSetting _defaultSetting;

        public DateShiftProcessor(DicomDateShiftSetting defaultSetting)
        {
            _defaultSetting = defaultSetting;
        }

        public DateShiftFunction CreateDateShiftFunction(DicomDateShiftSetting setting)
        {
            return new DateShiftFunction(new DateShiftSetting()
            {
                DateShiftRange = setting.DateShiftRange,
                DateShiftKey = setting.DateShiftKey,
            });
        }

        public void Process(DicomDataset dicomDataset, DicomItem item, DicomBasicInformation basicInfo, IDicomAnonymizationSetting settings = null)
        {
            EnsureArg.IsNotNull(dicomDataset, nameof(dicomDataset));
            EnsureArg.IsNotNull(item, nameof(item));
            EnsureArg.IsNotNull(basicInfo, nameof(basicInfo));

            if (!IsValidItemForDateShift(item))
            {
                throw new AnonymizationOperationException(DicomAnonymizationErrorCode.UnsupportedAnonymizationFunction, $"Dateshift is not supported for {item.ValueRepresentation}");
            }

            var dateShiftSetting = (DicomDateShiftSetting)(settings ?? _defaultSetting);
            var dateShiftFunction = CreateDateShiftFunction(dateShiftSetting);
            dateShiftFunction.DateShiftKeyPrefix = dateShiftSetting.DateShiftScope switch
            {
                DateShiftScope.StudyInstance => basicInfo.StudyInstanceUID ?? string.Empty,
                DateShiftScope.SeriesInstance => basicInfo.StudyInstanceUID ?? string.Empty,
                DateShiftScope.SopInstance => basicInfo.SopInstanceUID ?? string.Empty,
                _ => string.Empty,
            };
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
                }

                dicomDataset.AddOrUpdate(item.ValueRepresentation, item.Tag, results.ToArray());
            }
        }

        public bool IsValidItemForDateShift(DicomItem item)
        {
            var supportedVR = Enum.GetNames(typeof(DateShiftSupportedVR)).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            return supportedVR.Contains(item.ValueRepresentation.Code);
        }
    }
}
