using System;
using System.Collections.Generic;

namespace EEGDataHandling
{
    public interface IEEGData
    {
        event EventHandler DataUpdated;

        long LastUpdateTime { get; }

        IEnumerable<(long timestamp, double value)> DataPoints { get; }
    }
}
