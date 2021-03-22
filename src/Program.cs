using Dicom;
using Dicom.Anonymization.AnonymizerConfigurations;
using Dicom.Anonymization.Processors;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.IO;
using System.Linq;

namespace Dicom.Anonymization
{
    public class Program
    {

        private readonly Dictionary<string, IAnonymizerProcessor> _processors;

        /*
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            // TestRedactAge();
            // TestRedactDateTime();
            //TestPerturb();
            
            string fileName = "image-00000.dcm";
            DicomFile dicomFile = await DicomFile.OpenAsync(fileName).ConfigureAwait(false);

            //FilterByVR(dicomFile, "PN");
            var engine = new AnonimizerEngine();
            engine.Anonymize(dicomFile);
            dicomFile.Save("result.dcm");
        }
        */

        public static void TestRedactAge()
        {
            var tag1 = DicomTag.PatientAge;
            var tag2 = DicomTag.SelectorASValue;

            var dataset = new DicomDataset
            {
                { tag1, "090Y" },
                { tag2, "010D", "010W", "100M", "010Y", "090Y"},
            };

            var itemList = dataset.ToArray();

            var redactProcess = new RedactProcessor(new ParameterConfiguration() { EnablePartialAgesForRedact = true});
            foreach (var item in itemList)
            {
                redactProcess.Process(dataset, item);
            }
        }

        public static void TestRedactDateTime()
        {
            var tag1 = DicomTag.Instance​Creation​Date;
            var tag2 = DicomTag.Instance​Coercion​Date​Time;
            var tag3 = DicomTag.Calibration​Date;
            var tag4 = DicomTag.Referenced​Date​Time;

            var dataset = new DicomDataset
            {
                { tag1, "20210320" },
                { tag2, "20210320202020.20+0800" },
                { tag3, "20210320", "19110101", "20200101" },
                { tag4, "20210320202020.20+0800", "20210320202020.00654", "20210320202020+1400" },
            };

            var itemList = dataset.ToArray();

            var redactProcess = new RedactProcessor(new ParameterConfiguration() { EnablePartialDatesForRedact = true });
            foreach (var item in itemList)
            {
                redactProcess.Process(dataset, item);
            }
        }


        public static void TestPerturb()
        {
            var tag1 = DicomTag.Reference​Pixel​X0; // SL VM =1
            var tag2 = DicomTag.Rational​Numerator​Value; // SL Vm=1-n
            var tag3 = DicomTag.Detector​Temperature; // DS VM = 1
            var tag4 = DicomTag.Filter​Thickness​Minimum; // DS VM = 1-n

            var dataset = new DicomDataset
            {
                { tag1, 1234 },
                { tag2, 1234, -1234, 4321, -4321 },
                { tag3, "1234.5" },
                { tag4, "1234.5", "-1234.5" },
            };

            var itemList = dataset.ToArray();

            var redactProcess = new PerturbProcessor();
            var settings = new Dictionary<string, object> { { "span", 1 }, { "roundTo", 2 }, { "rangeType", "proportional" } };
            foreach (var item in itemList)
            {
                redactProcess.Process(dataset, item, settings);
            }
        }

        public static ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public static void FilterByVR(DicomFile dicomFile, string vr)
        {
            var dataset = dicomFile.Dataset;
            string strFilePath = @"tciadicoms-SH.csv";
            foreach (var item in dataset)
            {
                if (item.ValueRepresentation.Equals(DicomVR.Parse(vr)))
                {
                    var tmpresult = dataset.GetValues<object>(item.Tag);
                    if(tmpresult.Count() == 0)
                    {
                        continue;
                    }

                    var result = dicomFile.File.ToString() +","+ item.Tag.ToString();
                    foreach(var i in tmpresult)
                    {
                        result = string.Format("{0},{1}\n", result, i.ToString());
                    }
                    File.AppendAllText(strFilePath, result);
                }
            }
        }
    }
}
