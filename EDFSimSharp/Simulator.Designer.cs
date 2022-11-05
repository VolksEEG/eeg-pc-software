
namespace EDFSimSharp
{
    partial class Simulator
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
            this.btnFileSelect = new System.Windows.Forms.Button();
            this.btnSimulate = new System.Windows.Forms.Button();
            this.tmrSampleOut = new System.Windows.Forms.Timer(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblSamplesSet = new System.Windows.Forms.Label();
            this.updNumSamplesToWrite = new System.Windows.Forms.NumericUpDown();
            this.lblWhatever = new System.Windows.Forms.Label();
            this.lblElapsed = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbPort = new System.Windows.Forms.ComboBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.tmrWriteStatus = new System.Windows.Forms.Timer(this.components);
            this.lblSelectedFile = new System.Windows.Forms.Label();
            this.grpOutput = new System.Windows.Forms.GroupBox();
            this.rdoOutputFixed = new System.Windows.Forms.RadioButton();
            this.ckbLoop = new System.Windows.Forms.CheckBox();
            this.rdoOutputComplete = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.updNumSamplesToWrite)).BeginInit();
            this.grpOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFileSelect
            // 
            this.btnFileSelect.Location = new System.Drawing.Point(15, 82);
            this.btnFileSelect.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnFileSelect.Name = "btnFileSelect";
            this.btnFileSelect.Size = new System.Drawing.Size(182, 46);
            this.btnFileSelect.TabIndex = 1;
            this.btnFileSelect.Text = "Select File...";
            this.btnFileSelect.UseVisualStyleBackColor = true;
            this.btnFileSelect.Click += new System.EventHandler(this.btnFileSelect_Click);
            // 
            // btnSimulate
            // 
            this.btnSimulate.Enabled = false;
            this.btnSimulate.Location = new System.Drawing.Point(18, 302);
            this.btnSimulate.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.btnSimulate.Name = "btnSimulate";
            this.btnSimulate.Size = new System.Drawing.Size(130, 40);
            this.btnSimulate.TabIndex = 2;
            this.btnSimulate.Text = "Simulate";
            this.btnSimulate.UseVisualStyleBackColor = true;
            this.btnSimulate.Click += new System.EventHandler(this.btnSimulate_Click);
            // 
            // tmrSampleOut
            // 
            this.tmrSampleOut.Interval = 10;
            this.tmrSampleOut.Tick += new System.EventHandler(this.tmrSampleOut_Tick);
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.White;
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.Location = new System.Drawing.Point(313, 302);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(336, 40);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Idle";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSamplesSet
            // 
            this.lblSamplesSet.AutoSize = true;
            this.lblSamplesSet.Location = new System.Drawing.Point(208, 85);
            this.lblSamplesSet.Name = "lblSamplesSet";
            this.lblSamplesSet.Size = new System.Drawing.Size(88, 30);
            this.lblSamplesSet.TabIndex = 4;
            this.lblSamplesSet.Text = "samples";
            // 
            // updNumSamplesToWrite
            // 
            this.updNumSamplesToWrite.Location = new System.Drawing.Point(117, 83);
            this.updNumSamplesToWrite.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.updNumSamplesToWrite.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.updNumSamplesToWrite.Name = "updNumSamplesToWrite";
            this.updNumSamplesToWrite.Size = new System.Drawing.Size(87, 35);
            this.updNumSamplesToWrite.TabIndex = 5;
            this.updNumSamplesToWrite.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lblWhatever
            // 
            this.lblWhatever.AutoSize = true;
            this.lblWhatever.Location = new System.Drawing.Point(389, 198);
            this.lblWhatever.Name = "lblWhatever";
            this.lblWhatever.Size = new System.Drawing.Size(84, 30);
            this.lblWhatever.TabIndex = 6;
            this.lblWhatever.Text = "Elapsed";
            this.lblWhatever.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblElapsed
            // 
            this.lblElapsed.BackColor = System.Drawing.Color.White;
            this.lblElapsed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblElapsed.Location = new System.Drawing.Point(389, 229);
            this.lblElapsed.Name = "lblElapsed";
            this.lblElapsed.Size = new System.Drawing.Size(121, 44);
            this.lblElapsed.TabIndex = 7;
            this.lblElapsed.Text = "0.000";
            this.lblElapsed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 30);
            this.label3.TabIndex = 8;
            this.label3.Text = "Port";
            // 
            // cmbPort
            // 
            this.cmbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPort.FormattingEnabled = true;
            this.cmbPort.Location = new System.Drawing.Point(74, 20);
            this.cmbPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbPort.Name = "cmbPort";
            this.cmbPort.Size = new System.Drawing.Size(211, 38);
            this.cmbPort.TabIndex = 9;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(166, 302);
            this.btnStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(130, 40);
            this.btnStop.TabIndex = 10;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tmrWriteStatus
            // 
            this.tmrWriteStatus.Tick += new System.EventHandler(this.tmrWriteStatus_Tick);
            // 
            // lblSelectedFile
            // 
            this.lblSelectedFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelectedFile.BackColor = System.Drawing.SystemColors.Window;
            this.lblSelectedFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSelectedFile.Location = new System.Drawing.Point(207, 88);
            this.lblSelectedFile.Name = "lblSelectedFile";
            this.lblSelectedFile.Size = new System.Drawing.Size(470, 34);
            this.lblSelectedFile.TabIndex = 11;
            // 
            // grpOutput
            // 
            this.grpOutput.Controls.Add(this.rdoOutputFixed);
            this.grpOutput.Controls.Add(this.ckbLoop);
            this.grpOutput.Controls.Add(this.rdoOutputComplete);
            this.grpOutput.Controls.Add(this.updNumSamplesToWrite);
            this.grpOutput.Controls.Add(this.lblSamplesSet);
            this.grpOutput.Location = new System.Drawing.Point(17, 155);
            this.grpOutput.Name = "grpOutput";
            this.grpOutput.Size = new System.Drawing.Size(354, 140);
            this.grpOutput.TabIndex = 12;
            this.grpOutput.TabStop = false;
            this.grpOutput.Text = "Output";
            this.grpOutput.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // rdoOutputFixed
            // 
            this.rdoOutputFixed.AutoSize = true;
            this.rdoOutputFixed.Location = new System.Drawing.Point(25, 83);
            this.rdoOutputFixed.Name = "rdoOutputFixed";
            this.rdoOutputFixed.Size = new System.Drawing.Size(86, 34);
            this.rdoOutputFixed.TabIndex = 2;
            this.rdoOutputFixed.TabStop = true;
            this.rdoOutputFixed.Text = "Fixed";
            this.rdoOutputFixed.UseVisualStyleBackColor = true;
            this.rdoOutputFixed.CheckedChanged += new System.EventHandler(this.rdoOutputFixed_CheckedChanged);
            // 
            // ckbLoop
            // 
            this.ckbLoop.AutoSize = true;
            this.ckbLoop.Location = new System.Drawing.Point(208, 40);
            this.ckbLoop.Name = "ckbLoop";
            this.ckbLoop.Size = new System.Drawing.Size(122, 34);
            this.ckbLoop.TabIndex = 1;
            this.ckbLoop.Text = "Loop File";
            this.ckbLoop.UseVisualStyleBackColor = true;
            // 
            // rdoOutputComplete
            // 
            this.rdoOutputComplete.AutoSize = true;
            this.rdoOutputComplete.Location = new System.Drawing.Point(25, 39);
            this.rdoOutputComplete.Name = "rdoOutputComplete";
            this.rdoOutputComplete.Size = new System.Drawing.Size(164, 34);
            this.rdoOutputComplete.TabIndex = 0;
            this.rdoOutputComplete.TabStop = true;
            this.rdoOutputComplete.Text = "Complete File";
            this.rdoOutputComplete.UseVisualStyleBackColor = true;
            this.rdoOutputComplete.CheckedChanged += new System.EventHandler(this.rdoOutputComplete_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(516, 236);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 30);
            this.label1.TabIndex = 13;
            this.label1.Text = "sec";
            // 
            // Simulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 368);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grpOutput);
            this.Controls.Add(this.lblSelectedFile);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.cmbPort);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblElapsed);
            this.Controls.Add(this.lblWhatever);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSimulate);
            this.Controls.Add(this.btnFileSelect);
            this.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.MaximumSize = new System.Drawing.Size(1000, 432);
            this.MinimumSize = new System.Drawing.Size(694, 432);
            this.Name = "Simulator";
            this.Text = "EDF Simulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSimulator_FormClosing);
            this.Load += new System.EventHandler(this.frmSimulator_Load);
            this.Move += new System.EventHandler(this.frmSimulator_Move);
            ((System.ComponentModel.ISupportInitialize)(this.updNumSamplesToWrite)).EndInit();
            this.grpOutput.ResumeLayout(false);
            this.grpOutput.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnFileSelect;
        private System.Windows.Forms.Button btnSimulate;
        private System.Windows.Forms.Timer tmrSampleOut;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblSamplesSet;
        private System.Windows.Forms.NumericUpDown updNumSamplesToWrite;
        private System.Windows.Forms.Label lblWhatever;
        private System.Windows.Forms.Label lblElapsed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Timer tmrWriteStatus;
        private System.Windows.Forms.Label lblSelectedFile;
        private System.Windows.Forms.GroupBox grpOutput;
        private System.Windows.Forms.RadioButton rdoOutputFixed;
        private System.Windows.Forms.CheckBox ckbLoop;
        private System.Windows.Forms.RadioButton rdoOutputComplete;
        private System.Windows.Forms.Label label1;
    }
}

