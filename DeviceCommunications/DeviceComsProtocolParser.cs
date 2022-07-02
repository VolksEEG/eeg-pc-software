using DeviceCommunications.Interfaces;
using EEGDataHandling;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace DeviceCommunications
{
    public class DeviceComsProtocolParser : IDeviceComs, IEEGData
    {
        // Set capacity to 30 seconds at 500 samples/second
        private const int QUEUE_CAPACITY = 500 * 30;

        // use an abstracted device coms stream.
        IDeviceComsStream _DeviceComsStream;

        private object _dataPointsLock = new object();

        private readonly ConcurrentQueue<(long, EEGData)> _dataPoints;

        public DeviceComsProtocolParser(IDeviceComsStream deviceComsStream)
        {
            _DeviceComsStream = deviceComsStream;

            if (_DeviceComsStream.Connect())
            {
                // if device is connected then subscribe to it's DataReceived events
                _DeviceComsStream.DataReceivedEvent += DataReceivedEventHandler;
            }

            _dataPoints = new ConcurrentQueue<(long, EEGData)>();
        }

        public event EventHandler DataUpdated;

        void IDeviceComs.SendGetDataSinceLastRequest()
        {
            byte[] data = new byte[3];

            data[0] = 0xFF;
            data[1] = 0xFF;
            data[2] = 0x03;

            _DeviceComsStream.TransmitData(data);
        }

        void IDeviceComs.SendStartDataCaptureRequest()
        {
            byte[] data = new byte[3];

            data[0] = 0xFF;
            data[1] = 0xFF;
            data[2] = 0x01;

            _DeviceComsStream.TransmitData(data);
        }

        void IDeviceComs.SendStopDataCaptureRequest()
        {
            byte[] data = new byte[3];

            data[0] = 0xFF;
            data[1] = 0xFF;
            data[2] = 0x02;

            _DeviceComsStream.TransmitData(data);
        }

        private int _RxIndex = 0;
        private byte[] _RxData = new byte[27];

        EEGSignalMetadata IEEGData.SignalMetadata => throw new NotImplementedException();

        long _LastUpdateTime;
        long IEEGData.LastUpdateTime { get { return _LastUpdateTime; } }

        /// <summary>
        /// Also determines the interval at which this MockEEGData class will 
        /// generate data.
        /// Changing this has no effect if the timer is running.
        /// </summary>
        double IEEGData.DigitalMinimum { get; } = -1;

        double IEEGData.DigitalMaximum { get; } = 1;

        double IEEGData.PhysicalMinimum { get; } = -20;

        double IEEGData.PhysicalMaximum { get; } = 20;

        EEGSignalMetadata SignalMetadata { get; } = new EEGSignalMetadata()
        {
            Label = "EEG",
            Units = "mV",
            TransducerType = "UNKNOWN",
            Prefiltering = "UNKNOWN",
        };

        // Returns a copy whenever it's accessed, to avoid concurrency issues.
        public IEnumerable<(long, EEGData)> DataPoints
        {
            get
            {
                lock (_dataPointsLock)
                {
                    return _dataPoints.ToArray();
                }
            }
        }

        private void DataReceivedEventHandler(object sender, EventArgs e)
        {
            byte[] rxData;

            int count = _DeviceComsStream.GetReceivedData(out rxData);

            foreach (byte b in rxData)
            {
                _RxData[_RxIndex] = b;

                switch (_RxIndex)
                {
                    case 0:
                        if (0xFF == b)
                        {
                            _RxIndex = 1;
                            break;
                        }

                        _RxIndex = 0;
                        break;
                    case 1:
                        if (0xFF == b)
                        {
                            _RxIndex = 2;
                            break;
                        }

                        _RxIndex = 0;
                        break;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                        _RxIndex++;
                        break;
                    case 19:
                        // all data captured so parse it.
                        long timeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                        double[] cData = new double[8];

                        for (int i = 0; i < 8; i++)
                        {
                            cData[i] = BitConverter.ToInt16(_RxData, ((i * 2) + 4));
                        }

                        EEGData voltages = new EEGData(cData);

                        lock (_dataPointsLock)
                        {
                            _dataPoints.Enqueue((timeMs, voltages));

                            if (_dataPoints.Count > QUEUE_CAPACITY)
                            {
                                // Toss oldest, so the queue doesn't get too big.
                                _dataPoints.TryDequeue(out _);
                            }
                        }

                        _LastUpdateTime = timeMs;

                        DataUpdated?.Invoke(this, EventArgs.Empty);

                        _RxIndex = 0;
                        break;
                }
            }
        }
    }
}
