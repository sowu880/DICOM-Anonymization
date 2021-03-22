using System;
using System.Collections.Generic;
using System.Text;

namespace De_Id_Function_Shared
{
    public class FixedLengthString
    {
        private string value;
        private int length;
        private string sourceValue;

        public FixedLengthString(int length)
            : this(length, string.Empty)
        {
        }

        public FixedLengthString(string sourceValue)
        {
            length = sourceValue.Length;
            this.sourceValue = sourceValue;
        }

        public FixedLengthString(int length, string sourceValue)
        {
            this.length = length;
            this.sourceValue = sourceValue;
            if (sourceValue.Length > length)
            {
                value = sourceValue.Substring(0, length);
            }
            else
            {
                value = sourceValue + new string((char)0, length - sourceValue.Length);
            }
        }

        public override string ToString()
        {
            return value;
        }

        public int GetLength()
        {
            return length;
        }
    }
}
