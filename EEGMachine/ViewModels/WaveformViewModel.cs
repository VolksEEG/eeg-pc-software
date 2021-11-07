using EEGDataHandling;
using System;
using System.Collections.Generic;
using System.Text;

namespace EEGMachine.ViewModels
{
    public class WaveformViewModel
    {
        public WaveformViewModel(IEEGData data)
        {
            Data = data;
        }

        // Obviously this doesn't work for live updates
        public IEEGData Data { get; }
    }
}
