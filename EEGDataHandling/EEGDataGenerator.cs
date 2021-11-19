﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace EEGDataHandling
{
    /// <summary>
    /// A class for holding mocked EEG data for testing.
    public class EEGDataGenerator : IEEGData
    {
        // Set capacity to 30 seconds at 500 samples/second
        private const int QUEUE_CAPACITY = 500 * 30;

        /// <summary>
        /// Generator for time-series EEG data
        /// For simplicity (for now), timestamps are in milliseconds from
        /// 'start' and values are in the range (0, 1).
        /// </summary>
        public EEGDataGenerator()
        {
            _dataPoints = new ConcurrentQueue<(long, double)>();
        }

        public event EventHandler DataUpdated;

        private object _dataPointsLock = new object();

        /// <summary>
        /// Determines the interval at which this MockEEGData class will 
        /// generate data.
        /// Changing this has no effect if the timer is running.
        /// </summary>
        public int Interval { get; set; } = 2;

        private readonly ConcurrentQueue<(long, double)> _dataPoints;

        // Returns a copy whenever it's accessed, to avoid concurrency issues.
        public IEnumerable<(long, double)> DataPoints
        {
            get
            {
                lock (_dataPointsLock)
                {
                    return _dataPoints.ToArray();
                }
            }
        }

        public long LastUpdateTime { get; private set; }

        private CancellationTokenSource _cts;

        /// <summary>
        /// Not part of the interface.
        /// Call to create a timer that adds a (random) data point to DataPoints
        /// every Interval milliseconds.
        /// </summary>
        public void StartGenerating()
        {
            // Initiate a task.
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            Task.Run(() => GenerateValues(_cts.Token), _cts.Token);
        }

        private void GenerateValues(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                GenerateDataPoint(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

                Thread.Sleep(2);
            }
        }

        /// <summary>
        ///  Call to stop generating data.
        /// </summary>
        public void StopGenerating()
        {
            _cts?.Cancel();
        }

        public void GenerateDataPoint(long timeMs)
        {
            double voltage = (Math.Sin(timeMs / 1000.0 * Math.PI) + 1) / 2.0;

            AddDataPoint(timeMs, voltage);
        }

        public void AddDataPoint(long timeMs, double voltage)
        {
            lock (_dataPointsLock)
            {
                _dataPoints.Enqueue((timeMs, voltage));

                if (_dataPoints.Count > QUEUE_CAPACITY)
                {
                    // Toss oldest, so the queue doesn't get too big.
                    _dataPoints.TryDequeue(out _);
                }
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
