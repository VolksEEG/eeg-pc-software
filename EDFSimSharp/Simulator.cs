// <copyright file="frmMain.cs" company="VolksEEG Project">
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </copyright>

namespace EDFSimSharp
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.IO.Ports;
    using System.Diagnostics;
    using EDFSharp;
    using System.Threading;

    public partial class Simulator : Form
    {
        // TODO: Test with other sample files
        // TODO: Test for EDF files wit < 8 samples

        private int numDisplayChans = 8;
        private SerialPort serialPort = new SerialPort();
        private EDFFile edfFile;
        private Header edfHeader;
        //private bool[] chanAcceptable;
        float secsPerSample;
        int outSampleCount;
        DateTime startTime;
        long numSamples;
        int numChans;
        Stopwatch stopwatch = new Stopwatch();
        private List<ChanExtraInfo> chanExtraInfos = new List<ChanExtraInfo>();
        enum States {Idle, Sending, TryingToSend };
        bool writingSuccessfully = true;
        int prevPacketNum = 0;


        public Simulator()
        {
            InitializeComponent();
        }

        private void btnFileSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open an EDF File";
            ofd.Filter = "EDF Files (*.edf)|*.edf|All Files (*.*)|*.*"; //Filter which all files to open
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                lblSelectedFile.Text = ofd.FileName;
                ChangeToNeedEDFFileState();
                ChangeToStoppedState();

            }
        }

        private void btnSimulate_Click(object sender, EventArgs e)
        {
            //to do
            //--- deal with > 8 or < 8 channels
            try
            {
                this.serialPort.PortName = this.cmbPort.Text;
                Properties.Settings.Default.SelectedPort = this.cmbPort.Text;
                this.serialPort.BaudRate = 921600;
                this.serialPort.Parity = Parity.None;
                this.serialPort.DataBits = 8;
                this.serialPort.StopBits = StopBits.One;
                this.serialPort.WriteTimeout = 10;
                this.serialPort.Open();
                startTime = DateTime.Now;
                ReadFile();
                stopwatch.Restart();
                ChangeToAcquiringState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void ReadFile()
        {
            //read the edf file
            EDFReader reader = new EDFReader(null);
            using (Stream stream = File.OpenRead(lblSelectedFile.Text))
            {
                edfFile = await reader.Read(stream);
                edfHeader = edfFile.Header;
                numChans = edfFile.Signals.Count;
            }

            int acceptableSampleCountPerRecord = edfFile.Signals[0].SignalHeader.SampleCountPerRecord;
            secsPerSample = ((float)edfHeader.RecordDurationInSeconds) / acceptableSampleCountPerRecord;
            numSamples = edfFile.Signals[0].Values.Count;
            for (int chan = 0; chan < numChans; chan++)
            {
                ChanExtraInfo c = new ChanExtraInfo();
                chanExtraInfos.Add(c);

                //check for acceptable sampling rate
                if (edfFile.Signals[chan].SignalHeader.SampleCountPerRecord == acceptableSampleCountPerRecord)
                {
                    chanExtraInfos[chan].chanAcceptable = true;
                }
                else
                {
                    chanExtraInfos[chan].chanAcceptable = false;
                }

                //find multiplier and offset
                double deltaPhys = edfFile.Signals[chan].SignalHeader.PhysicalMaximum
                    - edfFile.Signals[chan].SignalHeader.PhysicalMinimum;
                double deltaDig = edfFile.Signals[chan].SignalHeader.DigitalMaximum
                    - edfFile.Signals[chan].SignalHeader.DigitalMinimum;
                chanExtraInfos[chan].multiplier = deltaPhys / deltaDig;
                chanExtraInfos[chan].offset = edfFile.Signals[chan].SignalHeader.PhysicalMaximum
                    - ((chanExtraInfos[chan].multiplier) * edfFile.Signals[chan].SignalHeader.DigitalMaximum);
            }

        }

        private long GetShouldBeSampleNumber()
        {
            TimeSpan ts = DateTime.Now - startTime;
            double msec = ts.TotalMilliseconds;
            float secs = (float)((msec) / 1000.0);
            long shouldBeSampleNumber = (long)(secs / secsPerSample);
            //Debug.Print(shouldBeSampleNumber.ToString());
            return shouldBeSampleNumber;
        }

        private class ChanExtraInfo
        {
            public bool chanAcceptable;
            public Double multiplier;
            public Double offset;
        }

        private void SendOutPacket(int packetNum)
        {
            //Just for testing - probably remove
            if (packetNum != prevPacketNum + 1)
            {
                MessageBox.Show("oopsie. missed one. This = " + packetNum + ", previous = " + prevPacketNum);
            }
            else
            {
                prevPacketNum = packetNum;
            }

            {
                //start of packet flag
                byte[] startFlag = { 255, 255 };
                serialPort.Write(startFlag, 0, 2);

                //counter
                uint counter = (uint)(packetNum % Math.Pow(2, 16));
                WriteUintAsTwoBytes(counter);

                for (int chanNum = 0; chanNum < numDisplayChans; chanNum++)
                {
                    //read the sample
                    short sampleVal = edfFile.Signals[chanNum].Values[packetNum];

                    //convert to physical units
                    short samplePhysVal = (short)((sampleVal * chanExtraInfos[chanNum].multiplier)
                        + chanExtraInfos[chanNum].offset);
                    WriteUintAsTwoBytes((uint)samplePhysVal);
                }
                byte[] ZeroByte = new byte[1];
                ZeroByte[0] = 0x00;
                serialPort.Write(ZeroByte,0,1);
                writingSuccessfully = true;
            }
        }

        private void WriteUintAsTwoBytes(uint ToWrite)
        {
            byte[] sampleValBytes = BitConverter.GetBytes(ToWrite);
            if (sampleValBytes[0] == 0xFF)
            {
                sampleValBytes[0] = 0xFE;
            }
            serialPort.Write(sampleValBytes, 0, 1);
            serialPort.Write(sampleValBytes, 1, 1);
        }

        private void tmrSampleOut_Tick(object sender, EventArgs e)
        {
            //output samples as needed
            int numSinceLastTick = (int)(GetShouldBeSampleNumber() - outSampleCount);

            for (int sampNum = 0; sampNum < numSinceLastTick; sampNum++)
            {
                outSampleCount++;
                Debug.Print(outSampleCount.ToString());
                try
                {
                    SendOutPacket(outSampleCount);
                }
                catch (TimeoutException ex)
                {
                    tmrSampleOut.Enabled = false;
                    writingSuccessfully = false;
                    MessageBox.Show("Canot output samples - need to attach something to receive.");
                }
                catch (InvalidOperationException ex)
                {
                    if (writingSuccessfully)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                if ((outSampleCount == updNumSamplesToWrite.Value) || (writingSuccessfully == false))
                {
                    tmrSampleOut.Enabled = false;
                    stopwatch.Stop();
                    lblElapsed.Text = stopwatch.ElapsedMilliseconds.ToString();
                    btnSimulate.Enabled = true;
                    ChangeToStoppedState();
                    break;
                }
            }
        }

        private void frmSimulator_Load(object sender, EventArgs e)
        {
            //restore form
            if (Properties.Settings.Default.frmMainLocation == new Point(0, 0))
            {
                // first start
                FormHelpers.Centerform(this);
            }
            else
            {
                // we don't want a minimized window at startup
                this.Location = Properties.Settings.Default.frmMainLocation;
                this.Size = Properties.Settings.Default.frmMainSize;
            }
            this.WindowState = FormWindowState.Normal;

            //get com ports
            var ports = SerialPort.GetPortNames();
            cmbPort.DataSource = ports;
            if (ports.Contains<String>(Properties.Settings.Default.SelectedPort))
            {
                cmbPort.Text = Properties.Settings.Default.SelectedPort;
            }

            ChangeToNeedEDFFileState();
        }

        private void frmSimulator_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.frmMainLocation = this.Location;
            Properties.Settings.Default.frmMainSize = this.Size;
            Properties.Settings.Default.SelectedPort = cmbPort.Text;
            Properties.Settings.Default.Save();
        }

        private void frmSimulator_Move(object sender, EventArgs e)
        {
           
        }
        private void ChangeToStoppedState()
        {
            tmrSampleOut.Enabled = false;
            tmrWriteStatus.Enabled = false;
            this.btnSimulate.Enabled = true;
            this.btnStop.Enabled = false;
            this.cmbPort.Enabled = true;
            lblStatus.Text = "Stopped";
            lblStatus.BackColor = Color.White;
            this.serialPort.Close();
        }

        private void ChangeToNeedEDFFileState()
        {
            tmrSampleOut.Enabled = false;
            tmrWriteStatus.Enabled = false;
            this.btnSimulate.Enabled = false;
            this.btnStop.Enabled = false;
            this.cmbPort.Enabled = true;
            lblStatus.Text = "Select EDF File";
            lblStatus.BackColor = Color.White;
            this.serialPort.Close();
        }

        private void ChangeToAcquiringState()
        {
            this.btnSimulate.Enabled = false;
            this.btnStop.Enabled = true;
            this.cmbPort.Enabled = false;
            lblElapsed.Text = String.Empty;
            lblStatus.Text = "Starting...";
            prevPacketNum = 0;
            outSampleCount = 0;
            tmrSampleOut.Interval = (int)(secsPerSample * 1000);
            tmrSampleOut.Enabled = true;
            tmrWriteStatus.Enabled = true;
        }

        private void UpdateStatusMessage()
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Sorry, not implemented yet.");
            ChangeToStoppedState();
        }

        private void tmrWriteStatus_Tick(object sender, EventArgs e)
        {
            if (writingSuccessfully)
            {
                lblStatus.Text = "Writing Samples";
                lblStatus.BackColor = Color.LightGreen;
            }
            else
            {
                lblStatus.Text = "No One Receiving...";
                lblStatus.BackColor = Color.Yellow;
            }
        }
    }
}
