using EDFLib;
using EEGDataHandling;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGMachineTests
{
    public class EDFWriterTest
    {
        // Write an EDF File.
        [Test]
        public async Task TestWriteEDFFile()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            // Using a specific date time offset for ease of verification.
            var startTime = DateTimeOffset.Parse("2021-11-19 06:09:00Z");

            // Initialize metadata
            EEGRecordingMetadata metadata = new EEGRecordingMetadata()
            {
                PatientID = "Patient ID",
                RecordingID = "Recording ID",
                RecordDurationInSeconds = 1,
                StartTime = startTime
            };

            // Initialize signal metadata for one signal
            EEGSignalMetadata signalMetadata = new EEGSignalMetadata()
            {
                Label = "Label",
                Units = "Units",
                TransducerType = "Transducer type",
                Prefiltering = "Prefiltering"
            };

            // Mock IEEGData
            var mock = new Mock<IEEGData>();

            // Return our signalMetadata
            mock.Setup(x => x.SignalMetadata).Returns(signalMetadata);

            // Mock other relevant properties
            mock.Setup(x => x.PhysicalMinimum).Returns(-20);
            mock.Setup(x => x.PhysicalMaximum).Returns(20);
            mock.Setup(x => x.DigitalMinimum).Returns(0);
            mock.Setup(x => x.DigitalMaximum).Returns(20);

            // Mock sample data - 2 samples/second (total of 20 samples.)
            long startTimeMs = startTime.ToUnixTimeMilliseconds();

            var dataPoints = new List<(long timestamp, double value)>();
            for (int i = 0; i < 20; i++)
            {
                dataPoints.Add((startTimeMs + i * 500, i));
            }

            mock.Setup(x => x.DataPoints).Returns(dataPoints);

            var edfFileName = "test.edf";

            using (EDFWriter writer = new EDFWriter(edfFileName, null))
            {
                writer.WriteEEG(metadata, new List<IEEGData>() { mock.Object });
            }

            // Read file as string.
            //string fileContent = File.ReadAllText(edfFileName);
            //Console.WriteLine(fileContent);

            // Read file as EDF
            EDFReader reader = new EDFReader(null);

            using (Stream stream = File.OpenRead(edfFileName))
            {
                var edfFile = await reader.Read(stream);
                var header = edfFile.Header;

                Assert.AreEqual(metadata.PatientID, header.PatientID, "PatientID mismatch");
                Assert.AreEqual(metadata.RecordingID, header.RecordingID, "RecordID mismatch");
                Assert.AreEqual(metadata.StartTime.ToString("dd.MM.yy"), header.RecordingStartDateString, "RecordStartDate mismatch");
                Assert.AreEqual(metadata.StartTime.ToString("hh.mm.ss"), header.RecordingStartTimeString, "RecordStartTime mismatch");
                Assert.AreEqual(10, header.NumberOfRecords, "RecordCount mismatch");
                Assert.AreEqual(1, header.SignalCount, "SignalCount mismatch");

                Assert.AreEqual(1, header.SignalHeaders.Count, "Signal headers Length mismatch");

                for (int i = 0; i < header.SignalHeaders.Count; i++)
                {
                    Assert.AreEqual(2, header.SignalHeaders[i].SampleCountPerRecord, $"Signal {i} SampleCountPerRecord mismatch");
                    Assert.AreEqual(signalMetadata.Units, header.SignalHeaders[i].PhysicalDimension, $"Signal {i} PhysicalDimension mismatch");
                    Assert.AreEqual(signalMetadata.Prefiltering, header.SignalHeaders[i].Prefiltering, $"Signal {i} Prefiltering mismatch");
                    Assert.AreEqual(signalMetadata.TransducerType, header.SignalHeaders[i].TransducerType, $"Signal {i} TransducerType mismatch");
                    Assert.AreEqual(mock.Object.PhysicalMinimum, header.SignalHeaders[i].PhysicalMinimum, $"Signal {i} PhysicalMinimum mismatch");
                    Assert.AreEqual(mock.Object.PhysicalMaximum, header.SignalHeaders[i].PhysicalMaximum, $"Signal {i} PhysicalMaximum mismatch");
                    Assert.AreEqual(mock.Object.DigitalMinimum, header.SignalHeaders[i].DigitalMinimum, $"Signal {i} DigitalMinimum mismatch");
                    Assert.AreEqual(mock.Object.DigitalMaximum, header.SignalHeaders[i].DigitalMaximum, $"Signal {i} DigitalMaximum mismatch");

                    CollectionAssert.AreEqual(mock.Object.DataPoints.Select(x => (short)x.value), edfFile.Signals[i].Values);
                }
            }

            // clean up the file.
            File.Delete(edfFileName);
        }
    }
}
