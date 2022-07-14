using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace EEGDataHandling
{
    /// <summary>
    /// A parser that's fed the incoming serial stream a byte at a time
    /// then returns a populated SimplePacket object
    /// on completing a packet read.
    /// Upon starting, it will read and discard bytes until it synchronizes
    /// by reading the 0xFFFFFF start packet flag.
    /// </summary>
    public class SimplePacketReaderParser
    {
        private const int BytesPerSample = 2;
        private readonly SimplePacket packet = new SimplePacket();
        private readonly byte[] packetBuffer;
        private bool foundStartDoublet;
        private int currByteNumInPacket = 0;
        private bool firstPacket;
        private SerialPort port;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplePacketParser"/> class.
        /// </summary>
        public SimplePacketReaderParser()
        {
            this.packetBuffer = new byte[this.packet.Length];
            this.port = new SerialPort("COM6", 115200); /////////////////////////////////////////////////////// Ended Here
            this.port.DataReceived += new SerialDataReceivedEventHandler(this.Port_DataReceived);
        }

        /// <summary>
        /// Resets the parser so it starts looking for an 0xFFFF to synchronize to.
        /// Only needed at the start at a second or subsequent packet-reading run.
        /// </summary>
        public void Reset()
        {
            this.foundStartDoublet = false;
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
        public bool AddByte(byte inByte, SimplePacket rxPacket)
        {
            if (!this.foundStartDoublet)
            {
                // Haven't found a 0xFFFFFF packet start flag yet.
                // so we are not yet synchronized
                this.packetBuffer[1] = this.packetBuffer[0];
                this.packetBuffer[0] = inByte;

                if (this.packetBuffer[0] == 0xFF
                    && this.packetBuffer[1] == 0xFF)
                {
                    // found the packet start flag, so now we know where we are
                    this.currByteNumInPacket = 2;
                    this.foundStartDoublet = true;
                    this.firstPacket = true;
                }

                return false;
            }
            else
            {
                // fill packetBuffer
                if (this.firstPacket)
                {
                    this.currByteNumInPacket = 2; // this skips over 0xFFFFFF flag at first 3 bytes in packet
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
                    this.DoubletsToPacket(this.packetBuffer, rxPacket);
                    this.currByteNumInPacket = 0;
                    return true;
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
            //intToReturn = i;
            //intToReturn += (int)inBuffer[byteNum];
            //intToReturn += (int)inBuffer[byteNum + 1] * 0x000100;
            //return intToReturn;
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            /*
            int numbytes = this.port.BytesToRead;
            for (int i = 0; i < numbytes; i++)
            {
                // if (numPacketsSoFar < updNumPackets.Value)
                if (this.areReading == true)
                {
                    int inInt = this.port.ReadByte(); // ReadByte returns an int. Weird.
                    byte tempByte = (byte)inInt;
                    bool newPacket = this.packetizer.AddByte(tempByte, this.packetBuffer[this.numPacketsSoFar]);
                    if (newPacket)
                    {

                        if ((this.numPacketsSoFar > 0) && (this.packetBuffer[this.numPacketsSoFar].Counter != this.lastCounter + 1))
                        {
                            throw new Exception("oops -- out of sync");
                        }

                        this.lastCounter = this.packetBuffer[this.numPacketsSoFar].Counter;
                        this.numPacketsSoFar++;
                        newPacket = false;
                    }

                    if (this.numPacketsSoFar == (this.updNumPackets.Value - 1))
                    {
                        this.port.Close();
                        this.areReading = false;
                        this.endTime = DateTime.Now;
                        var diffInSeconds = (this.endTime - this.startTime).TotalSeconds;
                        double freq = Convert.ToDouble(this.updNumPackets.Value) / diffInSeconds;
                        this.lblFreq.Invoke(this.updateUiDelegate, new object[] { diffInSeconds, freq, this.packetBuffer });
                    }
                }
            }
            */
        }
    }
}
