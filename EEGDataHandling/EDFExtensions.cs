using System;
using System.Collections.Generic;
using System.Linq;
using EDFLib;

namespace EEGDataHandling
{
    public static class EDFExtensions
    {
        public static void WriteEEG(this EDFWriter writer, EEGRecordingMetadata metadata, List<IEEGData> channelData)
        {
            // Create an EDF File

            // Set header properties from metadata.
            Header header = metadata.ToEDFHeader();

            header.SignalCount = channelData.Count;

            // Record count: since RecordDurationInSeconds is 1s, this is
            // equivalent to the number of seconds in the data we're writing.

            // Get longest duration?
            // (there are some things that are awkward / problematic about this).
            // For EDF export, we must have a very consistent sampling rate.
            long longestDuration = 0;
            foreach (IEEGData channel in channelData)
            {
                long min = channel.DataPoints.Select(x => x.timestamp).Min();
                long max = channel.DataPoints.Select(x => x.timestamp).Max();
                long duration = max - min;

                if (longestDuration < duration)
                {
                    longestDuration = duration;
                }
            }

            // Number of seconds in the data. (round up).
            header.NumberOfRecords = (int)Math.Ceiling(longestDuration / (metadata.RecordDurationInSeconds * 1000.0));

            header.SignalHeaders = channelData.Select(x => x.ToEDFSignalHeader(header.NumberOfRecords)).ToList();

            List<Signal> signals = channelData.Select(x => new Signal(x.ToEDFSignalHeader(header.NumberOfRecords))
            {
                Values = x.DataPoints.Select(d => (short)d.values.channelData[0]).ToList()
            }).ToList();

            EDFFile file = new EDFFile
            {
                Header = header,
                Signals = signals
            };

            writer.Write(file);
        }

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
