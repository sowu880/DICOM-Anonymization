using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Dicom;
using Dicom.Anonymization.AnonymizationConfigurations;
using Dicom.Anonymization.Processors.Settings;

namespace Dicom.Anonymization.Processors
{
    public class KeepProcessor : IAnonymizationProcessor
    {
        public void Process(DicomDataset dicomDataset, DicomItem item, IDicomAnonymizationSetting settings = null)
        {
        }
    }
}
