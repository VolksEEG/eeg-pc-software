using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EDFLib
{
    public class Signal
    {
        public Signal(SignalHeader signalHeader)
        {
            SignalHeader = signalHeader;
        }

        // References its Signal Header.
        public SignalHeader SignalHeader { get; set; }

        // Maybe include a start time to support annotations?

        // Contains signal data.
        public List<short> Values { get; set; } = new List<short>();
    }
}
