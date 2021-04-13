using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.AnonymizationConfigurations
{
    public class DicomTagList
    {
        public List<string> TagList { get; set; }

        public List<string> ExceptionList { get;set; }
    }
}
