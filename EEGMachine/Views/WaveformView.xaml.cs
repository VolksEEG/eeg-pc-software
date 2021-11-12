using EEGMachine.ViewModels;
using Serilog;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EEGMachine.Views
{
    /// <summary>
    /// Interaction logic for WaveformView.xaml
    /// </summary>
    public partial class WaveformView : UserControl
    {
        private Timer _timer;

        public WaveformViewModel viewModel => DataContext as WaveformViewModel;

        public WaveformView()
        {
            InitializeComponent();

            // Create drawing loop on a timer.
            // TODO: figure out if we can hook into WPF render loops?
            // CompositionTarget.Rendering, e.g.

            // TODO: figure out where to dispose of this timer
            _timer = new Timer();
            _timer.AutoReset = true;
            _timer.Interval = 50; // 50ms = 20 updates per second
            _timer.Elapsed += RedrawCanvas;
        }

        private void RedrawCanvas(object sender, ElapsedEventArgs e)
        {
            WaveformCanvas.InvalidateVisual();
        }

        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            // TODO display scaling
            // e.Info;

            if (viewModel == null)
            {
                Log.Error("PaintSurface called but viewModel is null.");
                return;
            }

            // Draw on canvas based on this WaveformView's datacontext.
            SKCanvas canvas = e.Surface.Canvas;

            // Create an SKPath object based on the viewModel's data points.
            // TODO - make the ViewModel responsible for creating Path object,
            // and we just rescale it?
            SKPath path = new SKPath();

            foreach ((long time, double value) in viewModel.Data.DataPoints)
            {
                path.LineTo(new SKPoint(time, (float)value));
            }

            // Todo - several matrix transformations.

            SKPaint paint = new SKPaint
            {
                Color = SKColors.Black,
                StrokeWidth = 2
            };

            canvas.DrawPath(path, paint);
        }
    }
}
