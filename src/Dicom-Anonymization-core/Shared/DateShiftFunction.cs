// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using De_Id_Function_Shared.Exceptions;
using De_Id_Function_Shared.Settings;
using System;
using System.Text;

namespace De_Id_Function_Shared
{
    public class DateShiftFunction
    {
        public DateShiftFunction(DateShiftSetting dateShiftSetting)
        {
            if (dateShiftSetting != null)
            {
                DateShiftKey = dateShiftSetting.DateShiftKey;
                DateShiftKeyPrefix = dateShiftSetting.DateShiftKeyPrefix;
                DateShiftRange = dateShiftSetting.DateShiftRange;
            }

            DateShiftOffset = GetDateShiftValue();
        }

        public string DateShiftKey { get; set; } = string.Empty;

        public string DateShiftKeyPrefix { get; set; } = string.Empty;

        public int DateShiftRange { get; set; } = 50;

        public int DateShiftOffset { get; }

        public string ShiftDate(string inputString, string inputDateTimeFormat = null, string outputDateTimeFormat = null, IFormatProvider provider = null)
        {
            var date = DateTimeUtility.ParseDateTime(inputString, inputDateTimeFormat, provider);

            var outputFormat = outputDateTimeFormat ?? DeIDConfig.OutputDateTimeFormat ?? inputDateTimeFormat ?? DateTimeUtility.GetDateTimeFormat(inputString, null);
            if (string.IsNullOrEmpty(outputFormat))
            {
                throw new DeIDFunctionException();
            }

            return date.AddDays(GetDateShiftValue()).ToString(outputFormat);
        }

        public string ShiftDateTime(string inputString, string inputDateTimeFormat = null, string outputDateTimeFormat = null, IFormatProvider provider = null)
        {
            var date = DateTimeUtility.ParseDateTime(inputString, inputDateTimeFormat, provider);

            var outputFormat = outputDateTimeFormat ?? DeIDConfig.OutputDateTimeFormat ?? inputDateTimeFormat ?? DateTimeUtility.GetDateTimeFormat(inputString, null);
            if (string.IsNullOrEmpty(outputFormat))
            {
                throw new DeIDFunctionException();
            }

            DateTimeOffset newDateTime = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, date.Offset);
            return newDateTime.AddDays(GetDateShiftValue()).ToString(outputFormat);
        }


        public DateTimeOffset ShiftDateTime(DateTimeOffset dateTime)
        {
            DateTimeOffset newDateTime = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Offset);
            return newDateTime.AddDays(GetDateShiftValue());
        }

        private int GetDateShiftValue()
        {
            int offset = 0;
            var bytes = Encoding.UTF8.GetBytes(DateShiftKeyPrefix + DateShiftKey);
            foreach (byte b in bytes)
            {
                offset = ((offset * Constants.DateShiftSeed) + (int)b) % ((2 * DateShiftRange) + 1);
            }

            offset -= DateShiftRange;

            return offset;
        }
    }
}
