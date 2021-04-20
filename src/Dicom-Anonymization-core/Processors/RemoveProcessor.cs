using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Processors
{
    public class RemoveProcessor : IAnonymizationProcessor
    {
        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
            dicomDataset.Remove(item.Tag);
        }
    }
}
