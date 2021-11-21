using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpEDF = SharpLib.EuropeanDataFormat;

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
            // Create an EDF File
            SharpEDF.File edfFile = new SharpEDF.File();

            // Set header properties from metadata.
            SharpEDF.Header header = _headerData.ToEDFHeader();

            header.SignalCount.Value = _channelData.Count;
            header.Signals.Reserveds.Value = Enumerable.Repeat("RESERVED", header.SignalCount.Value).ToArray();

            // Record count: since RecordDurationInSeconds is 1s, this is
            // equivalent to the number of seconds in the data we're writing.

            // Get longest duration?
            // (there are some things that are awkward / problematic about this).
            // For EDF export, we must have a very consistent sampling rate.
            long longestDuration = 0;
            foreach(IEEGData channel in _channelData)
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
            header.RecordCount.Value = (int)Math.Ceiling(longestDuration / (_headerData.RecordDurationInSeconds * 1000.0));

            edfFile.Header = header;

            edfFile.Signals = _channelData.Select(x => x.ToEDFSignal(_headerData.RecordDurationInSeconds)).ToArray();

            edfFile.Save(path);
        }
    }
}