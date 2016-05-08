﻿namespace BizHawk.Client.EmuHawk
{
	partial class SoundConfig
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Cancel = new System.Windows.Forms.Button();
			this.OK = new System.Windows.Forms.Button();
			this.SoundOnCheckBox = new System.Windows.Forms.CheckBox();
			this.MuteFrameAdvance = new System.Windows.Forms.CheckBox();
			this.SoundVolGroup = new System.Windows.Forms.GroupBox();
			this.SoundVolBar = new System.Windows.Forms.TrackBar();
			this.SoundVolNumeric = new System.Windows.Forms.NumericUpDown();
			this.listBoxSoundDevices = new System.Windows.Forms.ListBox();
			this.SoundDeviceLabel = new System.Windows.Forms.Label();
			this.BufferSizeLabel = new System.Windows.Forms.Label();
			this.BufferSizeNumeric = new System.Windows.Forms.NumericUpDown();
			this.BufferSizeUnitsLabel = new System.Windows.Forms.Label();
			this.grpOutputMethod = new System.Windows.Forms.GroupBox();
			this.rbOutputMethodXAudio2 = new System.Windows.Forms.RadioButton();
			this.rbOutputMethodDirectSound = new System.Windows.Forms.RadioButton();
			this.rbOutputMethodOpenAL = new System.Windows.Forms.RadioButton();
			this.SoundVolGroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.SoundVolBar)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SoundVolNumeric)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.BufferSizeNumeric)).BeginInit();
			this.grpOutputMethod.SuspendLayout();
			this.SuspendLayout();
			// 
			// Cancel
			// 
			this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel.Location = new System.Drawing.Point(317, 244);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(75, 23);
			this.Cancel.TabIndex = 1;
			this.Cancel.Text = "&Cancel";
			this.Cancel.UseVisualStyleBackColor = true;
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// OK
			// 
			this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OK.Location = new System.Drawing.Point(236, 244);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(75, 23);
			this.OK.TabIndex = 0;
			this.OK.Text = "&OK";
			this.OK.UseVisualStyleBackColor = true;
			this.OK.Click += new System.EventHandler(this.OK_Click);
			// 
			// SoundOnCheckBox
			// 
			this.SoundOnCheckBox.AutoSize = true;
			this.SoundOnCheckBox.Location = new System.Drawing.Point(108, 31);
			this.SoundOnCheckBox.Name = "SoundOnCheckBox";
			this.SoundOnCheckBox.Size = new System.Drawing.Size(74, 17);
			this.SoundOnCheckBox.TabIndex = 3;
			this.SoundOnCheckBox.Text = "Sound On";
			this.SoundOnCheckBox.UseVisualStyleBackColor = true;
			this.SoundOnCheckBox.CheckedChanged += new System.EventHandler(this.SoundOnCheckBox_CheckedChanged);
			// 
			// MuteFrameAdvance
			// 
			this.MuteFrameAdvance.AutoSize = true;
			this.MuteFrameAdvance.Location = new System.Drawing.Point(108, 54);
			this.MuteFrameAdvance.Name = "MuteFrameAdvance";
			this.MuteFrameAdvance.Size = new System.Drawing.Size(128, 17);
			this.MuteFrameAdvance.TabIndex = 4;
			this.MuteFrameAdvance.Text = "Mute Frame Advance";
			this.MuteFrameAdvance.UseVisualStyleBackColor = true;
			// 
			// SoundVolGroup
			// 
			this.SoundVolGroup.Controls.Add(this.SoundVolBar);
			this.SoundVolGroup.Controls.Add(this.SoundVolNumeric);
			this.SoundVolGroup.Location = new System.Drawing.Point(12, 12);
			this.SoundVolGroup.Name = "SoundVolGroup";
			this.SoundVolGroup.Size = new System.Drawing.Size(90, 219);
			this.SoundVolGroup.TabIndex = 2;
			this.SoundVolGroup.TabStop = false;
			this.SoundVolGroup.Text = "Volume";
			// 
			// SoundVolBar
			// 
			this.SoundVolBar.LargeChange = 10;
			this.SoundVolBar.Location = new System.Drawing.Point(23, 23);
			this.SoundVolBar.Maximum = 100;
			this.SoundVolBar.Name = "SoundVolBar";
			this.SoundVolBar.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.SoundVolBar.Size = new System.Drawing.Size(45, 164);
			this.SoundVolBar.TabIndex = 0;
			this.SoundVolBar.TickFrequency = 10;
			this.SoundVolBar.Scroll += new System.EventHandler(this.trackBar1_Scroll);
			// 
			// SoundVolNumeric
			// 
			this.SoundVolNumeric.Location = new System.Drawing.Point(16, 190);
			this.SoundVolNumeric.Name = "SoundVolNumeric";
			this.SoundVolNumeric.Size = new System.Drawing.Size(59, 20);
			this.SoundVolNumeric.TabIndex = 1;
			this.SoundVolNumeric.ValueChanged += new System.EventHandler(this.SoundVolNumeric_ValueChanged);
			// 
			// listBoxSoundDevices
			// 
			this.listBoxSoundDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxSoundDevices.FormattingEnabled = true;
			this.listBoxSoundDevices.Location = new System.Drawing.Point(108, 108);
			this.listBoxSoundDevices.Name = "listBoxSoundDevices";
			this.listBoxSoundDevices.Size = new System.Drawing.Size(284, 95);
			this.listBoxSoundDevices.TabIndex = 7;
			// 
			// SoundDeviceLabel
			// 
			this.SoundDeviceLabel.AutoSize = true;
			this.SoundDeviceLabel.Location = new System.Drawing.Point(105, 92);
			this.SoundDeviceLabel.Name = "SoundDeviceLabel";
			this.SoundDeviceLabel.Size = new System.Drawing.Size(78, 13);
			this.SoundDeviceLabel.TabIndex = 6;
			this.SoundDeviceLabel.Text = "Sound Device:";
			// 
			// BufferSizeLabel
			// 
			this.BufferSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.BufferSizeLabel.AutoSize = true;
			this.BufferSizeLabel.Location = new System.Drawing.Point(105, 213);
			this.BufferSizeLabel.Name = "BufferSizeLabel";
			this.BufferSizeLabel.Size = new System.Drawing.Size(61, 13);
			this.BufferSizeLabel.TabIndex = 8;
			this.BufferSizeLabel.Text = "Buffer Size:";
			// 
			// BufferSizeNumeric
			// 
			this.BufferSizeNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.BufferSizeNumeric.Location = new System.Drawing.Point(172, 211);
			this.BufferSizeNumeric.Maximum = new decimal(new int[] {
            250,
            0,
            0,
            0});
			this.BufferSizeNumeric.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
			this.BufferSizeNumeric.Name = "BufferSizeNumeric";
			this.BufferSizeNumeric.Size = new System.Drawing.Size(59, 20);
			this.BufferSizeNumeric.TabIndex = 9;
			this.BufferSizeNumeric.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// BufferSizeUnitsLabel
			// 
			this.BufferSizeUnitsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.BufferSizeUnitsLabel.AutoSize = true;
			this.BufferSizeUnitsLabel.Location = new System.Drawing.Point(237, 213);
			this.BufferSizeUnitsLabel.Name = "BufferSizeUnitsLabel";
			this.BufferSizeUnitsLabel.Size = new System.Drawing.Size(63, 13);
			this.BufferSizeUnitsLabel.TabIndex = 10;
			this.BufferSizeUnitsLabel.Text = "milliseconds";
			// 
			// grpOutputMethod
			// 
			this.grpOutputMethod.Controls.Add(this.rbOutputMethodOpenAL);
			this.grpOutputMethod.Controls.Add(this.rbOutputMethodXAudio2);
			this.grpOutputMethod.Controls.Add(this.rbOutputMethodDirectSound);
			this.grpOutputMethod.Location = new System.Drawing.Point(292, 12);
			this.grpOutputMethod.Name = "grpOutputMethod";
			this.grpOutputMethod.Size = new System.Drawing.Size(100, 90);
			this.grpOutputMethod.TabIndex = 5;
			this.grpOutputMethod.TabStop = false;
			this.grpOutputMethod.Text = "Output Method";
			// 
			// rbOutputMethodXAudio2
			// 
			this.rbOutputMethodXAudio2.AutoSize = true;
			this.rbOutputMethodXAudio2.Location = new System.Drawing.Point(6, 42);
			this.rbOutputMethodXAudio2.Name = "rbOutputMethodXAudio2";
			this.rbOutputMethodXAudio2.Size = new System.Drawing.Size(65, 17);
			this.rbOutputMethodXAudio2.TabIndex = 1;
			this.rbOutputMethodXAudio2.TabStop = true;
			this.rbOutputMethodXAudio2.Text = "XAudio2";
			this.rbOutputMethodXAudio2.UseVisualStyleBackColor = true;
			this.rbOutputMethodXAudio2.CheckedChanged += new System.EventHandler(this.OutputMethodRadioButtons_CheckedChanged);
			// 
			// rbOutputMethodDirectSound
			// 
			this.rbOutputMethodDirectSound.AutoSize = true;
			this.rbOutputMethodDirectSound.Location = new System.Drawing.Point(6, 19);
			this.rbOutputMethodDirectSound.Name = "rbOutputMethodDirectSound";
			this.rbOutputMethodDirectSound.Size = new System.Drawing.Size(84, 17);
			this.rbOutputMethodDirectSound.TabIndex = 0;
			this.rbOutputMethodDirectSound.TabStop = true;
			this.rbOutputMethodDirectSound.Text = "DirectSound";
			this.rbOutputMethodDirectSound.UseVisualStyleBackColor = true;
			this.rbOutputMethodDirectSound.CheckedChanged += new System.EventHandler(this.OutputMethodRadioButtons_CheckedChanged);
			// 
			// rbOutputMethodOpenAL
			// 
			this.rbOutputMethodOpenAL.AutoSize = true;
			this.rbOutputMethodOpenAL.Location = new System.Drawing.Point(6, 65);
			this.rbOutputMethodOpenAL.Name = "rbOutputMethodOpenAL";
			this.rbOutputMethodOpenAL.Size = new System.Drawing.Size(64, 17);
			this.rbOutputMethodOpenAL.TabIndex = 2;
			this.rbOutputMethodOpenAL.TabStop = true;
			this.rbOutputMethodOpenAL.Text = "OpenAL";
			this.rbOutputMethodOpenAL.UseVisualStyleBackColor = true;
			this.rbOutputMethodOpenAL.CheckedChanged += new System.EventHandler(this.OutputMethodRadioButtons_CheckedChanged);
			// 
			// SoundConfig
			// 
			this.AcceptButton = this.OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Cancel;
			this.ClientSize = new System.Drawing.Size(404, 279);
			this.Controls.Add(this.grpOutputMethod);
			this.Controls.Add(this.BufferSizeUnitsLabel);
			this.Controls.Add(this.BufferSizeNumeric);
			this.Controls.Add(this.BufferSizeLabel);
			this.Controls.Add(this.SoundDeviceLabel);
			this.Controls.Add(this.listBoxSoundDevices);
			this.Controls.Add(this.SoundVolGroup);
			this.Controls.Add(this.MuteFrameAdvance);
			this.Controls.Add(this.SoundOnCheckBox);
			this.Controls.Add(this.OK);
			this.Controls.Add(this.Cancel);
			this.MinimumSize = new System.Drawing.Size(279, 259);
			this.Name = "SoundConfig";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Sound Configuration";
			this.Load += new System.EventHandler(this.SoundConfig_Load);
			this.SoundVolGroup.ResumeLayout(false);
			this.SoundVolGroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.SoundVolBar)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SoundVolNumeric)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.BufferSizeNumeric)).EndInit();
			this.grpOutputMethod.ResumeLayout(false);
			this.grpOutputMethod.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button Cancel;
		private System.Windows.Forms.Button OK;
		private System.Windows.Forms.CheckBox SoundOnCheckBox;
		private System.Windows.Forms.CheckBox MuteFrameAdvance;
		private System.Windows.Forms.GroupBox SoundVolGroup;
		private System.Windows.Forms.NumericUpDown SoundVolNumeric;
		private System.Windows.Forms.TrackBar SoundVolBar;
		private System.Windows.Forms.ListBox listBoxSoundDevices;
		private System.Windows.Forms.Label SoundDeviceLabel;
		private System.Windows.Forms.Label BufferSizeLabel;
		private System.Windows.Forms.NumericUpDown BufferSizeNumeric;
		private System.Windows.Forms.Label BufferSizeUnitsLabel;
		private System.Windows.Forms.GroupBox grpOutputMethod;
		private System.Windows.Forms.RadioButton rbOutputMethodXAudio2;
		private System.Windows.Forms.RadioButton rbOutputMethodDirectSound;
		private System.Windows.Forms.RadioButton rbOutputMethodOpenAL;
	}
}