using De_Id_Function_Shared;
using De_Id_Function_Shared.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace De.ID.Function.Shared.UnitTests
{
    public class DateShiftTests
    {

        public static IEnumerable<object[]> GetDateStringForDateShift()
        {
            yield return new object[] { "2015-02-07", DateTime.Parse("2014-12-19"), DateTime.Parse("2015-03-29") };
            yield return new object[] { "2020-01-17", DateTime.Parse("2019-11-28"), DateTime.Parse("2020-03-07") };
            yield return new object[] { "1998-10-02", DateTime.Parse("1998-08-13"), DateTime.Parse("1998-11-21") };
            yield return new object[] { "1975-12-26", DateTime.Parse("1975-11-06"), DateTime.Parse("1976-02-14") };
        }

        public static IEnumerable<object[]> GetDateStringForDateShiftWithPrefix()
        {
            yield return new object[] { "2015-02-07", "1975-11-06" };
            yield return new object[] { "2020-01-17", "1998-11-21" };
            yield return new object[] { "1998-10-02", "2019-11-28" };
            yield return new object[] { "1975-12-26", "2020-03-07" };
        }


        public static IEnumerable<object[]> GetDateTimeStringForDateShift()
        {
            yield return new object[] { "2015-02-07", DateTimeOffset.Parse("2014-12-19"), DateTimeOffset.Parse("2015-03-29") };
            yield return new object[] { "2015-02-07T13:28:17-05:00", DateTimeOffset.Parse("2014-12-19T00:00:00-05:00"), DateTimeOffset.Parse("2015-03-29T00:00:00-05:00") };
            yield return new object[] { "1998-10-02", DateTimeOffset.Parse("1998-08-13"), DateTimeOffset.Parse("1998-11-21") };
            yield return new object[] { "1998-10-02T08:47:25+08:00", DateTimeOffset.Parse("1998-08-13T00:00:00+08:00"), DateTimeOffset.Parse("1998-11-21T00:00:00+08:00") };
        }

        public static IEnumerable<object[]> GetDateTimeStringForDateShiftFormatTest()
        {
            yield return new object[] { "dummy", "2015-02-07", "2015-01-17T00:00:00+08:00" };  // mark
            yield return new object[] { "dummy", "2015-02-07T13:28:17-05:00", "2015-01-17T00:00:00-05:00" };
            yield return new object[] { "dummy", "2015-02-07T13:28:17+05:00", "2015-01-17T00:00:00+05:00" };
            yield return new object[] { "dummy", "2015-02-07T13:28:17Z", "2015-01-17T00:00:00+00:00" }; // mark
            yield return new object[] { "dummy", "2015-02-07T13:28:17.12345-05:00", "2015-01-17T00:00:00-05:00" };
        }

        public static IEnumerable<object[]> GetDateTimeForDateShift()
        {
            yield return new object[] { DateTimeOffset.Parse("2015-02-07"), DateTimeOffset.Parse("2014-12-19"), DateTimeOffset.Parse("2015-03-29") };
            yield return new object[] { DateTimeOffset.Parse("2015-02-07T13:28:17-05:00"), DateTimeOffset.Parse("2014-12-19T00:00:00-05:00"), DateTimeOffset.Parse("2015-03-29T00:00:00-05:00") };
            yield return new object[] { DateTimeOffset.Parse("1998-10-02"), DateTimeOffset.Parse("1998-08-13"), DateTimeOffset.Parse("1998-11-21") };
            yield return new object[] { DateTimeOffset.Parse("1998-10-02T08:47:25+08:00"), DateTimeOffset.Parse("1998-08-13T00:00:00+08:00"), DateTimeOffset.Parse("1998-11-21T00:00:00+08:00") };
        }
        /*
        [Theory]
        [MemberData(nameof(GetDateStringForDateShift))]
        public void GivenADate_WhenDateShift_ThenDateShouldBeShifted(string date, DateTime minExpectedDate, DateTime maxExpectedDate)
        {
            var processResult = DateShiftFunction.ShiftDate(date, new DateShiftSetting() { DateShiftKey = string.Empty });

            Assert.True(minExpectedDate <= DateTime.Parse(processResult));
            Assert.True(maxExpectedDate >= DateTime.Parse(processResult));
        }

        [Theory]
        [MemberData(nameof(GetDateStringForDateShiftWithPrefix))]
        public void GivenADate_WhenDateShiftWithSamePrefix_ThenSameAmountShouldBeShifted(string date1, string date2)
        {
            var processResult1 = DateShiftFunction.ShiftDate(date1, new DateShiftSetting() { DateShiftKey = "123", DateShiftKeyPrefix = "filename" }, null);
            var offset1 = DateTime.Parse(processResult1).Subtract(DateTime.Parse(date1.ToString()));

            var processResult2 = DateShiftFunction.ShiftDate(date2, new DateShiftSetting() { DateShiftKey = "123", DateShiftKeyPrefix = "filename" }, null);
            var offset2 = DateTime.Parse(processResult2).Subtract(DateTime.Parse(date2.ToString()));

            Assert.Equal(offset1.Days, offset2.Days);
        }

        [Theory]
        [MemberData(nameof(GetDateTimeStringForDateShift))]
        public void GivenADateTimeString_WhenDateShift_ThenDateTimeShouldBeShifted(string dateTime, DateTimeOffset minExpectedDateTime, DateTimeOffset maxExpectedDateTime)
        {
            var processResult = DateShiftFunction.ShiftDateTime(dateTime, new DateShiftSetting() { DateShiftKey = Guid.NewGuid().ToString("N") }, null);

            Assert.True(minExpectedDateTime <= DateTimeOffset.Parse(processResult));
            Assert.True(maxExpectedDateTime >= DateTimeOffset.Parse(processResult));
        }

        [Theory]
        [MemberData(nameof(GetDateTimeStringForDateShiftFormatTest))]
        public void GivenADateTime_WhenDateShift_ThenDateTimeFormatShouldNotChange(string dateShiftKey, string dateTime, string expectedDateTimeString)
        {
            var processResult = DateShiftFunction.ShiftDateTime(dateTime, new DateShiftSetting() { DateShiftKey = dateShiftKey }, null);
            Assert.Equal(expectedDateTimeString, processResult);
        }
        
        [Theory]
        [MemberData(nameof(GetDateTimeForDateShift))]
        public void GivenADateTime_WhenDateShift_ThenDateTimeShouldBeShifted(DateTimeOffset dateTime, DateTimeOffset minExpectedDateTime, DateTimeOffset maxExpectedDateTime)
        {
            var processResult = DateShiftFunction.ShiftDateTime(dateTime, new DateShiftSetting() { DateShiftKey = Guid.NewGuid().ToString("N") });

            Assert.True(minExpectedDateTime <= processResult);
            Assert.True(maxExpectedDateTime >= processResult);
        }
        */
    }
}
