using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;

namespace EEGDataHandling
{
    /// <summary>
    /// A class for holding mocked EEG data for testing.
    public class EEGDataGenerator : IEEGData
    {
        // Set capacity to 60 seconds at 500 samples/second
        private const int QUEUE_CAPACITY = 500 * 60;

        /// <summary>
        /// Generator for time-series EEG data
        /// For simplicity (for now), timestamps are in milliseconds from
        /// 'start' and values are in the range (0, 1).
        /// </summary>
        public EEGDataGenerator()
        {
            _dataPoints = new ConcurrentQueue<(long, double)>();
            _random = new Random();
            _timer = new Timer();

            // Note: 500 samples/second means one sample every 2 ms.
            _timer.Interval = 2;
            _timer.Elapsed += (sender, e) => GenerateDataPoint(e.SignalTime);
        }

        public event EventHandler DataUpdated;

        /// <summary>
        /// Determines the interval at which this MockEEGData class will 
        /// generate data.
        /// Changing this has no effect if the timer is running.
        /// </summary>
        public double Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        private readonly ConcurrentQueue<(long, double)> _dataPoints;
        public IEnumerable<(long, double)> DataPoints => _dataPoints;

        public long LastUpdateTime { get; private set; }

        private readonly Timer _timer;
        private readonly Random _random;

        /// <summary>
        /// Not part of the interface.
        /// Call to create a timer that adds a (random) data point to DataPoints
        /// every Interval milliseconds.
        /// </summary>
        public void StartGenerating()
        {
            _timer.Start();
        }

        /// <summary>
        ///  Call to stop generating data.
        /// </summary>
        public void StopGenerating()
        {
            _timer.Stop();
        }

        public void GenerateDataPoint(DateTime time)
        {
            DateTimeOffset offset = time;
            double voltage = _random.NextDouble();
            long timeMs = offset.ToUnixTimeMilliseconds();

            AddDataPoint(timeMs, voltage);
        }

        public void AddDataPoint(long timeMs, double voltage)
        {
            _dataPoints.Enqueue((timeMs, voltage));

            if (_dataPoints.Count > QUEUE_CAPACITY)
            {
                // Toss oldest, so it doesn't get too big.
                _dataPoints.TryDequeue(out (long, double) _);
            }

            LastUpdateTime = timeMs;

            RaiseDataUpdated();
        }

        private void RaiseDataUpdated()
        {
            DataUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
