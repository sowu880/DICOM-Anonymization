// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Dicom;
using Dicom.Anonymization.AnonymizationConfigurations.Exceptions;
using Dicom.Anonymization.Processors;
using Dicom.Anonymization.Processors.Settings;
using Dicom.IO.Buffer;
using Xunit;

namespace UnitTests
{
    public class SubstituteProcessUnitTests
    {
        public SubstituteProcessUnitTests()
        {
            Processor = new SubstituteProcessor(new DicomSubstituteSetting() { ReplaceWith = "Anonymous" });
        }

        public SubstituteProcessor Processor { get; set; }

        public static IEnumerable<object[]> GetValidItemAndSettingForSubstitute()
        {
            yield return new object[] { DicomTag.RetrieveAETitle, "TEST", new DicomSubstituteSetting { ReplaceWith = "Anonymous" }, "Anonymous" }; // AE
            yield return new object[] { DicomTag.Query​Retrieve​Level, "0", new DicomSubstituteSetting { ReplaceWith = "1" }, "1" }; // CS
            yield return new object[] { DicomTag.Patient​Telephone​Numbers, "TEST", null, "Anonymous" }; // SH
            yield return new object[] { DicomTag.SOP​Classes​In​Study, "12345", new DicomSubstituteSetting { ReplaceWith = "10000" }, "10000" }; // UI
            yield return new object[] { DicomTag.Frame​Acquisition​Date​Time, "20200101", new DicomSubstituteSetting { ReplaceWith = "20000101" }, "20000101" }; // DT
            yield return new object[] { DicomTag.Expiry​Date, "20200101", new DicomSubstituteSetting { ReplaceWith = "20000101" }, "20000101" }; // DA
            yield return new object[] { DicomTag.Secondary​Review​Time, "120101.000", new DicomSubstituteSetting { ReplaceWith = "000000.000" }, "000000.000" }; // TM
        }

        [Theory]
        [MemberData(nameof(GetValidItemAndSettingForSubstitute))]
        public void GivenADataSetWithValidVRForSubstitute_WhenSubstitute_ValueWillBeReplaced(DicomTag tag, string value, DicomSubstituteSetting settings, string replaceWith)
        {
            var dataset = new DicomDataset
            {
                { tag, value },
            };

            Processor.Process(dataset, dataset.GetDicomItem<DicomElement>(tag), null, settings);
            Assert.Equal(replaceWith, dataset.GetDicomItem<DicomElement>(tag).Get<string>());
        }

        [Fact]
        public void GivenADataSetWithOWForSubstitute_WhenSubstitute_ValueWillBeSubstituted()
        {
            var tag = DicomTag.Red​Palette​Color​Lookup​Table​Data;
            var dataset = new DicomDataset
            {
                { tag, (ushort)10 },
            };

            Processor.Process(dataset, dataset.GetDicomItem<DicomElement>(tag), null, new DicomSubstituteSetting { ReplaceWith = "20" });
            Assert.True(dataset.GetDicomItem<DicomElement>(tag).Get<ushort>() == 20);
        }

        [Fact]
        public void GivenADataSetWithOWForSubstitute_WhenSubstituteWithInvalidValue_ExceptionWillBeThrown()
        {
            var tag = DicomTag.Red​Palette​Color​Lookup​Table​Data;
            var dataset = new DicomDataset
            {
                { tag, (ushort)10 },
            };

            Assert.Throws<AnonymizationConfigurationException>(() => Processor.Process(dataset, dataset.GetDicomItem<DicomElement>(tag)));
        }

        [Fact]
        public void GivenADataSetWithOLForSubstitute_WhenSubstitute_ValueWillBeSubstituted()
        {
            var tag = DicomTag.Selector​OLValue;
            var dataset = new DicomDataset
            {
                { tag, 10U },
            };

            Processor.Process(dataset, dataset.GetDicomItem<DicomElement>(tag), null, new DicomSubstituteSetting { ReplaceWith = "20" });
            Assert.True(dataset.GetDicomItem<DicomElement>(tag).Get<uint>() == 20);
        }

        [Fact]
        public void GivenADataSetWithODForSubstitute_WhenSubstitute_ValueWillBeSubstituted()
        {
            var tag = DicomTag.Volumetric​Curve​Up​Directions;
            var dataset = new DicomDataset
            {
                { tag, 10D },
            };

            Processor.Process(dataset, dataset.GetDicomItem<DicomElement>(tag), null, new DicomSubstituteSetting { ReplaceWith = "20" });
            Assert.True(dataset.GetDicomItem<DicomElement>(tag).Get<double>() == 20);
        }

        [Fact]
        public void GivenADataSetWithOFForSubstitute_WhenSubstitute_ValueWillBeSubstituted()
        {
            var tag = DicomTag.Float​​Pixel​​Data;
            var dataset = new DicomDataset
            {
                { tag, 10F },
            };

            Processor.Process(dataset, dataset.GetDicomItem<DicomElement>(tag), null, new DicomSubstituteSetting { ReplaceWith = "20" });
            Assert.True(dataset.GetDicomItem<DicomElement>(tag).Get<float>() == 20);
        }

        [Fact]
        public void GivenADataSetWithDicomFragmentSequence_WhenPerturb_ExceptionWillBeThrown()
        {
            var tag = DicomTag.PixelData;
            var item = new DicomOtherWordFragment(tag);
            item.Fragments.Add(new MemoryByteBuffer(Convert.FromBase64String("fragment")));
            item.Fragments.Add(new MemoryByteBuffer(Convert.FromBase64String("fragment")));

            var dataset = new DicomDataset(item);

            Assert.Throws<AnonymizationOperationException>(() => Processor.Process(dataset, item));
        }

        [Fact]
        public void GivenADataSetWithSQItem_WhenPerturb_ExceptionWillBeThrown()
        {
            var dataset = new DicomDataset { };
            var sps1 = new DicomDataset { { DicomTag.ScheduledStationName, "1" } };
            var sps2 = new DicomDataset { { DicomTag.ScheduledStationName, "2" } };
            var spcs1 = new DicomDataset { { DicomTag.ContextIdentifier, "1" } };
            var spcs2 = new DicomDataset { { DicomTag.ContextIdentifier, "2" } };
            var spcs3 = new DicomDataset { { DicomTag.ContextIdentifier, "3" } };
            sps1.Add(new DicomSequence(DicomTag.ScheduledProtocolCodeSequence, spcs1, spcs2));
            sps2.Add(new DicomSequence(DicomTag.ScheduledProtocolCodeSequence, spcs3));
            dataset.Add(new DicomSequence(DicomTag.ScheduledProcedureStepSequence, sps1, sps2));

            Assert.Throws<AnonymizationOperationException>(() => Processor.Process(dataset, dataset.GetDicomItem<DicomItem>(DicomTag.ScheduledProcedureStepSequence)));
        }
    }
}