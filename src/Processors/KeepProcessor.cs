using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Dicom;
using Dicom.Anonymization.AnonymizerConfigurations;

namespace Dicom.Anonymization.Processors
{
    public class KeepProcessor : IAnonymizerProcessor
    {
        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
        }
    }
}
