// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Text.RegularExpressions;
using De_Id_Function_Shared.Model;
using De_Id_Function_Shared.Settings;
using EnsureThat;

namespace De_Id_Function_Shared
{
    public class RedactFunction
    {

        private static readonly int s_ageThreshold = 89;
        private static readonly string s_replacementDigit = "0";
        private static readonly int s_initialDigitsCount = 3;

        public RedactSetting Settings { get; set; }

        public RedactFunction(RedactSetting redactSetting)
        {
            Settings = redactSetting;
        }

        public string RedactDateTime(string dateTime, string dateTimeFormat = null, IFormatProvider provider = null)
        {
            if (string.IsNullOrEmpty(dateTime))
            {
                return null;
            }

            if (Settings.EnablePartialDatesForRedact)
            {
                DateTimeOffset date = DateTimeUtility.ParseDateTime(dateTime, dateTimeFormat, provider);
                return DateTimeUtility.IndicateAgeOverThreshold(date) ? null : date.Year.ToString();
            }
            else
            {
                return null;
            }
        }

        public DateTimeOffset? RedactDateTime(DateTimeOffset dateTime)
        {
            EnsureArg.IsNotNull<DateTimeOffset>(dateTime, nameof(dateTime));

            if (Settings.EnablePartialDatesForRedact)
            {
                if (DateTimeUtility.IndicateAgeOverThreshold(dateTime))
                {
                    return null;
                }

                return new DateTimeOffset(dateTime.Year, 1, 1 , 0, 0, 0, default);
            }
            else
            {
                return null;
            }
        }

        public DateTimeObject RedactDateTime(DateTimeObject dateObject)
        {
            EnsureArg.IsNotNull(dateObject, nameof(dateObject));

            if (Settings.EnablePartialDatesForRedact)
            {
                if (DateTimeUtility.IndicateAgeOverThreshold(dateObject.DateValue))
                {
                    return null;
                }

                dateObject.DateValue = new DateTimeOffset(dateObject.DateValue.Year, 1, 1, 0, 0, 0, dateObject.HasTimeZone == null || !(bool)dateObject.HasTimeZone ? new TimeSpan(0, 0, 0) : dateObject.DateValue.Offset);
                return dateObject;
            }
            else
            {
                return null;
            }
        }

        public int? RedactAge(int age)
        {
            if (Settings.EnablePartialAgeForRedact)
            {
                if (age > s_ageThreshold)
                {
                    return null;
                }

                return age;
            }
            else
            {
                return null;
            }
        }

        public decimal? RedactAge(decimal? age)
        {
            if (age == null)
            {
                return null;
            }

            if (Settings.EnablePartialAgeForRedact)
            {
                if (age > s_ageThreshold)
                {
                    return null;
                }

                return age;
            }
            else
            {
                return null;
            }
        }

        public AgeValue RedactAge(AgeValue age)
        {
            if (Settings.EnablePartialAgeForRedact)
            {
                if (age.AgeToYearsOld() > s_ageThreshold)
                {
                    return null;
                }

                return age;
            }
            else
            {
                return null;
            }
        }

        public string RedactPostalCode(string postalCode)
        {
            if (Settings.EnablePartialZipCodesForRedact)
            {
                if (Settings.RestrictedZipCodeTabulationAreas != null && Settings.RestrictedZipCodeTabulationAreas.Any(x => postalCode.StartsWith(x)))
                {
                    postalCode = Regex.Replace(postalCode, @"\d", s_replacementDigit);
                }
                else if (postalCode.Length >= s_initialDigitsCount)
                {
                    var suffix = postalCode[s_initialDigitsCount..];
                    postalCode = $"{postalCode.Substring(0, s_initialDigitsCount)}{Regex.Replace(suffix, @"\d", s_replacementDigit)}";
                }

                return postalCode;
            }
            else
            {
                return null;
            }
        }

    }
}
