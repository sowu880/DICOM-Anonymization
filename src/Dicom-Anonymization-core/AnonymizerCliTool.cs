using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Dicom;
using Newtonsoft.Json;

namespace Microsoft.Health.Dicom.Anonymization.Tool
{
    class Options
    {
        [Option('i', "inputFile", Required = false, HelpText = "Input dicom file")]
        public string InputFile { get; set; }

        [Option('o', "outputFile", Required = false, HelpText = "Output dicom file")]
        public string OutputFile { get; set; }

        [Option('c', "configFile", Required = false, Default = "configuration.json", HelpText = "Anonymization configuration file path.")]
        public string ConfigurationFilePath { get; set; }

        [Option('f', "inputFolder", Required = false, HelpText = "Input folder")]
        public string InputFolder { get; set; }

        [Option('t', "outputFolder", Required = false, HelpText = "Output folder")]
        public string OutputFolder { get; set; }
    }

    public class Program
    {
        public async static Task Main(string[] args)
        {
            //var result = new DateTime(2021, 02, 28, 0, 0, 0, 0).ToString("yyyy-MM-ddTHH:mm:ss.FFF");
             // args = "-i /SampleDicom/tciadicoms/acrin-flt-breast/1.3.6.1.4.1.14519.5.2.1.7009.2401.112735177269997047313598985735/1-384c4c41409ec3f802d2e09a2740ff30.dcm - o test.dcm".Split();
             await CommandLine.Parser.Default.ParseArguments<Options>(args)
               .MapResult(async options => await AnonymizationLogic.AnonymizeAsync(options).ConfigureAwait(false), _ => Task.FromResult(1)).ConfigureAwait(false);
        }
    }
}