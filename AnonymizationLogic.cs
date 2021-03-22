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

                //FilterByVR(dicomFile, "PN");
                var engine = new AnonymizerEngine(options.ConfigurationFilePath);
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