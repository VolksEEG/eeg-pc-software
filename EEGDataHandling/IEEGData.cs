using System;
using System.Collections.Generic;
using System.Text;

namespace EEGDataHandling
{
    public interface IEEGData
    {
        IEnumerable<(long, double)> DataPoints { get; }
    }
}
