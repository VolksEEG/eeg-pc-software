using System;
using System.Collections.Generic;
using System.Text;

namespace EDFLib
{
    public class SignalHeader
    {
        public string Label { get; set; }
        public string PhysicalDimension { get; set; }
        public string Prefiltering { get; set; }
        public string TransducerType { get; set; }
        public double PhysicalMinimum { get; set; }
        public double PhysicalMaximum { get; set; }
        public double DigitalMinimum { get; set; }
        public double DigitalMaximum { get; set; }
        public int SampleCountPerRecord { get; set; }
    }
}
