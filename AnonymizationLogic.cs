using Dicom;
using Dicom.Anonymization;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Anonymizer.Tool
{
    internal static class AnonymizationLogic
    {
        internal static async Task AnonymizeAsync(Options options)
        {
            try
            {
                DicomFile dicomFile = await DicomFile.OpenAsync(options.InputFile).ConfigureAwait(false);

                var engine = new AnonymizerEngine(options.ConfigurationFilePath);
                Console.WriteLine("{0,-15}{1,-40}{2,-15}{3,-30}{4,-70}", "Tag", "Name", "De-ID Method", "Original Value", "Result");
                Console.WriteLine(new string('-', 150));
                engine.Anonymize(dicomFile.Dataset);
                dicomFile.Save(options.OutputFile);

                Console.WriteLine($"Finished processing '{options.InputFile}'!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Process failed! {ex}");
            }
        }

    }
}