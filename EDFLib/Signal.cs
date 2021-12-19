using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EDFLib
{
    public class Signal
    {
        // Contains signal data only.
        // Maybe include a start time to support annotations?

        public List<short> Values { get; set; }
    }
}
