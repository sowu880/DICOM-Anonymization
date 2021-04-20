using System;
using System.Collections.Generic;
using System.Text;

namespace De_Id_Function_Shared.Settings
{
    public class DateShiftSetting
    {
        public int DateShiftRange { get; set; } = 50;

        public string DateShiftKey { get; set; }

        public string DateShiftKeyPrefix { get; set; } = string.Empty;

    }
}
