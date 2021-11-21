using EEGDataHandling;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EDFFile = SharpLib.EuropeanDataFormat.File;

namespace EEGMachineTests
{
    public class EDFWriterTest
    {
        // Write an EDF File.
        [Test]
        public void TestWriteEDFFile()
        {
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
                dataPoints.Add((startTimeMs + i*500, i));
            }

            mock.Setup(x => x.DataPoints).Returns(dataPoints);

            EDFWriter writer = new EDFWriter(metadata, new List<IEEGData>() { mock.Object });

            var edfFileName = "test.edf";

            writer.Write(edfFileName);

            // Read file as string.
            string fileContent = File.ReadAllText(edfFileName);
            Console.WriteLine(fileContent);

            // Read file as EDF
            var edf2 = new EDFFile(edfFileName);

            Assert.AreEqual("0", edf2.Header.Version.Value, "Version mismatch");
            Assert.AreEqual(metadata.PatientID, edf2.Header.PatientID.Value, "PatientID mismatch");
            Assert.AreEqual(metadata.RecordingID, edf2.Header.RecordID.Value, "RecordID mismatch");
            Assert.AreEqual(metadata.StartTime.ToString("dd.MM.yy"), edf2.Header.RecordingStartDate.Value, "RecordStartDate mismatch");
            Assert.AreEqual(metadata.StartTime.ToString("hh.mm.ss"), edf2.Header.RecordingStartTime.Value, "RecordStartTime mismatch");
            Assert.AreEqual(10, edf2.Header.RecordCount.Value, "RecordCount mismatch");
            Assert.AreEqual(1, edf2.Header.SignalCount.Value, "SignalCount mismatch");

            Assert.AreEqual("RESERVED", edf2.Header.Reserved.Value, $"Header Reserved mismatch");

            Assert.AreEqual(1, edf2.Signals.Length, "Signals Length mismatch");

            for (int i = 0; i < edf2.Signals.Length; i++)
            {
                Assert.AreEqual(2, edf2.Signals[i].SampleCountPerRecord.Value, $"Signal {i} SampleCountPerRecord mismatch");
                Assert.AreEqual(signalMetadata.Units,  edf2.Signals[i].PhysicalDimension.Value, $"Signal {i} PhysicalDimension mismatch");
                Assert.AreEqual(signalMetadata.Prefiltering, edf2.Signals[i].Prefiltering.Value, $"Signal {i} Prefiltering mismatch");
                Assert.AreEqual(signalMetadata.TransducerType, edf2.Signals[i].TransducerType.Value, $"Signal {i} TransducerType mismatch");
                Assert.AreEqual(mock.Object.PhysicalMinimum, edf2.Signals[i].PhysicalMinimum.Value, $"Signal {i} PhysicalMinimum mismatch");
                Assert.AreEqual(mock.Object.PhysicalMaximum, edf2.Signals[i].PhysicalMaximum.Value, $"Signal {i} PhysicalMaximum mismatch");

                Assert.AreEqual("RESERVED", edf2.Signals[i].Reserved.Value, $"Signal {i} Reserved mismatch");

                CollectionAssert.AreEqual(mock.Object.DataPoints.Select(x => x.value), edf2.Signals[i].Samples);
            }

            // clean up the file.
            File.Delete(edfFileName);
        }
    }
}
