using De_Id_Function_Shared;
using Dicom;
using Dicom.Anonymization.Model;
using Dicom.Anonymization.Processors;
using Dicom.IO.Buffer;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class EncryptionProcessUnitTests
    {
        public EncryptionProcessUnitTests()
        {
            Processor = new EncryptionProcessor(new DicomEncryptionSetting() { EncryptKey = DefaultEncryptKey });
        }

        public EncryptionProcessor Processor { get; set; }

        public string DefaultEncryptKey = "1234567812345678";

        public static IEnumerable<object[]> GetUnsupportedVRItemForEncryption()
        {
            // Invalid output length limitation
            yield return new object[] { DicomTag.RetrieveAETitle, "TEST" }; // AE
            yield return new object[] { DicomTag.PatientAge, "100Y" }; // AS
            yield return new object[] { DicomTag.Query​Retrieve​Level, "0" }; // CS
            yield return new object[] { DicomTag.Event​Elapsed​Times, "1234.5" }; // DS
            yield return new object[] { DicomTag.Stage​Number, "1234" }; // IS
            yield return new object[] { DicomTag.Patient​Telephone​Numbers, "TEST" }; // SH
            yield return new object[] { DicomTag.SOP​Classes​In​Study, "12345" }; // UI

            // Invalid input
            yield return new object[] { DicomTag.Longitudinal​Temporal​Offset​From​Event, "12345" }; // FD
            yield return new object[] { DicomTag.Examined​Body​Thickness, "12345" }; // FL
            yield return new object[] { DicomTag.Doppler​Sample​Volume​X​Position, "12345" }; // SL
            yield return new object[] { DicomTag.Real​World​Value​First​Value​Mapped, "12345" }; // SS
            yield return new object[] { DicomTag.Referenced​Content​Item​Identifier, "12345" }; // UL
            yield return new object[] { DicomTag.Referenced​Waveform​Channels, "12345\\1234" }; // FD
        }

        public static IEnumerable<object[]> GetValidVRItemForEncryption()
        {
            yield return new object[] { DicomTag.Consulting​Physician​Name, "Test\\Test", @"hzmCq54ocn2tdSHD+byM6o8rX1GU49MF9uILJ++Jdjk=\UMYXR6OVofmgd+uQ0rKp5ZEXBchB7zu+7k+zmiLL5JE=" }; // PN
            yield return new object[] { DicomTag.Long​Code​Value, "TEST" , "jJ7zRxhIpEWWIH9qAIHDyg90+s0wl15xgVP+yt4Agb8=" }; // UC
            yield return new object[] { DicomTag.Event​Timer​Names, "TestTimer", "RQ6/Ni0NxK5KvTXiCht6NLzJMXsV6hai7JL9cpp+tXk=" }; // LO
            yield return new object[] { DicomTag.Strain​Additional​Information, "TestInformation", "L4vCn/M10JRqiceL63O+7wEuFQZqIcwAr/Kfa1hRHRI=" }; // UT
            yield return new object[] { DicomTag.Derivation​Description, "TestDescription", "x51RrmYbzHSr35/lg7EKTRBZVyajSPv/A2fQswGalu4=" }; // ST
            yield return new object[] { DicomTag.Pixel​Data​Provider​URL, "http://test", "Cy3cAem05Htuj2b+ng3sVcNI5WScyQuzNeNEHSkFtuw=" }; // LT
        }

        public static IEnumerable<object[]> GetValidItemForEncryptionWithOutputExceedLengthLimitation()
        {
            yield return new object[] { DicomTag.Consulting​Physician​Name, "jJ7zRxhIpEWWIH9qAIHDyg90+s0wl15xgVP+yt4Agb8=jJ7zRxhIpEWWIH9qAIHDyg90+s0wl15xgVP+yt4Agb8=" };
        }

        [Theory]
        [MemberData(nameof(GetUnsupportedVRItemForEncryption))]
        public void GivenADataSetWithUnsupportedVRForEncryption_WhenEncrypt_ExceptionWillBeThrown(DicomTag tag, string value)
        {
            var dataset = new DicomDataset
            {
                { tag, value },
            };

            Assert.Throws<Exception>(() => Processor.Process(dataset, dataset.GetDicomItem<DicomElement>(tag)));
        }

        [Theory]
        [MemberData(nameof(GetValidItemForEncryptionWithOutputExceedLengthLimitation))]
        public void GivenADataSetWithValidVRForEncryption_IfOutputExceedLengthLimitation_WhenEncrypt_ExceptionWillBeThrown(DicomTag tag, string value)
        {
            var dataset = new DicomDataset
            {
                { tag, value },
            };

            Assert.Throws<Exception>(() => Processor.Process(dataset, dataset.GetDicomItem<DicomElement>(tag)));
        }

        [Theory]
        [MemberData(nameof(GetValidVRItemForEncryption))]
        public void GivenADataSetWithValidVRForCryptoHash_WhenCryptoHash_ItemWillBeHashed(DicomTag tag, string value, string result)
        {
            var dataset = new DicomDataset
            {
                { tag, value },
            };

            Processor.Process(dataset, dataset.GetDicomItem<DicomElement>(tag), new Dictionary<string, object> { { "EncryptKey", "0000000000000000" } });
            var test = dataset.GetDicomItem<DicomElement>(tag).Get<string>();

            var decryptedValue = string.Join(@"\", dataset.GetDicomItem<DicomElement>(tag).Get<string[]>().Select(x => Decryption(x, "0000000000000000")));
            Assert.Equal(value, decryptedValue);
        }

        private string Decryption(string encryptedValue, string key)
        {
            return Encoding.UTF8.GetString(EncryptFunction.DecryptContentWithAES(Convert.FromBase64String(encryptedValue), Encoding.UTF8.GetBytes("0000000000000000")));
        }

        [Fact]
        public void GivenADataSetWithDicomElementOB_WhenEncrypt_ValueWillBeEncrypted()
        {
            var tag = DicomTag.PixelData;
            var item = new DicomOtherByte(tag, Encoding.UTF8.GetBytes("test"));
            var dataset = new DicomDataset(item);

            Processor.Process(dataset, item);
            Assert.Equal(Encoding.UTF8.GetBytes("test"), EncryptFunction.DecryptContentWithAES(dataset.GetDicomItem<DicomOtherByte>(tag).Get<byte[]>(), Encoding.UTF8.GetBytes(DefaultEncryptKey)));
        }

        [Fact]
        public void GivenADataSetWithDicomFragmentSequence_WhenEncrypt_FragmentsWillBeEncrypted()
        {
            var tag = DicomTag.PixelData;
            var item = new DicomOtherByteFragment(tag);
            item.Fragments.Add(new MemoryByteBuffer(Encoding.UTF8.GetBytes("fragment")));
            item.Fragments.Add(new MemoryByteBuffer(Encoding.UTF8.GetBytes("fragment")));

            var dataset = new DicomDataset(item);

            Processor.Process(dataset, item);

            var enumerator = ((DicomFragmentSequence)dataset.GetDicomItem<DicomItem>(tag)).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Assert.Equal(Encoding.UTF8.GetBytes("fragment"), EncryptFunction.DecryptContentWithAES(enumerator.Current.Data, Encoding.UTF8.GetBytes(DefaultEncryptKey)));
            }
        }

        [Fact]
        public void GivenADataSetWithSQItem_WhenEncrypt_ExceptionWillBeThrown()
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

            Assert.Throws<Exception>(() => Processor.Process(dataset, dataset.GetDicomItem<DicomItem>(DicomTag.ScheduledProcedureStepSequence)));
        }

    }
}