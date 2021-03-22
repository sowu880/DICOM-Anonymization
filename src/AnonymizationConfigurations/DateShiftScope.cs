using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dicom.Anonymization.AnonymizerConfigurations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DateShiftScope
    {
        [EnumMember(Value = "StudyInstance")]
        StudyInstance,
        [EnumMember(Value = "SeriesInstance")]
        SeriesInstance,
        [EnumMember(Value = "SopInstance")]
        SopInstance,
    }
}
