using System;
using System.Collections.Generic;
using System.Text;

namespace De_Id_Function_Shared.Model
{
    public class DateTimeObject
    {
        public DateTimeOffset DateValue { get; set; }

        public bool? HasTimeZone { get; set; } = null;

        public bool? HasMillionSecond { get; set; } = null;

        public bool? HasSecond { get; set; } = null;

        public bool? HasHour { get; set; } = null;

        public bool? HasDay { get; set; } = null;

        public bool? HasMonth { get; set; } = null;

        public bool? HasYear { get; set; } = null;
    }
}
