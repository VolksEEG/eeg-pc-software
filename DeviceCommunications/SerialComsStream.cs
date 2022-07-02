using DeviceCommunications.Interfaces;
using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceCommunications
{
    public class SerialComsStream : IDeviceComsStream
    {
        private SerialPort _SerialPort = new SerialPort();

        private CancellationTokenSource _cts;

        private object _RxDataLock = new object();
        private bool _RxDataRead;


        // Class functions
        public SerialComsStream(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            _SerialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            _SerialPort.DtrEnable = true;
            _SerialPort.RtsEnable = true;
        }

        private void MonitorForReceivedData(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                lock (_RxDataLock)
                {
                    if  (
                            (false == _RxDataRead)
                            && (0 != _SerialPort.BytesToRead)
                        )
                    {
                        if (null != DataReceivedEvent)
                        {
                            DataReceivedEvent(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        // Interface functions

        public event EventHandler DataReceivedEvent;

        bool IDeviceComsStream.IsConnected
        {
            get
            {
                return _IsConnected;
            }
        }

        private bool _IsConnected
        {
            get { return (bool)_SerialPort.IsOpen; }
        }

        bool IDeviceComsStream.Connect()
        {
            if (_IsConnected)
            {
                _SerialPort.Close();
            }

            _SerialPort.Open();

            if (_IsConnected)
            {
                // now that we are connected, make sure we monitor for received data
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                _RxDataRead = false;
                Task.Run(() => MonitorForReceivedData(_cts.Token), _cts.Token);
            }

            return _IsConnected;
        }

        int IDeviceComsStream.GetReceivedData(out byte[] data)
        {
            int bytesRead = 0;

            lock (_RxDataLock)
            {
                _RxDataRead = false;

                int bytesToRead = _SerialPort.BytesToRead;

                if (bytesToRead == 0)
                {
                    data = new byte[0];
                    return 0;
                }

                // there are bytes to read 
                data = new byte[bytesToRead];

                bytesRead = _SerialPort.Read(data, 0, bytesToRead);
            }

            // read them and return the number of bytes read
            return bytesRead;
        }

        int IDeviceComsStream.TransmitData(byte[] data)
        {
            if (0 == data.Length)
            {
                return 0;
            }

            _SerialPort.Write(data, 0, data.Length);

            return data.Length;
        }
    }
}
