namespace SerialCommunication
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
            this.Length = (NumChannels + 2) * 3; // add 2 for start and counter triplets
        }

        /// <summary>
        /// Gets or sets an array of channel values.
        /// </summary>
        public int[] ChannelSet { get; set; }

        /// <summary>
        /// Gets or sets the sequential counter used to test for missed packets.
        /// </summary>
        public int Counter { get; set; }

        /// <summary>
        /// Gets the packet's length, in bytes.
        /// </summary>
        public int Length { get; }
    }
}
