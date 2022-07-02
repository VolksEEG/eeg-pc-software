using EEGDataHandling;
using EEGMachine.Interfaces;
using EEGMachine.ViewModels;
using Moq;
using NUnit.Framework;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace EEGMachineTests
{
    public class WaveformViewModelTest
    {
        private Mock<IEEGData> _mockEEGData;
        private Mock<ITimeRange> _mockTimeRange;

        [SetUp]
        public void Setup()
        {
            _mockEEGData = new Mock<IEEGData>();
            _mockTimeRange = new Mock<ITimeRange>();
        }

        [Test]
        public void TestWaveformViewModelDrawsLine()
        {
            // Set up IEEGData to have a straight line as its data points.
            List<(long, EEGData)> line = new List<(long, EEGData)>() { (0, new EEGData(new double[] { 0.5 })), (1000, new EEGData(new double[] { 0.5 })), (2000, new EEGData(new double[] { 0.5 })) };
            _mockEEGData.Setup(x => x.DataPoints).Returns(line);
            _mockEEGData.Setup(x => x.LastUpdateTime).Returns(2000);
            _mockEEGData.Setup(x => x.DigitalMinimum).Returns(0);
            _mockEEGData.Setup(x => x.DigitalMaximum).Returns(1);

            // Set up time range to cover the 2-second time span we've given.
            _mockTimeRange.Setup(x => x.StartTime).Returns(0);
            _mockTimeRange.Setup(x => x.Duration).Returns(3000);
            _mockTimeRange.Setup(x => x.EndTime).Returns(3000);

            SKBitmap bitmap = new SKBitmap(4, 3);
            SKCanvas canvas = new SKCanvas(bitmap);

            WaveformViewModel viewModel = new WaveformViewModel(_mockEEGData.Object, _mockTimeRange.Object);
            viewModel.DrawWaveform(canvas, bitmap.Info);

            // Assert we drew what was expected:
            // A backround of the appropriate color
            // A horizontal line through the middle of the appropriate color
            // A vertical line at the 3rd column of the appropriate color (leading edge)

            // First row (with leading edge)
            AssertColorMatches(WaveformViewModel.BACKGROUND, bitmap.Bytes, 0);
            AssertColorMatches(WaveformViewModel.BACKGROUND, bitmap.Bytes, 4);
            AssertColorMatches(WaveformViewModel.STROKE, bitmap.Bytes, 8);
            AssertColorMatches(WaveformViewModel.BACKGROUND, bitmap.Bytes, 12);

            // Second row (line)
            AssertColorMatches(WaveformViewModel.STROKE, bitmap.Bytes, 16);
            AssertColorMatches(WaveformViewModel.STROKE, bitmap.Bytes, 20);
            AssertColorMatches(WaveformViewModel.STROKE, bitmap.Bytes, 24);
            AssertColorMatches(WaveformViewModel.BACKGROUND, bitmap.Bytes, 28);

            // Third row (with leading edge)
            AssertColorMatches(WaveformViewModel.BACKGROUND, bitmap.Bytes, 32);
            AssertColorMatches(WaveformViewModel.BACKGROUND, bitmap.Bytes, 36);
            AssertColorMatches(WaveformViewModel.STROKE, bitmap.Bytes, 40);
            AssertColorMatches(WaveformViewModel.BACKGROUND, bitmap.Bytes, 44);
        }

        private void AssertColorMatches(SKColor color, byte[] bytes, int byteIndex)
        {
            Assert.AreEqual(color.Red, bytes[byteIndex]);
            Assert.AreEqual(color.Green, bytes[byteIndex + 1]);
            Assert.AreEqual(color.Blue, bytes[byteIndex + 2]);
            Assert.AreEqual(color.Alpha, bytes[byteIndex + 3]);
        }

        [Test]
        public void TestDataUpdateRaisesEvent()
        {
            _mockEEGData.Setup(x => x.LastUpdateTime).Returns(1);

            bool propertyChangedFired = false;

            WaveformViewModel viewModel = new WaveformViewModel(_mockEEGData.Object, _mockTimeRange.Object);

            viewModel.PropertyChanged += (obj, args) =>
            {
                propertyChangedFired = true;
                Assert.AreEqual(nameof(viewModel.LastUpdateTime), args.PropertyName);
            };

            // Raise mock EEG data's event
            _mockEEGData.Raise(x => x.DataUpdated += null, new EventArgs());

            // That should cause our ViewModel's property changed event to fire.
            Assert.IsTrue(propertyChangedFired);
        }
    }
}