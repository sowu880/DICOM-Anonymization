using System;
using System.Collections.Generic;
using System.Text;

namespace De_Id_Function_Shared.Model
{
    public class AgeValue
    {
        public AgeValue(uint age, AgeType ageType)
        {
            Age = age;
            AgeType = ageType;
        }

        public uint Age { get; } = 0;

        public AgeType AgeType { get; }

        public decimal? AgeToYearsOld()
        {
            if (AgeType == AgeType.Year)
            {
                return Age;
            }
            else if (AgeType == AgeType.Month)
            {
                return Age / 12;
            }
            else if (AgeType == AgeType.Week)
            {
                return Age / 52;
            }
            else if (AgeType == AgeType.Day)
            {
                return Age / 365;
            }

            return null;
        }
    }
}
