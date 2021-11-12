using System;
using System.Collections.Generic;
using System.Text;

namespace EEGMachine.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            WaveformsViewModel = new WaveformsViewModel();
        }

        public WaveformsViewModel WaveformsViewModel { get; }
    }
}
