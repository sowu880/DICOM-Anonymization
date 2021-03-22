using System;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json;

namespace Microsoft.Health.Fhir.Anonymizer.Tool
{
    class Options
    {
        [Option('i', "inputFile", Required = true, HelpText = "Input dicom file")]
        public string InputFile { get; set; }

        [Option('o', "outputFile", Required = true, HelpText = "Output dicom file")]
        public string OutputFile { get; set; }

        [Option('c', "configFile", Required = false, Default = "configuration-sample.json", HelpText = "Anonymizer configuration file path.")]
        public string ConfigurationFilePath { get; set; }
    }

    public class Program
    {
        public async static Task Main(string[] args)
        {
            args = "-i image-00000.dcm -o test.dcm".Split();
            await CommandLine.Parser.Default.ParseArguments<Options>(args)
               .MapResult(async options => await AnonymizationLogic.AnonymizeAsync(options).ConfigureAwait(false), _ => Task.FromResult(1)).ConfigureAwait(false);
        }
    }
}