using De_Id_Function_Shared.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Dicom.Anonymization
{
    public class Utility
    {
        

        public static DateTimeOffset ParseDicomDate(string date)
        {
            return DateTimeOffset.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static DateTimeObject ParseDicomDateTime(string date)
        {
            Regex DateTimeRegex = new Regex(@"^((?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})(\.(?<millisecond>\d{1,6}))?(?<timeZone>(?<sign>-|\+)(?<timeZoneHour>\d{2})(?<timeZoneMinute>\d{2}))?)(\s*)");
            var matches = DateTimeRegex.Matches(date);
            if (matches.Count != 1)
            {
                throw new Exception();
            }

            var groups = matches[0].Groups;

            int year = groups["year"].Success ? int.Parse(groups["year"].Value) : 0;
            int month = groups["month"].Success ? int.Parse(groups["month"].Value) : 0;
            int day = groups["day"].Success ? int.Parse(groups["day"].Value) : 0;
            int hour = groups["hour"].Success ? int.Parse(groups["hour"].Value) : 0;
            int minute = groups["minute"].Success ? int.Parse(groups["minute"].Value) : 0;
            int second = groups["second"].Success ? int.Parse(groups["second"].Value) : 0;
            int millisecond = groups["millisecond"].Success ? int.Parse(groups["millisecond"].Value) : 0;

            if (groups["timeZone"].Success)
            {
                int timeZoneHour = int.Parse(groups["timeZoneHour"].Value) * int.Parse(groups["sign"].Value + "1");
                int timeZoneMinute = int.Parse(groups["timeZoneMinute"].Value);
                return new DateTimeObject()
                {
                    DateValue = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, new TimeSpan(timeZoneHour, timeZoneMinute, 0)),
                    HasTimeZone = true,
                };
            }
            else
            {
                return new DateTimeObject()
                {
                    DateValue = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, default),
                    HasTimeZone = false,
                };
            }
        }

        public static string GenerateDicomDateString(DateTimeOffset date)
        {
            if (date == null)
            {
                return null;
            }

            return date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static string GenerateDicomDateTimeString(DateTimeObject date)
        {
            if (date == null)
            {
                return null;
            }

            if (date.HasTimeZone == null || !(bool)date.HasTimeZone)
            {
                return date.DateValue.ToString("yyyyMMddhhmmss.ffffff", CultureInfo.InvariantCulture);
            }
            else
            {
                return date.DateValue.ToString("yyyyMMddHHmmss.ffffffzzz", CultureInfo.InvariantCulture).Replace(":", "");
            }
        }

        public static string[] SplitValues(string stringValue)
        {
            return string.IsNullOrEmpty(stringValue)
                    ? Array.Empty<string>()
                    : stringValue.Split('\\');
        }

        public static AgeValue ParseAge(string age)
        {
            Dictionary<string, AgeType> ageTypeMapping = new Dictionary<string, AgeType>
            {
                {"Y", AgeType.Year },
                {"M", AgeType.Month },
                {"W", AgeType.Week },
                {"D", AgeType.Day },
            };

            foreach (var item in ageTypeMapping)
            {
                if (new Regex(@"\d{3}" + item.Key).IsMatch(age))
                {
                    return new AgeValue(uint.Parse(age.Substring(0, 3)), item.Value);
                }
            }

            return null;
        }

        public static string AgeToString(AgeValue? age)
        {
            Dictionary<string, AgeType> ageTypeMapping = new Dictionary<string, AgeType>
            {
                {"Y", AgeType.Year },
                {"M", AgeType.Month },
                {"W", AgeType.Week },
                {"D", AgeType.Day },
            };

            if (age == null)
            {
                return null;
            }

            foreach (var item in ageTypeMapping)
            {
                if (age.AgeType == item.Value)
                {
                    if (age.Age.ToString().Length > 3)
                    {
                        throw new Exception();
                    }

                    return age.Age.ToString().PadLeft(3, '0') + item.Key;
                }
            }

            return null;
        }
    }
}
