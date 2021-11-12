using EEGMachine.ViewModels;
using Serilog;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace EEGMachine.Views
{
    /// <summary>
    /// Interaction logic for WaveformView.xaml
    /// </summary>
    public partial class WaveformView : UserControl
    {
        private Timer _timer;

        public WaveformViewModel viewModel => DataContext as WaveformViewModel;

        public Dispatcher CurrentDispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

        public WaveformView()
        {
            InitializeComponent();

            // Create drawing loop on a timer.
            // TODO: figure out if we can or should hook into WPF render loops?
            // CompositionTarget.Rendering, e.g.

            // TODO: figure out where to dispose of this timer
            _timer = new Timer();
            _timer.AutoReset = true;
            _timer.Interval = 50; // 50ms = 20 updates per second
            _timer.Elapsed += RedrawCanvas;
            _timer.Start();
        }

        private void RedrawCanvas(object sender, ElapsedEventArgs e)
        {
            // This needs to be invoked back on the UI thread - the timer thread does not own
            // the Canvas object, so calling InvalidateVisual from it results in an error.
            CurrentDispatcher.Invoke(() => WaveformCanvas.InvalidateVisual());
        }

        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            // TODO: display scaling
            Log.Debug($"SKCanvas info: Height={e.Info.Height}, Width={e.Info.Width}, Empty={e.Info.IsEmpty}.");

            if (viewModel == null)
            {
                Log.Error("PaintSurface called, but viewModel is null.");
                return;
            }

            // If we have no data, do nothing
            if (viewModel.EEGData.DataPoints.Count() == 0)
            {
                Log.Debug("PaintSurface called, but viewModel has no data.");
                return;
            }

            // Draw on canvas based on this WaveformView's datacontext.
            SKCanvas canvas = e.Surface.Canvas;

            // Set canvas background (black)
            canvas.Clear(SKColors.Black);

            // Create an SKPath object based on the viewModel's data points.
            // TODO - make the ViewModel responsible for creating Path object,
            // and we just rescale it?
            SKPath path = new SKPath();

            long startTime = 0;
            double firstValue = 0;
            bool started = false;

            foreach ((long time, double value) in viewModel.EEGData.DataPoints)
            {
                // Find the first point within our time range.
                if (time < viewModel.TimeRange.StartTime)
                {
                    startTime = time;
                    firstValue = value;
                    continue;
                }

                if (!started)
                {
                    path.MoveTo(startTime - viewModel.TimeRange.StartTime, (float)firstValue);
                    started = true;
                }

                path.LineTo(new SKPoint(time - viewModel.TimeRange.StartTime, (float)value));

                if (time > viewModel.TimeRange.EndTime)
                {
                    break;
                }
            }

            // Do time translation first (relative to ViewModel's start/end times), then
            // scale to canvas width.

            // Scaling: Based on Duration and canvas width; and on Min/Max and canvas height (inverted).
            SKMatrix timeScaling = SKMatrix.CreateScale(
                e.Info.Width / (float)viewModel.TimeRange.Duration,
                -e.Info.Height / (float)(viewModel.Max - viewModel.Min));

            path.Transform(timeScaling);

            // Vertical translation by height.
            SKMatrix translateDown = SKMatrix.CreateTranslation(0, e.Info.Height);
            path.Transform(translateDown);

            SKPaint paint = new SKPaint
            {
                Color = new SKColor(0, 175, 0),
                StrokeWidth = 1,
                Style = SKPaintStyle.Stroke
            };

            canvas.DrawPath(path, paint);
        }
    }
}
