using SerialCommunication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EEGDataHandling
{
    /// <summary>
    /// An EEG reader for the simple packet format (used for demo purposes).
    /// </summary>
    public class SimplePacketSerialEEGReader : IEEGData
    {
        private const int BUFFER_SIZE = 1024;

        public SimplePacketSerialEEGReader(Stream serialStream, SimplePacketParser parser)
        {
            this.serialStream = serialStream;
            this.parser = parser;
        }

        private Stream serialStream;
        private SimplePacketParser parser;
        private object dataPointsLock = new object();

        /// <summary>
        /// Raised each time we receive a complete packet from our
        /// serial stream.
        /// </summary>
        public event EventHandler DataUpdated;

        /// <summary>
        /// Kicks off a task that continuously reads bytes from this reader's
        /// serial stream.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous read operation.
        /// </returns>
        public async Task ReadBytes()
        {
            byte[] buffer = new byte[BUFFER_SIZE];

            while (this.serialStream.CanRead)
            {
                int bytesRead = await this.serialStream.ReadAsync(buffer, 0, BUFFER_SIZE);

                // Add to SimplePacketParser one at a time.
                for (int i = 0; i < bytesRead; i++)
                {
                    bool foundPacket = this.parser.AddByte(buffer[i], out SimplePacket rxPacket);

                    // Add to DataPoints
                    if (foundPacket)
                    {
                        // locking around dataPoints, because of concurrent access.
                        lock (this.dataPointsLock)
                        {
                            // Just taking data from channel 0 for now - will need to re-work the IEEGData interface to
                            // provide access to multiple channels.
                            this.dataPoints.Add((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), rxPacket.ChannelSet[0]));
                        }

                        // Once we've released the lock (important), notify that the data has been updated.
                        this.DataUpdated?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        public long LastUpdateTime { get; private set; }

        public double DigitalMinimum { get; private set; }

        public double DigitalMaximum { get; private set; }

        public double PhysicalMinimum { get; private set; }

        public double PhysicalMaximum { get; private set; }

        public EEGSignalMetadata SignalMetadata { get; private set; }

        // This might need to be a concurrent queue like EEGDataGenerator, but I'm unsure
        // (we are locking when reading and writing which may accomplish what we need already).
        private List<(long timestamp, double value)> dataPoints;

        public IEnumerable<(long timestamp, double value)> DataPoints
        {
            get
            {
                lock (this.dataPointsLock)
                {
                    return this.dataPoints.ToArray();
                }
            }
        }
    }
}
