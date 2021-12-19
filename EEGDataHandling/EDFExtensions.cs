using EDFLib;
using System;
using System.Linq;

namespace EEGDataHandling
{
    public static class EDFExtensions
    {
        public static Header ToEDFHeader(this EEGRecordingMetadata metadata)
        {
            Header header = new Header();

            header.PatientID = metadata.PatientID;
            header.RecordingID = metadata.RecordingID;
            header.RecordingStartTime = metadata.StartTime;
            header.RecordDurationInSeconds = (short)metadata.RecordDurationInSeconds;

            return header;
        }

        public static SignalHeader ToEDFSignalHeader(this IEEGData signalData, int recordCount)
        {
            SignalHeader signal = signalData.SignalMetadata.ToEDFSignalHeader();

            signal.PhysicalMinimum = signalData.PhysicalMinimum;
            signal.PhysicalMaximum = signalData.PhysicalMaximum;
            signal.DigitalMinimum = (int)Math.Floor(signalData.DigitalMinimum);
            signal.DigitalMaximum = (int)Math.Ceiling(signalData.DigitalMaximum);

            // This should be accurate, provided that the sample rate is consistent
            // and covers the whole time period the caller is recording for.
            signal.SampleCountPerRecord = signalData.DataPoints.Count() / recordCount;

            return signal;
        }

        public static SignalHeader ToEDFSignalHeader(this EEGSignalMetadata signalMetadata)
        {
            SignalHeader signal = new SignalHeader
            {
                Label = signalMetadata.Label,
                PhysicalDimension = signalMetadata.Units,
                Prefiltering = signalMetadata.Prefiltering,
                TransducerType = signalMetadata.TransducerType
            };

            return signal;
        }
    }
}
