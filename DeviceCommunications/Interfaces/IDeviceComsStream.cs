using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceCommunications.Interfaces
{
    public interface IDeviceComsStream
    {
        bool IsConnected { get; }
        bool Connect();
        int TransmitData(byte[] data);
        int GetReceivedData(out byte[] data);

        event EventHandler DataReceivedEvent;
    }
}
