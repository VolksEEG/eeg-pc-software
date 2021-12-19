using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EDFLib
{
    public class EDFFile
    {
        public Header Header { get; set; }

        public List<Signal> Signals { get; set; }
    }
}
