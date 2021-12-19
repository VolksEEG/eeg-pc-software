using EEGDataHandling;
using EEGMachine.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace EEGMachine.ViewModels
{
    public class WaveformsViewModel : ObservableObject, ITimeRange
    {
        // For now, simply instantiates some Waveforms.
        public WaveformsViewModel()
        {
            List<WaveformViewModel> waveforms = new List<WaveformViewModel>();
            _generators = new List<EEGDataGenerator>();

            for (int i = 0; i < 8; i++)
            {
                EEGDataGenerator generator = new EEGDataGenerator();

                _generators.Add(generator);

                WaveformViewModel vm = new WaveformViewModel(generator, this);
                vm.PropertyChanged += WaveformPropertyChanged;
                waveforms.Add(vm);
            }

            Waveforms = waveforms;

            // For now, just using current time in UTC, but we may want to set this
            // initially based on the oldest time we want to display from the source data.
            StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        private void WaveformPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(WaveformViewModel.LastUpdateTime))
            {
                // Check if we need to roll our time interval.
                // Retrieve the last (most recent) sample from each waveform:
                long currentTimestamp = CurrentTime;

                foreach (WaveformViewModel vm in Waveforms)
                {
                    long newestTimestamp = vm.LastUpdateTime;

                    if (newestTimestamp > currentTimestamp)
                    {
                        currentTimestamp = newestTimestamp;
                    }
                }

                // Update current time indicator (line that sweeps across display).
                CurrentTime = currentTimestamp;

                // Check if we need to roll our start and end times.
                if (CurrentTime > EndTime)
                {
                    // Update time interval we're currently displaying.
                    StartTime = EndTime;
                }
            }
        }

        public IEnumerable<WaveformViewModel> Waveforms { get; }

        private List<EEGDataGenerator> _generators;

        // Range of values in milliseconds to display at a time.
        // (20 seconds)
        // TODO: determine how configurable this needs to be -
        // support live updates?
        public long Duration { get; private set; } = 20000;

        private long _startTime;
        public long StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                OnPropertyChanged();
            }
        }

        public long EndTime => StartTime + Duration;

        private long _currentTime; // Could be pulled from most recent sample among all waveforms we contain?
        public long CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                OnPropertyChanged();
            }
        }

        // Commands
        private RelayCommand _startCaptureCommand;
        public ICommand StartCaptureCommand => _startCaptureCommand ??= new RelayCommand(StartCapture);

        private RelayCommand _stopCaptureCommand;
        public ICommand StopCaptureCommand => _stopCaptureCommand ??= new RelayCommand(StopCapture);

        private void StartCapture()
        {
            // Call start on each waveform generator.
            foreach (EEGDataGenerator generator in _generators)
            {
                generator.StartGenerating();
            }
        }

        private void StopCapture()
        {
            // Call stop on each waveform generator.
            foreach (EEGDataGenerator generator in _generators)
            {
                generator.StopGenerating();
            }
        }
    }
}
