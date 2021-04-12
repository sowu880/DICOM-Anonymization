using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Dicom;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.Anonymizer.Tool
{
    class Options
    {
        [Option('i', "inputFile", Required = true, HelpText = "Input dicom file")]
        public string InputFile { get; set; }

        [Option('o', "outputFile", Required = true, HelpText = "Output dicom file")]
        public string OutputFile { get; set; }

        [Option('c', "configFile", Required = false, Default = "configuration.json", HelpText = "Anonymizer configuration file path.")]
        public string ConfigurationFilePath { get; set; }
    }

    public class Program
    {
        public async static Task Main(string[] args)
        {

             Test();
            //var result = new DateTime(2021, 02, 28, 0, 0, 0, 0).ToString("yyyy-MM-ddTHH:mm:ss.FFF");
            args = "-i samples/c1937034-f8a4-4a84-a69c-213911b39907.dcm -o test.dcm".Split();
            await CommandLine.Parser.Default.ParseArguments<Options>(args)
               .MapResult(async options => await AnonymizationLogic.AnonymizeAsync(options).ConfigureAwait(false), _ => Task.FromResult(1)).ConfigureAwait(false);
        }

        public static void Test()
        {
            /*
            var tag1 = DicomTag.Modality;

            var dataset = new DicomDataset
            {
                
            };
            dataset.Add( tag1, "01234567890123456789" );
            var itemList = dataset.ToArray();
            
            var maskedTag = DicomMaskedTag.Parse("(xxxx,xxxx)");
            var isMatch = maskedTag.IsMatch(new DicomTag(0x1110, 0x1111));
            */
            var dicomTags = new DicomTag(0,0);
            var resutl = dicomTags.GetType().GetField("PersonName").GetValue(dicomTags);
        }
    }
}