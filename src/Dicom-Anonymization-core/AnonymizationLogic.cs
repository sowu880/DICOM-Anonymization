using Dicom;
using Dicom.Anonymization;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Health.Dicom.Anonymization.Tool
{
    internal static class AnonymizationLogic
    {
        internal static async Task AnonymizeAsync(Options options)
        {
            try
            {
                var engine = new AnonymizerEngine(options.ConfigurationFilePath);
                if (options.InputFile != null)
                {
                    await AnonymizeOneFile(options.InputFile, options.OutputFile, engine);
                }
                else
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    var num = 0;
                    foreach (string file in Directory.EnumerateFiles(options.InputFolder, "*.dcm", SearchOption.AllDirectories))
                    {
                        Console.WriteLine(file);
                        await AnonymizeOneFile(file, Path.Join(options.OutputFolder, Path.GetFileName(file)), engine);
                        num++;
                    }

                    sw.Stop();
                    TimeSpan ts = sw.Elapsed;
                    Console.WriteLine("{1} items.DateTime costed for Shuffle function is: {0}ms", ts.TotalMilliseconds, num);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Process failed! {ex}");
            }
        }

        internal static async Task AnonymizeOneFile(string inputFile, string outputFile, AnonymizerEngine engine )
        {
            DicomFile dicomFile = await DicomFile.OpenAsync(inputFile).ConfigureAwait(false);
            // Console.WriteLine("{0,-15}{1,-40}{2,-15}{3,-50}{4,-70}", "Tag", "Name", "De-ID Method", "Original Value", "Result");
            // Console.WriteLine(new string('-', 150));
            engine.Anonymize(dicomFile.Dataset);

            // dicomFile.Save(outputFile);

            // Console.WriteLine($"Finished processing '{inputFile}'!");
        }

    }
}