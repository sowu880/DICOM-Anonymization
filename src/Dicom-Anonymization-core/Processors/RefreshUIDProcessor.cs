using System;
using System.Collections.Generic;
using System.Text;

namespace Dicom.Anonymization.Processors
{
    public class RefreshUIDProcessor : IAnonymizationProcessor
    {
        public Dictionary<string, string> ReplacedUIDs { get; } = new Dictionary<string, string>();

        public void Process(DicomDataset dicomDataset, DicomItem item, Dictionary<string, object> settings = null)
        {
            if (!(item is DicomElement) || item.ValueRepresentation != DicomVR.UI)
            {
                throw new Exception($"Invalid refresh UID operation for item {item}");
            }

            string rep;
            DicomUID uid;
            var old = ((DicomElement)item).Get<string>();

            if (ReplacedUIDs.ContainsKey(old))
            {
                rep = ReplacedUIDs[old];
                uid = new DicomUID(rep, "Anonymized UID", DicomUidType.Unknown);
            }
            else
            {
                uid = DicomUIDGenerator.GenerateDerivedFromUUID();
                rep = uid.UID;
                ReplacedUIDs[old] = rep;
            }

            var newItem = new DicomUniqueIdentifier(item.Tag, uid);
            dicomDataset.AddOrUpdate(newItem);
        }
    }
}
