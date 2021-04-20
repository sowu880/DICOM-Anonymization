using Dicom;
using Dicom.Anonymization.Model;
using Dicom.Anonymization.Processors;
using System.Linq;
using Xunit;

namespace UnitTests
{
    public class RedactProcessUnitTests
    {

        [Fact]
        public void GivenADataSetWithDTItem_WhenRedactWithPartialRedact_ValueWillBePartialRedact()
        {
            var tag1 = DicomTag.Instance​Coercion​Date​Time;
            var tag2 = DicomTag.Referenced​Date​Time;

            var dataset = new DicomDataset
            {
                { tag1, "20210320202020.20+0800" },
                { tag2, "20210320202020.20+0800", "20210320202020.00654", "20210320202020+1400", "19000320202020+1400" },
            };

            var itemList = dataset.ToArray();

            var redactProcess = new RedactProcessor(new DicomRedactSetting() { EnablePartialDatesForRedact = true });
            foreach (var item in itemList)
            {
                redactProcess.Process(dataset, item);
            }

            Assert.Equal("20210101000000.000000+0800", dataset.GetDicomItem<DicomElement>(tag1).Get<string>());
            Assert.Equal(@"20210101000000.000000+0800\20210101120000.000000\20210101000000.000000+1400", dataset.GetDicomItem<DicomElement>(tag2).Get<string>());
        }


        [Fact]
        public void GivenADataSetWithDAItem_WhenRedactWithPartialRedact_ValueWillBePartialRedact()
        {
            var tag1 = DicomTag.Instance​Creation​Date;
            var tag2 = DicomTag.Calibration​Date;

            var dataset = new DicomDataset
            {
                { tag1, "20210320" },
                { tag2, "20210320", "19110101", "20200101" },
            };

            var itemList = dataset.ToArray();

            var redactProcess = new RedactProcessor(new DicomRedactSetting() { EnablePartialDatesForRedact = true });
            foreach (var item in itemList)
            {
                redactProcess.Process(dataset, item);
            }

            Assert.Equal("20210101", dataset.GetDicomItem<DicomElement>(tag1).Get<string>());
            Assert.Equal(@"20210101\20200101", dataset.GetDicomItem<DicomElement>(tag2).Get<string>());
        }

        [Fact]
        public void GivenADataSetWithASItem_WhenRedactWithPartialRedact_ValueWillBePartialRedact()
        {
            var tag1 = DicomTag.PatientAge;
            var tag2 = DicomTag.SelectorASValue;

            var dataset = new DicomDataset
            {
                { tag1, "090Y" },
                { tag2, "010D", "010W", "100M", "010Y", "090Y" },
            };

            var itemList = dataset.ToArray();

            var redactProcess = new RedactProcessor(new DicomRedactSetting() { EnablePartialAgeForRedact = true });
            foreach (var item in itemList)
            {
                redactProcess.Process(dataset, item);
            }

            Assert.Equal(string.Empty, dataset.GetDicomItem<DicomElement>(tag1).Get<string>());
            Assert.Equal(@"010D\010W\100M\010Y", dataset.GetDicomItem<DicomElement>(tag2).Get<string>());
        }


        [Fact]
        public void GivenADataSetWithItemsForRedact_WhenDisablePartialRedact_ValueWillBeRedact()
        {
            var tag1 = DicomTag.Instance​Coercion​Date​Time;
            var tag2 = DicomTag.Referenced​Date​Time;
            var tag3 = DicomTag.Instance​Creation​Date;
            var tag4 = DicomTag.Calibration​Date;
            var tag5 = DicomTag.PatientAge;
            var tag6 = DicomTag.SelectorASValue;

            var dataset = new DicomDataset
            {
                { tag1, "20210320202020.20+0800" },
                { tag2, "20210320202020.20+0800", "20210320202020.00654", "20210320202020+1400", "19000320202020+1400" },
                { tag3, "20210320" },
                { tag4, "20210320", "19110101", "20200101" },
                { tag5, "090Y" },
                { tag6, "010D", "010W", "100M", "010Y", "090Y" },
            };

            var itemList = dataset.ToArray();

            var redactProcess = new RedactProcessor(new DicomRedactSetting() { EnablePartialDatesForRedact = false });
            foreach (var item in itemList)
            {
                redactProcess.Process(dataset, item);
                Assert.Equal(string.Empty, dataset.GetDicomItem<DicomElement>(item.Tag).Get<string>());
            }
        }

        [Fact]
        public void GivenADataSetWithSQItem_WhenRedact_ElementsInSQWillBeRedact()
        {
            var dataset = new DicomDataset {};
            var sps1 = new DicomDataset { { DicomTag.ScheduledStationName, "1" } };
            var sps2 = new DicomDataset { { DicomTag.ScheduledStationName, "2" } };
            var spcs1 = new DicomDataset { { DicomTag.ContextIdentifier, "1" } };
            var spcs2 = new DicomDataset { { DicomTag.ContextIdentifier, "2" } };
            var spcs3 = new DicomDataset { { DicomTag.ContextIdentifier, "3" } };
            sps1.Add(new DicomSequence(DicomTag.ScheduledProtocolCodeSequence, spcs1, spcs2));
            sps2.Add(new DicomSequence(DicomTag.ScheduledProtocolCodeSequence, spcs3));
            dataset.Add(new DicomSequence(DicomTag.ScheduledProcedureStepSequence, sps1, sps2));

            var itemList = dataset.ToArray();

            var redactProcess = new RedactProcessor(new DicomRedactSetting() { });
            foreach (var item in itemList)
            {
                redactProcess.Process(dataset, item);
            }

            Assert.Empty(dataset.GetDicomItem<DicomSequence>(DicomTag.ScheduledProcedureStepSequence).ToArray());
        }
    }
}