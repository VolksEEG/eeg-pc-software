using SharpLib.EuropeanDataFormat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EEGDataHandling
{
    public static class EDFExtensions
    {
        public static Header ToEDFHeader(this EEGRecordingMetadata metadata)
        {
            Header header = new Header();

            header.Version.Value = "0";
            header.PatientID.Value = metadata.PatientID;
            header.RecordID.Value = metadata.RecordingID;

            // EDF+ specification: use 1985 as a cutoff date.
            // https://www.edfplus.info/specs/edfplus.html#edfplusandedf
            if (metadata.StartTime.Year <= 2084)
            {
                header.RecordingStartDate.Value = metadata.StartTime.ToString("dd.MM.yy");
            }
            else
            {
                // EDF+: After 2084, replace year with 'yy'
                header.RecordingStartDate.Value = metadata.StartTime.ToString("dd.MM") + "yy";
            }

            header.RecordingStartTime.Value = metadata.StartTime.ToString("hh.mm.ss");

            header.Reserved.Value = "RESERVED";

            return header;
        }

        public static Signal ToEDFSignal(this IEEGData signalData, int recordDurationInSeconds)
        {
            Signal signal = signalData.SignalMetadata.ToEDFSignal();

            signal.PhysicalMinimum.Value = signalData.PhysicalMinimum;
            signal.PhysicalMaximum.Value = signalData.PhysicalMaximum;
            signal.DigitalMinimum.Value = (int)Math.Floor(signalData.DigitalMinimum);
            signal.DigitalMaximum.Value = (int)Math.Ceiling(signalData.DigitalMaximum);

            // Sample count: determine how many samples per second we want to write.
            long min = signalData.DataPoints.Select(x => x.timestamp).Min();
            long max = signalData.DataPoints.Select(x => x.timestamp).Max();
            long duration = max - min;

            // Note: duration is in ms.
            signal.SampleCountPerRecord.Value = (int)(duration / recordDurationInSeconds / 1000);
            signal.Samples = signalData.DataPoints.Select(x => (short)x.value).ToList();

            signal.Reserved.Value = "RESERVED";

            return signal;
        }

        public static Signal ToEDFSignal(this EEGSignalMetadata signalMetadata)
        {
            Signal signal = new Signal();

            signal.Label.Value = signalMetadata.Label;
            signal.PhysicalDimension.Value = signalMetadata.Units;
            signal.Prefiltering.Value = signalMetadata.Prefiltering;
            signal.TransducerType.Value = signalMetadata.TransducerType;

            return signal;
        }
    }
}
