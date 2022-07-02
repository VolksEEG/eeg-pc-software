using EDFLib;
using EEGDataHandling;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EEGMachineTests
{
    public class EDFWriterTest
    {
        // Write an EDF File.
        [Test]
        [Description("Writes mock data to an EDF file, reads it back in, and checks the data read matches the data written.")]
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
                StartTime = startTime,
            };

            // Initialize signal metadata for the first signal
            EEGSignalMetadata firstSignalMetadata = new EEGSignalMetadata()
            {
                Label = "Label1",
                Units = "Units",
                TransducerType = "Transducer type",
                Prefiltering = "Prefiltering",
            };

            // Initialize signal metadata for the second signal
            EEGSignalMetadata secondSignalMetadata = new EEGSignalMetadata()
            {
                Label = "Label2",
                Units = "Units",
                TransducerType = "Transducer type",
                Prefiltering = "Prefiltering",
            };

            // Placing in a list for ease of comparison later.
            List<EEGSignalMetadata> signalHeaders = new List<EEGSignalMetadata>()
            {
                firstSignalMetadata, secondSignalMetadata,
            };

            // Mock first signal's IEEGData
            Mock<IEEGData> firstMockSignal = new Mock<IEEGData>();

            // Return our signalMetadata
            firstMockSignal.Setup(x => x.SignalMetadata).Returns(firstSignalMetadata);

            // Mock other relevant properties
            firstMockSignal.Setup(x => x.PhysicalMinimum).Returns(-20);
            firstMockSignal.Setup(x => x.PhysicalMaximum).Returns(20);
            firstMockSignal.Setup(x => x.DigitalMinimum).Returns(0);
            firstMockSignal.Setup(x => x.DigitalMaximum).Returns(20);

            // Mock sample data - 2 samples/second (total of 20 samples).
            long startTimeMs = startTime.ToUnixTimeMilliseconds();
            Random random = new Random();

            var dataPoints = new List<(long timestamp, EEGData value)>();
            for (int i = 0; i < 20; i++)
            {
                dataPoints.Add(((startTimeMs + (i * 500)), new EEGData(new double[] { (random.NextDouble() * 20)})));
            }

            // Output data points (to better diagnose test failures, since this test case uses
            // random inputs).
            Console.WriteLine($"First Signal Values - {string.Join(',', dataPoints.Select(x => $"{x.timestamp}ms:{x.value}"))}");

            firstMockSignal.Setup(x => x.DataPoints).Returns(dataPoints);

            // Mock second signal's IEEGData
            Mock<IEEGData> secondMockSignal = new Mock<IEEGData>();

            // Return our signalMetadata
            secondMockSignal.Setup(x => x.SignalMetadata).Returns(firstSignalMetadata);

            // Mock other relevant properties
            secondMockSignal.Setup(x => x.PhysicalMinimum).Returns(-1.7);
            secondMockSignal.Setup(x => x.PhysicalMaximum).Returns(2.9);
            secondMockSignal.Setup(x => x.DigitalMinimum).Returns(-10);
            secondMockSignal.Setup(x => x.DigitalMaximum).Returns(10);

            // Mock sample data - 10 samples/second (total of 100 samples).
            dataPoints = new List<(long timestamp, EEGData value)>();
            for (int i = 0; i < 100; i++)
            {
                dataPoints.Add(((startTimeMs + (i * 100)), new EEGData(new double[] { ((random.NextDouble() * 20) - 10)})));
            }

            Console.WriteLine($"Second Signal Values - {string.Join(',', dataPoints.Select(x => $"{x.timestamp}ms:{x.value}"))}");

            secondMockSignal.Setup(x => x.DataPoints).Returns(dataPoints);

            var edfFileName = "test.edf";

            List<IEEGData> signals = new List<IEEGData>() { firstMockSignal.Object, secondMockSignal.Object };

            using (EDFWriter writer = new EDFWriter(edfFileName, null))
            {
                writer.WriteEEG(metadata, signals);
            }

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
                Assert.AreEqual(2, header.SignalCount, "SignalCount mismatch");

                Assert.AreEqual(2, header.SignalHeaders.Count, "Signal headers Length mismatch");

                // First signal: 2 samples/record.
                Assert.AreEqual(2, header.SignalHeaders[0].SampleCountPerRecord, $"Signal 0 SampleCountPerRecord mismatch");

                // Second signal: 10 samples/record.
                Assert.AreEqual(10, header.SignalHeaders[1].SampleCountPerRecord, $"Signal 1 SampleCountPerRecord mismatch");

                for (int i = 0; i < header.SignalHeaders.Count; i++)
                {
                    Assert.AreEqual(signalHeaders[i].Units, header.SignalHeaders[i].PhysicalDimension, $"Signal {i} PhysicalDimension mismatch");
                    Assert.AreEqual(signalHeaders[i].Prefiltering, header.SignalHeaders[i].Prefiltering, $"Signal {i} Prefiltering mismatch");
                    Assert.AreEqual(signalHeaders[i].TransducerType, header.SignalHeaders[i].TransducerType, $"Signal {i} TransducerType mismatch");

                    Assert.AreEqual(signals[i].PhysicalMinimum, header.SignalHeaders[i].PhysicalMinimum, $"Signal {i} PhysicalMinimum mismatch");
                    Assert.AreEqual(signals[i].PhysicalMaximum, header.SignalHeaders[i].PhysicalMaximum, $"Signal {i} PhysicalMaximum mismatch");
                    Assert.AreEqual(signals[i].DigitalMinimum, header.SignalHeaders[i].DigitalMinimum, $"Signal {i} DigitalMinimum mismatch");
                    Assert.AreEqual(signals[i].DigitalMaximum, header.SignalHeaders[i].DigitalMaximum, $"Signal {i} DigitalMaximum mismatch");

                    CollectionAssert.AreEqual(signals[i].DataPoints.Select(x => (short)x.values.channelData[0]), edfFile.Signals[i].Values);
                }
            }

            // clean up the file.
            File.Delete(edfFileName);
        }
    }
}
