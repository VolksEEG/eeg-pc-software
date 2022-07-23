namespace SerialCommunication
{
    /// <summary>
    /// A parser that's fed the incoming serial stream a byte at a time
    /// then returns a populated SimplePacket object
    /// on completing a packet read.
    /// Upon starting, it will read and discard bytes until it synchronizes
    /// by reading the 0xFFFFFF start packet flag.
    /// </summary>
    public class SimplePacketParser
    {
        private const int BytesPerSample = 3;
        private readonly SimplePacket packet = new SimplePacket();
        private readonly byte[] packetBuffer;
        private bool foundStartTriplet;
        private int currByteNumInPacket = 0;
        private bool firstPacket;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePacketParser"/> class.
        /// </summary>
        public SimplePacketParser()
        {
            this.packetBuffer = new byte[this.packet.Length];
        }

        /// <summary>
        /// Resets the parser so it starts looking for an 0xFFFFFF to synchronize to.
        /// Only needed at the start at a second or subsequent packet-reading run.
        /// </summary>
        public void Reset()
        {
            this.foundStartTriplet = false;
            this.currByteNumInPacket = 0;
        }

        /// <summary>
        /// Adds a byte received from the serial port, assembling the next packet
        /// Once a full packet is assembled, returns true to alert caller that rxPacket contains a new packet
        /// Otherwise returns false.
        /// </summary>
        /// <param name="inByte">The byte to add to the next packet.</param>
        /// <param name="rxPacket">used to pass back the packet once assembled.</param>
        /// <returns>true if a packet has finished being assembled.</returns>
        public bool AddByte(byte inByte, out SimplePacket rxPacket)
        {
            // Initialize rxPacket to null.
            rxPacket = null;

            if (!this.foundStartTriplet)
            {
                // Haven't found a 0xFFFFFF packet start flag yet.
                // so we are not yet synchronized
                this.packetBuffer[2] = this.packetBuffer[1];
                this.packetBuffer[1] = this.packetBuffer[0];
                this.packetBuffer[0] = inByte;

                if (this.packetBuffer[0] == 0xFF
                    && this.packetBuffer[1] == 0xFF
                    && this.packetBuffer[2] == 0xFF)
                {
                    // found the packet start flag, so now we know where we are
                    this.currByteNumInPacket = 3;
                    this.foundStartTriplet = true;
                    this.firstPacket = true;
                }

                return false;
            }
            else
            {
                // fill packetBuffer
                if (this.firstPacket)
                {
                    this.currByteNumInPacket = 3; // this skips over 0xFFFFFF flag at first 3 bytes in packet
                    this.firstPacket = false;
                }

                this.packetBuffer[this.currByteNumInPacket] = inByte;
                if (this.currByteNumInPacket != (this.packet.Length - 1))
                {
                    this.currByteNumInPacket++;
                    return false;
                }
                else
                {
                    // we have a complete packet to pass back
                    rxPacket = this.TripletsToPacket(this.packetBuffer);
                    this.currByteNumInPacket = 0;
                    return true;
                }
            }
        }

        /// <summary>
        /// Given an array of 24 bytes (8 value triplets),
        /// the values are copied into the 8 channels in the channelSet array.
        /// </summary>
        /// <param name="triplets">An array of 24 bytes, 8 3-byte integers (lsb first).</param>
        /// <param name="packet">The SimplePacket whose channelset values are to be set.</param>
        /// <returns>void.</returns>
        private SimplePacket TripletsToPacket(byte[] triplets)
        {
            SimplePacket packet = new SimplePacket
            {
                Counter = this.ExtractIntFromTriplets(triplets, 1),
            };

            // retrieve the channel values
            int numDataTriplets = (triplets.Length / 3) - 2; // -1 to account for start and counter triplets
            for (int i = 0; i < numDataTriplets; i++)
            {
                packet.ChannelSet[i] = this.ExtractIntFromTriplets(triplets, i + 2);
            }

            return packet;
        }

        /// <summary>
        /// Converts a single triplet in an array of bytes (lsB first) to an integer.
        /// </summary>
        /// <param name="inBuffer">An array of bytes.</param>
        /// <param name="tripletNumToConvert">The location of the triplet to convert, in triplet units. So "3" means third triplet, starts at 7th byte.</param>
        /// <returns>The triplet's integer value.</returns>
        private int ExtractIntFromTriplets(byte[] inBuffer, int tripletNumToConvert)
        {
            int intToReturn = 0;
            int byteNum = tripletNumToConvert * 3;
            intToReturn += (int)inBuffer[byteNum];
            intToReturn += (int)inBuffer[byteNum + 1] * 0x000100;
            intToReturn += (int)inBuffer[byteNum + 2] * 0x010000;
            return intToReturn;
        }
    }
}
