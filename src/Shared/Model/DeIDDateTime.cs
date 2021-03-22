using System;
using System.Collections.Generic;
using System.Text;

namespace De_Id_Function_Shared.Model
{
    public class DeIDDateTime
    {
        public DateTimeOffset dateTimeOffset;

        public int? Year { get; set; }

        public int? Month { get; set; }

        public int? Day { get; set; }

        public DeIDDateTime(string input)
        {
            var dateTime = DateTimeOffset.Parse(input);
            if (input.Contains(dateTime.Year.ToString()))
            {
                var yearIndex = input.IndexOf(dateTime.Year.ToString());
                input.Remove(yearIndex, dateTime.Year.ToString().Length);
            }
            else
            {
                return;
            }
        }
    }
}
