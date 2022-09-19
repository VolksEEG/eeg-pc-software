namespace SimplePacketLib
{
    /// <summary>
    /// Implements a simple 8-channel EEG packet
    /// As described in https://github.com/VolksEEG/VolksEEG/wiki/EEG-Box-to-PC-Streaming-Protocol.
    /// </summary>
    public class SimplePacket
    {
        private const int NumChannels = 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePacket"/> class.
        /// </summary>
        public SimplePacket()
        {
            this.ChannelSet = new int[NumChannels];
            this.Length = (NumChannels + 2) * 2; // 2 bytes per sample per channel, add 2 for start and counter doublets
        }

        /// <summary>
        /// Gets or sets an array of channel values.
        /// </summary>
        public int[] ChannelSet { get; set; }

        /// <summary>
        /// Gets or sets the start flag for the packets (should be 65536)
        /// </summary>
        public int Counter { get; set; }

        /// <summary>
        /// Gets or sets the sequential counter used to test for missed packets.
        /// </summary>
        public int StartFlag { get; set; }

        /// <summary>
        /// Gets the packet's length, in bytes.
        /// </summary>
        public int Length { get; }
    }
}