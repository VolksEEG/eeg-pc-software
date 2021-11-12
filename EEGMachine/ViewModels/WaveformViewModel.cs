using EEGDataHandling;
using EEGMachine.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace EEGMachine.ViewModels
{
    public class WaveformViewModel : ObservableObject
    {
        public WaveformViewModel(IEEGData data, ITimeRange timeRange)
        {
            EEGData = data;
            TimeRange = timeRange;
            data.DataUpdated += OnDataUpdated;
        }

        public ITimeRange TimeRange { get; }

        // TODO: determine how min/max should be handled.
        // Do we auto-scale based on EEGData's min/max, or have fixed values,
        // or allow the user to set these separately?
        public int Min { get; private set; } = 0;
        public int Max { get; private set; } = 1;

        private void OnDataUpdated(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(EEGData));
        }

        public IEEGData EEGData { get; }
    }
}
