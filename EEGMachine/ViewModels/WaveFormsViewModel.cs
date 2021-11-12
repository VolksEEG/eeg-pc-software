using EEGDataHandling;
using System;
using System.Collections.Generic;
using System.Text;

namespace EEGMachine.ViewModels
{
    public class WaveformsViewModel
    {
        // For now, simply instantiates five Waveforms.
        public WaveformsViewModel()
        {
            List<WaveformViewModel> waveforms = new List<WaveformViewModel>();

            for (int i = 0; i < 5; i++)
            {
                waveforms.Add(new WaveformViewModel(new EEGDataGenerator()));
            }

            Waveforms = waveforms;
        }

        public IEnumerable<WaveformViewModel> Waveforms { get; }
    }
}
