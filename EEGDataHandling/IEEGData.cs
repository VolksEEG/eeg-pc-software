using System;
using System.Collections.Generic;

namespace EEGDataHandling
{
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
        IEnumerable<(long timestamp, double value)> DataPoints { get; }
    }
}
