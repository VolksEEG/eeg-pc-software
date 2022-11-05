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

namespace Simple_Packet_Receiver
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO.Ports;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using CircularBuffer;
    using SimplePacketLib;

    public partial class FrmMain : Form
    {
        private SerialPort serialPort = new SerialPort();
        private int numPacketsReceived = 0;
        private SimplePacketParser packetizer = new SimplePacketParser(21);
        private SimplePacket packetBuffer = new SimplePacket();
        private Stopwatch stopwatch = new Stopwatch();
        private string prevDispString = string.Empty;
        private object mylock = new object();
        private CircularBuffer<string> outLinesBuffer;
        private string[] outLines = new string[1000];

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmMain"/> class.
        /// </summary>
        public FrmMain()
        {
            this.InitializeComponent();
        }

        private delegate void SetTextCallback(string text);

        private void Form1_Load(object sender, EventArgs e)
        {
            // set form location and size
            if (Properties.Settings.Default.frmMainLocation == new Point(0, 0))
            {
                FormHelpers.Centerform(this);
            }
            else if ((Properties.Settings.Default.frmMainLocation.X
                + Properties.Settings.Default.frmMainSize.Width)
                >= Screen.PrimaryScreen.Bounds.Width)
            {
                FormHelpers.Centerform(this);
            }
            else
            {
                this.Location = Properties.Settings.Default.frmMainLocation;
                this.Size = Properties.Settings.Default.frmMainSize;
            }

            // register serial port event handler
            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(this.SerialReceivedHandler);

            // populate port dropdown with this computer's com ports
            var ports = SerialPort.GetPortNames();
            this.cmbPort.DataSource = ports;

            // if com port saved in configuration exists on computer, then set to that
            if (ports.Contains<string>(Properties.Settings.Default.SelectedPort))
            {
                this.cmbPort.Text = Properties.Settings.Default.SelectedPort;
            }
        }

        private void BtnAcquire_Click(object sender, EventArgs e)
        {
            try
            {
                this.serialPort.PortName = this.cmbPort.Text;
                Properties.Settings.Default.SelectedPort = this.cmbPort.Text;
                this.serialPort.BaudRate = 921600;
                this.serialPort.Parity = Parity.None;
                this.serialPort.DataBits = 8;
                this.serialPort.StopBits = StopBits.One;
                this.serialPort.Open();
                this.tmrUpdateDisplay.Enabled = true;
                this.stopwatch.Start();
                this.outLinesBuffer = new CircularBuffer<string>((int)this.updBufferLength.Value);
                this.ChangeToAcquiringState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SerialReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            for (int i = 0; i < sp.BytesToRead; i++)
            {
                byte b = Convert.ToByte(sp.ReadByte());
                bool newPacket = this.packetizer.AddByte(b, this.packetBuffer);
                if (newPacket)
                {
                    StringBuilder lineToPrint = new StringBuilder();
                    lineToPrint.Append(Environment.NewLine);
                    lineToPrint.Append(this.packetBuffer.StartFlag + "\t" + this.packetBuffer.Counter);
                    for (int j = 0; j < this.packetBuffer.ChannelSet.Length; j++)
                    {
                        lineToPrint.Append("\t" + this.packetBuffer.ChannelSet[j].ToString());
                    }

                    this.outLinesBuffer.PushBack(lineToPrint.ToString());
                    this.numPacketsReceived++;
                }
            }
        }

        private void SetText(string text)
        {
            this.txtOutput.AppendText(text);
            Application.DoEvents();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.DisplayPackets();
        }

        private void DisplayPackets()
        {
            lock (this.mylock)
            {
                this.txtOutput.Text = this.GetOutText();
                if (this.txtOutput.Text != this.prevDispString)
                {
                    this.txtOutput.SelectionStart = this.txtOutput.Text.Length;
                    this.txtOutput.ScrollToCaret();
                    this.prevDispString = this.txtOutput.Text;
                }
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.frmMainLocation = this.Location;
            Properties.Settings.Default.frmMainSize = this.Size;
            Properties.Settings.Default.SelectedPort = this.cmbPort.Text;
            Properties.Settings.Default.Save();
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            Properties.Settings.Default.frmMainSize = this.Size;
        }

        private void BtnCopyClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.txtOutput.Text);
        }

        private string GetOutText()
        {
            StringBuilder outText = new StringBuilder();
            this.outLines = this.outLinesBuffer.ToArray<string>();
            for (int i = 0; i < this.outLines.Length; i++)
            {
                if (this.outLines[i] != string.Empty)
                {
                    outText.Append(this.outLines[i]);
                }
            }

            return outText.ToString();
        }

        private void ChangeToPausedState()
        {
            this.btnAcquire.Enabled = true;
            this.btnPause.Enabled = false;
            this.btnClearDisplay.Enabled = true;
            this.btnCopyClipboard.Enabled = true;
            this.cmbPort.Enabled = true;
            this.updBufferLength.Enabled = true;
            this.serialPort.Close();
        }

        private void ChangeToAcquiringState()
        {
            this.btnAcquire.Enabled = false;
            this.btnPause.Enabled = true;
            this.btnClearDisplay.Enabled = true;
            this.btnCopyClipboard.Enabled = true;
            this.cmbPort.Enabled = false;
            this.updBufferLength.Enabled = false;
        }

        private void CmbPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ChangeToPausedState();
        }

        private void BtnClearDisplay_Click(object sender, EventArgs e)
        {
            this.outLinesBuffer.Clear();
            this.DisplayPackets();
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            this.ChangeToPausedState();
        }
    }
}
