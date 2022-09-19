namespace SimplePacketLib
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A parser that's fed the incoming serial stream a byte at a time
    /// then returns a populated SimplePacket object
    /// on completing a packet read.
    /// Upon starting, it will read and discard bytes until it synchronizes
    /// by reading the 0xFFFFFF start packet flag.
    /// </summary>
    public class SimplePacketParser
    {
        byte[] Bytes { set; get; }
        bool synchronized { set; get; }
        enum SyncStates { UnSychronized, Synchronized}
        private SyncStates syncStatus;
        private const int BytesPerSample = 2;
        private readonly SimplePacket packet = new SimplePacket();
        //private readonly byte[] packetBuffer;
        //private packetBuffer2;
        private bool foundStartDoublet;
        private int currByteNumInPacket = 0;
        private bool firstPacket;
        static Object LockObj = new Object();

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePacketParser"/> class.
        /// </summary>
        public SimplePacketParser(int Size)
        {
            Bytes = new byte[Size];
            synchronized = false;
        }

        /// <summary>
        /// Resets the parser so it starts looking for an 0xFFFF to synchronize to.
        /// Only needed at the start at a second or subsequent packet-reading run.
        /// </summary>
        public void Reset()
        {
            this.foundStartDoublet = false;
            //this.currByteNumInPacket = 0;
        }

        public bool AddByte(byte InByte, SimplePacket Packet)
        {
            lock (LockObj)
            {

                //shift bytes one position higher
                for (int i = 0; i < (Bytes.Length - 1); i++)
                {
                    Bytes[i] = Bytes[i + 1];
                }

                Bytes[Bytes.Length - 1] = InByte;

                Packet = DoubletsToPacket(Bytes, Packet);

                //test for beginning = FFFF
                if (Bytes[0] == 0xFF && Bytes[1] == 0xFF)
                {
                    synchronized = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Adds a byte received from the serial port, assembling the next packet
        /// Once a full packet is assembled, returns true to alert caller that rxPacket contains a new packet
        /// Otherwise returns false.
        /// </summary>
        /// <param name="inByte">The byte to add to the next packet.</param>
        /// <param name="rxPacket">used to pass back the packet once assembled.</param>
        /// <returns>true if a packet has finished being assembled.</returns>
        //public string AddByte(byte inByte, SimplePacket rxPacket)
        //{
        //    if (this.syncStatus == SyncStates.UnSychronized)
        //    {
        //        // Haven't found a 0xFFFFFF packet start flag yet.
        //        // so we are not yet synchronized
        //        this.packetBuffer[1] = this.packetBuffer[0];
        //        this.packetBuffer[0] = inByte;

        //        if (this.packetBuffer[0] == 0xFF
        //            && this.packetBuffer[1] == 0xFF)
        //        {
        //            // found the packet start flag, so now we know where we are
        //            this.currByteNumInPacket = 2;
        //            this.foundStartDoublet = true;
        //            this.firstPacket = true;
        //        }

        //        return false;
        //    }
        //    else
        //    {
        //        // fill packetBuffer
        //        if (this.firstPacket)
        //        {
        //            this.currByteNumInPacket = 2; // this skips over 0xFFFFFF flag at first 3 bytes in packet
        //            this.firstPacket = false;
        //        }

        //        this.packetBuffer[this.currByteNumInPacket] = inByte;
        //        if (this.currByteNumInPacket != (this.packet.Length - 1))
        //        {
        //            this.currByteNumInPacket++;
        //            return false;
        //        }
        //        else
        //        {
        //            // we have a complete packet to pass back
        //            this.DoubletsToPacket(this.packetBuffer, rxPacket);
        //            this.currByteNumInPacket = 0;
        //            return true;
        //        }
        //    }
        //}

        /// <summary>
        /// Given an array of 24 bytes (8 value triplets),
        /// the values are copied into the 8 channels in the channelSet array.
        /// </summary>
        /// <param name="doublets">An array of 16 bytes, 8 2-byte integers (lsb first).</param>
        /// <param name="packet">The SimplePacket whose channelset values are to be set.</param>
        /// <returns>void.</returns>
        private SimplePacket DoubletsToPacket(byte[] doublets, SimplePacket packet)
        {
            packet.StartFlag = this.ExtractIntFromDoublets(doublets, 0, true);
            packet.Counter = this.ExtractIntFromDoublets(doublets, 1, true);

            // retrieve the channel values
            int numDataDoublets = (doublets.Length / 2) - 2; // -2 to account for start and counter doublets
            for (int i = 0; i < numDataDoublets; i++)
            {
                packet.ChannelSet[i] = this.ExtractIntFromDoublets(doublets, i + 2, false);
            }

            return packet;
        }

        /// <summary>
        /// Converts a single triplet in an array of bytes (lsB first) to an integer.
        /// </summary>
        /// <param name="inBuffer">An array of bytes.</param>
        /// <param name="doubletNumToConvert">The location of the doublet to convert, in doublet units. So "3" means third doublet, starts at 7th byte.</param>
        /// <returns>The doublet's integer value.</returns>
        private int ExtractIntFromDoublets(byte[] inBuffer, int doubletNumToConvert, bool unsigned)
        {
            int intToReturn = 0;
            int byteNum = doubletNumToConvert * 2;
            if (!unsigned)
            {
                short shortVal = BitConverter.ToInt16(inBuffer, byteNum);
                intToReturn = shortVal;
                return intToReturn;
            }
            else
            {
                ushort shortVal = BitConverter.ToUInt16(inBuffer, byteNum);
                intToReturn = shortVal;
                return intToReturn;
            }
        }

        private class PacketFrameBuffer
        {
            byte[] Bytes { set; get; }
            bool synchronized { set; get; }

            private PacketFrameBuffer(int Size)
            {

            }
            
            private bool AddByte(byte InByte)
            {
                //shift bytes one position higher
                for (int i = 0; i < Bytes.Length - 1; i++)
                {
                    Bytes[i + 1] = Bytes[i];
                }
                
                Bytes[0] = InByte;

                //test for beginning = FFFF
                if (Bytes[0] == 0xFF && Bytes[1] == 0xFF)
                {
                    synchronized = true;
                    return true;
                }
                else
                { 
                    return false;
                }
            }
        }
    }
}
