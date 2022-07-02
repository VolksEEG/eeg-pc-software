using System;
using System.Collections.Generic;

namespace EEGDataHandling
{
    public class EEGData : EventArgs
    {
        public double[] channelData;

        public EEGData(double[] chanData)
        {
            this.channelData = chanData;
        }
    }

    public interface IEEGData
    {
        event EventHandler DataUpdated;

        long LastUpdateTime { get; }

        double DigitalMinimum { get; }
        double DigitalMaximum { get; }

        double PhysicalMinimum { get; }
        double PhysicalMaximum { get; }

        EEGSignalMetadata SignalMetadata { get; }

        /// <summary>
        /// EEG data points contained within this channel.
        /// </summary>
        IEnumerable<(long timestamp, EEGData values)> DataPoints { get; }
    }
}
