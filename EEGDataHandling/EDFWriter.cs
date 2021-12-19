using EDFLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EEGDataHandling
{
    public class EDFWriter
    {
        // Construct with the appropriate metadata.
        public EDFWriter(EEGRecordingMetadata recordingMetadata, IList<IEEGData> channelData)
        {
            _headerData = recordingMetadata;
            _channelData = channelData;
        }

        private EEGRecordingMetadata _headerData;
        private IList<IEEGData> _channelData;

        public void Write(string path)
        {
            using (Stream stream = File.OpenWrite(path))
            {
                Write(stream);
            }
        }

        public void Write(Stream stream)
        {
            // Create an EDF File

            // Set header properties from metadata.
            Header header = _headerData.ToEDFHeader();

            header.SignalCount = _channelData.Count;

            // Record count: since RecordDurationInSeconds is 1s, this is
            // equivalent to the number of seconds in the data we're writing.

            // Get longest duration?
            // (there are some things that are awkward / problematic about this).
            // For EDF export, we must have a very consistent sampling rate.
            long longestDuration = 0;
            foreach (IEEGData channel in _channelData)
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
            header.NumberOfRecords = (int)Math.Ceiling(longestDuration / (_headerData.RecordDurationInSeconds * 1000.0));

            header.SignalHeaders = _channelData.Select(x => x.ToEDFSignalHeader(header.NumberOfRecords)).ToList();
            //edfFile.Header = header;

            //edfFile.Signals = _channelData.Select(x => x.ToEDFSignalHeader(header.NumberOfRecords)).ToArray();

            // Write header
            header.Write(stream);
        }
    }
}