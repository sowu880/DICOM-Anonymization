using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Processors
{
    public interface IAnonymizationProcessor
    {
        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null);
    }
}
