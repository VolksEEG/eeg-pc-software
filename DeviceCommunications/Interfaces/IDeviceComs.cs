using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceCommunications.Interfaces
{
    public interface IDeviceComs
    {

        void SendStartDataCaptureRequest();
        void SendStopDataCaptureRequest();
        void SendGetDataSinceLastRequest();
    }
}
