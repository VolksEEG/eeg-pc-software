
namespace Simple_Packet_Receiver
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnAcquire = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPort = new System.Windows.Forms.ComboBox();
            this.tmrUpdateDisplay = new System.Windows.Forms.Timer(this.components);
            this.btnCopyClipboard = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnClearDisplay = new System.Windows.Forms.Button();
            this.updBufferLength = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.updBufferLength)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAcquire
            // 
            this.btnAcquire.BackColor = System.Drawing.SystemColors.Control;
            this.btnAcquire.Enabled = false;
            this.btnAcquire.Location = new System.Drawing.Point(12, 22);
            this.btnAcquire.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAcquire.Name = "btnAcquire";
            this.btnAcquire.Size = new System.Drawing.Size(130, 48);
            this.btnAcquire.TabIndex = 0;
            this.btnAcquire.Text = "Acquire";
            this.btnAcquire.UseVisualStyleBackColor = false;
            this.btnAcquire.Click += new System.EventHandler(this.BtnAcquire_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(341, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 30);
            this.label1.TabIndex = 2;
            this.label1.Text = "Port";
            // 
            // cmbPort
            // 
            this.cmbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPort.FormattingEnabled = true;
            this.cmbPort.Location = new System.Drawing.Point(395, 28);
            this.cmbPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbPort.Name = "cmbPort";
            this.cmbPort.Size = new System.Drawing.Size(165, 38);
            this.cmbPort.TabIndex = 3;
            this.cmbPort.SelectedIndexChanged += new System.EventHandler(this.CmbPort_SelectedIndexChanged);
            // 
            // tmrUpdateDisplay
            // 
            this.tmrUpdateDisplay.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // btnCopyClipboard
            // 
            this.btnCopyClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopyClipboard.Location = new System.Drawing.Point(195, 492);
            this.btnCopyClipboard.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCopyClipboard.Name = "btnCopyClipboard";
            this.btnCopyClipboard.Size = new System.Drawing.Size(194, 50);
            this.btnCopyClipboard.TabIndex = 4;
            this.btnCopyClipboard.Text = "Copy to Clipboard";
            this.btnCopyClipboard.UseVisualStyleBackColor = true;
            this.btnCopyClipboard.Click += new System.EventHandler(this.BtnCopyClipboard_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.BackColor = System.Drawing.Color.Black;
            this.txtOutput.ForeColor = System.Drawing.Color.LimeGreen;
            this.txtOutput.Location = new System.Drawing.Point(12, 84);
            this.txtOutput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(1576, 396);
            this.txtOutput.TabIndex = 5;
            this.txtOutput.Text = "";
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.SystemColors.Control;
            this.btnPause.Enabled = false;
            this.btnPause.Location = new System.Drawing.Point(149, 22);
            this.btnPause.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(130, 48);
            this.btnPause.TabIndex = 6;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.BtnPause_Click);
            // 
            // btnClearDisplay
            // 
            this.btnClearDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearDisplay.Location = new System.Drawing.Point(12, 492);
            this.btnClearDisplay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClearDisplay.Name = "btnClearDisplay";
            this.btnClearDisplay.Size = new System.Drawing.Size(177, 50);
            this.btnClearDisplay.TabIndex = 7;
            this.btnClearDisplay.Text = "Clear Display";
            this.btnClearDisplay.UseVisualStyleBackColor = true;
            this.btnClearDisplay.Click += new System.EventHandler(this.BtnClearDisplay_Click);
            // 
            // updBufferLength
            // 
            this.updBufferLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.updBufferLength.Location = new System.Drawing.Point(586, 498);
            this.updBufferLength.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.updBufferLength.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.updBufferLength.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.updBufferLength.Name = "updBufferLength";
            this.updBufferLength.Size = new System.Drawing.Size(142, 35);
            this.updBufferLength.TabIndex = 8;
            this.updBufferLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.updBufferLength.ThousandsSeparator = true;
            this.updBufferLength.UpDownAlign = System.Windows.Forms.LeftRightAlignment.Left;
            this.updBufferLength.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(473, 502);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 30);
            this.label2.TabIndex = 9;
            this.label2.Text = "Buffer Size";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(735, 500);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 30);
            this.label3.TabIndex = 10;
            this.label3.Text = "(# lines)";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1599, 554);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.updBufferLength);
            this.Controls.Add(this.btnClearDisplay);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnCopyClipboard);
            this.Controls.Add(this.cmbPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAcquire);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FrmMain";
            this.Text = "SImple Packet Receiver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.updBufferLength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAcquire;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Timer tmrUpdateDisplay;
        private System.Windows.Forms.Button btnCopyClipboard;
        private System.Windows.Forms.RichTextBox txtOutput;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnClearDisplay;
        private System.Windows.Forms.NumericUpDown updBufferLength;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

