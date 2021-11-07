using System;
using System.Collections.Generic;
using System.Timers;

namespace EEGDataHandling
{
    /// <summary>
    /// A class for holding mocked EEG data for testing.
    public class EEGDataGenerator
    {
        /// <summary>
        /// Generator for time-series EEG data
        /// For simplicity (for now), timestamps are in milliseconds from
        /// 'start' and values are in the range (0, 1).
        /// </summary>
        public EEGDataGenerator()
        {
            dataPoints = new List<(long, double)>();
            random = new Random();
            timer = new Timer();

            timer.Interval = 25;
        }

        /// <summary>
        /// Determines the interval at which this MockEEGData class will 
        /// generate data.
        /// Changing this has no effect if the timer is running.
        /// </summary>
        public double Interval
        {
            get => timer.Interval;
            set => timer.Interval = value;
        }

        private List<(long, double)> dataPoints;
        public IEnumerable<(long, double)> DataPoints => dataPoints;

        private Timer timer;
        private Random random;

        /// <summary>
        /// Not part of the interface.
        /// Call to create a timer that adds a (random) data point to DataPoints
        /// every Interval milliseconds.
        /// </summary>
        public void StartGenerating()
        {
            timer.Interval = Interval;

            timer.AutoReset = true;
            timer.Elapsed += (sender, e) => GenerateDataPoint(e.SignalTime);

            timer.Start();
        }

        /// <summary>
        ///  Call to stop generating data.
        /// </summary>
        public void StopGenerating()
        {
            timer.Stop();
        }

        public void GenerateDataPoint(DateTime time)
        {
            DateTimeOffset offset = time;
            double voltage = random.NextDouble();
            long timeMs = offset.ToUnixTimeMilliseconds();

            AddDataPoint(timeMs, voltage);
        }

        public void AddDataPoint(long timeMs, double voltage)
        {
            dataPoints.Add((timeMs, voltage));
        }
    }
}
