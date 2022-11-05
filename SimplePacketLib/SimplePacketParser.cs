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
        private const int BytesPerSample = 2;
        private readonly SimplePacket packet = new SimplePacket();
        private bool foundStartDoublet;
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
        }

        public bool AddByte(byte InByte, SimplePacket PacketBuffer)
        {
            lock (LockObj)
            {

                //shift bytes one position higher
                for (int i = 0; i < (Bytes.Length - 1); i++)
                {
                    Bytes[i] = Bytes[i + 1];
                }

                Bytes[Bytes.Length - 1] = InByte;

                PacketBuffer = DoubletsToPacket(Bytes, PacketBuffer);

                //test for beginning = 0xAA55
                if (Bytes[0] == 0xAA && Bytes[1] == 0x55)
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

        //private class PacketFrameBuffer
        //{
        //    byte[] Bytes { set; get; }
        //    bool synchronized { set; get; }

        //    private PacketFrameBuffer(int Size)
        //    {

        //    }
            
        //    private bool AddByte(byte InByte)
        //    {
        //        //shift bytes one position higher
        //        for (int i = 0; i < Bytes.Length - 1; i++)
        //        {
        //            Bytes[i + 1] = Bytes[i];
        //        }
                
        //        Bytes[0] = InByte;

        //        //test for beginning = FFFF
        //        if (Bytes[0] == 0xFF && Bytes[1] == 0xFF)
        //        {
        //            synchronized = true;
        //            return true;
        //        }
        //        else
        //        { 
        //            return false;
        //        }
        //    }
        //}
    }
}
