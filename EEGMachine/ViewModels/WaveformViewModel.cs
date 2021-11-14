using EEGDataHandling;
using EEGMachine.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using SkiaSharp;
using System;
using System.Timers;

namespace EEGMachine.ViewModels
{
    public class WaveformViewModel : ObservableObject
    {
        // How often we raise an event to as the view to 
        // re-draw us.
        private const int REFRESH_INTERVAL = 50;

        // Gap between the leading edge of this waveform and the
        // 'old' data we're still showing.
        private const int OLD_TO_NEW_INTERVAL = 200;

        public WaveformViewModel(IEEGData data, ITimeRange timeRange)
        {
            EEGData = data;
            TimeRange = timeRange;
            data.DataUpdated += OnDataUpdated;

            // Create a timer to communicate with the view about when
            // to re-draw paths.
            _timer = new Timer();
            _timer.AutoReset = true;
            _timer.Interval = REFRESH_INTERVAL; // 50ms = 20 updates per second
            _timer.Elapsed += OnRedrawTimerElapsed;
            _timer.Start();
        }

        private Timer _timer;

        public event ElapsedEventHandler RedrawIntervalElapsed;

        // Identifies the time range that should be displayed.
        // The EEG Data may (will) contain data points outside of
        // this time range.
        public ITimeRange TimeRange { get; }

        // TODO: determine how min/max should be handled.
        // Do we auto-scale based on EEGData's min/max, or have fixed values,
        // or allow the user to set these separately?
        public int Min { get; private set; } = 0;
        public int Max { get; private set; } = 1;

        private void OnDataUpdated(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(LastUpdateTime));
        }

        private void OnRedrawTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Pass through the ElapsedEventArgs in case we want timing information
            // later?
            RedrawIntervalElapsed?.Invoke(this, e);
        }

        public IEEGData EEGData { get; }

        public long LastUpdateTime => EEGData.LastUpdateTime;

        public void DrawWaveform(SKCanvas canvas, SKImageInfo info)
        {
            // Set canvas background (black).
            canvas.Clear(SKColors.Black);

            // Create SKPath objects based on the waveforms's data points.
            SKPath newData = new SKPath();
            SKPath oldData = new SKPath();

            bool startedNewData = false;
            bool startedOldData = false;

            // Create two paths:
            // Second path is the old data that's currently being overwritten,
            // First path is the expected one (current data).
            // TODO: This can probably be simplified, and we could cache the SKPath
            // objects we're creating here.
            foreach ((long time, double value) in EEGData.DataPoints)
            {
                // Find the first point that we're going to draw.
                if (time < LastUpdateTime - TimeRange.Duration + OLD_TO_NEW_INTERVAL)
                {
                    continue;
                }
                else if (time >= LastUpdateTime - TimeRange.Duration + OLD_TO_NEW_INTERVAL &&
                         time < TimeRange.StartTime)
                {
                    if (!startedOldData)
                    {
                        startedOldData = true;
                        oldData.MoveTo(time - (TimeRange.StartTime - TimeRange.Duration), (float)value);
                    }
                    else
                    {
                        oldData.LineTo(time - (TimeRange.StartTime - TimeRange.Duration), (float)value);
                    }
                }
                else if (time >= TimeRange.StartTime && time <= TimeRange.EndTime)
                {
                    if (!startedNewData)
                    {
                        startedNewData = true;
                        newData.MoveTo(time - TimeRange.StartTime, (float)value);
                    }
                    else
                    {
                        newData.LineTo(time - TimeRange.StartTime, (float)value);
                    }
                }
                else
                {
                    break;
                }
            }

            // Do time translation first (relative to ViewModel's start/end times), then
            // scale to canvas width.

            // Horizontal Scaling: Based on Duration and canvas width.
            SKMatrix timeScaling = SKMatrix.CreateScale(info.Width / (float)TimeRange.Duration, 1);

            oldData.Transform(timeScaling);
            newData.Transform(timeScaling);

            // Vertical scaling: based on Min / Max and canvas height(inverted).
            SKMatrix verticalScaling = SKMatrix.CreateScale(1, -info.Height / (float)(Max - Min));

            oldData.Transform(verticalScaling);
            newData.Transform(verticalScaling);

            // Vertical translation by height.
            SKMatrix translateDown = SKMatrix.CreateTranslation(0, info.Height);

            oldData.Transform(translateDown);
            newData.Transform(translateDown);

            SKPaint paint = new SKPaint
            {
                Color = new SKColor(0, 175, 0),
                StrokeWidth = 1,
                Style = SKPaintStyle.Stroke
            };

            canvas.DrawPath(oldData, paint);
            canvas.DrawPath(newData, paint);

            // Draw leading edge line
            SKPath verticalLine = new SKPath();

            verticalLine.MoveTo(LastUpdateTime - TimeRange.StartTime, 0);
            verticalLine.LineTo(LastUpdateTime - TimeRange.StartTime, info.Height);
            verticalLine.Transform(timeScaling);

            canvas.DrawPath(verticalLine, paint);
        }
    }
}
