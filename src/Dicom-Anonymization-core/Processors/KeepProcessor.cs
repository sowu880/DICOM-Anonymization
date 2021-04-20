using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Dicom;
using Dicom.Anonymization.AnonymizationConfigurations;

namespace Dicom.Anonymization.Processors
{
    public class KeepProcessor : IAnonymizationProcessor
    {
        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
        }
    }
}
