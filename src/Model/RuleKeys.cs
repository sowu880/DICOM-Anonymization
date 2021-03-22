using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Model
{
    internal static class RuleKeys
    {
        //perturb
        internal const string ReplaceWith = "replaceWith";
        internal const string RangeType = "rangeType";
        internal const string RoundTo = "roundTo";
        internal const string Span = "span";
        internal const string Distribution = "normal";

        //generalize
        internal const string Cases = "cases";
        internal const string OtherValues = "otherValues";
    }
}
