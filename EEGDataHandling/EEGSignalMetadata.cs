using System;
using System.Collections.Generic;
using System.Text;

namespace EEGDataHandling
{
    /// <summary>
    /// Container class for metadata concerning an EEG signal,
    /// such as min/max values, units, etc.
    /// Currently just contains whatever is required to support EDF files;
    /// it may be useful to break this class up and/or add more fields, depending
    /// on where else in the application these properties are needed, when they're
    /// entered, etc.
    /// </summary>
    public class EEGSignalMetadata
    {
        public EEGSignalMetadata()
        {

        }

        public string Label { get; set; }
        public string Units { get; set; }
        public string TransducerType { get; set; }
        public string Prefiltering { get; set; }
    }
}