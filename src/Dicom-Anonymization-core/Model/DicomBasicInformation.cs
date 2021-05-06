using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Model
{
    public class DicomBasicInformation
    {
        public string StudyInstanceUID { get; set; }

        public string SeriesInstanceUID { get; set; }

        public string SopInstanceUID { get; set; }
    }
}
