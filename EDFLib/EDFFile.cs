using System.Collections.Generic;

namespace EDFLib
{
    public class EDFFile
    {
        public Header Header { get; set; }

        public List<Signal> Signals { get; set; }
    }
}
