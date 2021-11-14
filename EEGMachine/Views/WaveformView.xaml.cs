using EEGMachine.ViewModels;
using Serilog;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace EEGMachine.Views
{
    /// <summary>
    /// Interaction logic for WaveformView.xaml
    /// </summary>
    public partial class WaveformView : UserControl
    {
        public WaveformViewModel viewModel => DataContext as WaveformViewModel;

        public Dispatcher CurrentDispatcher => Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;

        public WaveformView()
        {
            InitializeComponent();

            // Create drawing loop on a timer.
            // TODO: figure out if we can or should hook into WPF render loops?
            // CompositionTarget.Rendering, e.g.
            Loaded += WaveformViewLoaded;
        }

        private void WaveformViewLoaded(object sender, RoutedEventArgs e)
        {
            // Subscribe to the ViewModel's notifications that we should
            // re-draw the canvas.
            // This is done in the Loaded event because the ViewModel is still
            // null in the WaveformView's constructor.
            if (viewModel != null)
            {
                viewModel.RedrawIntervalElapsed += RedrawCanvas;
            }
        }

        private void RedrawCanvas(object sender, ElapsedEventArgs e)
        {
            // This needs to be invoked back on the UI thread - the timer thread does not own
            // the Canvas object, so calling InvalidateVisual from it results in an error.
            CurrentDispatcher.Invoke(() => WaveformCanvas.InvalidateVisual());
        }

        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (viewModel == null)
            {
                Log.Error("PaintSurface called, but viewModel is null.");
                return;
            }

            // If we have no data, do nothing.
            if (viewModel.EEGData.DataPoints.Count() == 0)
            {
                Log.Debug("PaintSurface called, but viewModel has no data.");
                return;
            }

            // The ViewModel is responsible for the actual logic of drawing its
            // waveform to the canvas.
            // This makes things much easier to unit test.
            viewModel.DrawWaveform(e.Surface.Canvas, e.Info);
        }
    }
}
