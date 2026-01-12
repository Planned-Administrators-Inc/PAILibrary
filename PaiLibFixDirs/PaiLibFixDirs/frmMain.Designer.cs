namespace PaiLibFixDirs
{
	partial class frmMain
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
			this.pbStatus = new System.Windows.Forms.ProgressBar();
			this.lblStatus = new System.Windows.Forms.Label();
			this.chkOverwrite = new System.Windows.Forms.CheckBox();
			this.btnSpeedRacer = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pbStatus
			// 
			this.pbStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pbStatus.Location = new System.Drawing.Point(0, 124);
			this.pbStatus.Name = "pbStatus";
			this.pbStatus.Size = new System.Drawing.Size(463, 23);
			this.pbStatus.TabIndex = 0;
			// 
			// lblStatus
			// 
			this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblStatus.Location = new System.Drawing.Point(0, 101);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(463, 23);
			this.lblStatus.TabIndex = 1;
			this.lblStatus.Text = "lblStatus";
			// 
			// chkOverwrite
			// 
			this.chkOverwrite.AutoSize = true;
			this.chkOverwrite.Location = new System.Drawing.Point(167, 54);
			this.chkOverwrite.Name = "chkOverwrite";
			this.chkOverwrite.Size = new System.Drawing.Size(98, 20);
			this.chkOverwrite.TabIndex = 3;
			this.chkOverwrite.Text = "Overwrite?";
			this.chkOverwrite.UseVisualStyleBackColor = true;
			// 
			// btnSpeedRacer
			// 
			this.btnSpeedRacer.AutoSize = true;
			this.btnSpeedRacer.Location = new System.Drawing.Point(167, 12);
			this.btnSpeedRacer.Name = "btnSpeedRacer";
			this.btnSpeedRacer.Size = new System.Drawing.Size(128, 26);
			this.btnSpeedRacer.TabIndex = 4;
			this.btnSpeedRacer.Text = "Go Speed Racer!";
			this.btnSpeedRacer.UseVisualStyleBackColor = true;
			this.btnSpeedRacer.Click += new System.EventHandler(this.btnSpeedRacer_Click);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(463, 147);
			this.Controls.Add(this.btnSpeedRacer);
			this.Controls.Add(this.chkOverwrite);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.pbStatus);
			this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmMain";
			this.Text = "PAILib Sep Dirs";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar pbStatus;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.CheckBox chkOverwrite;
		private System.Windows.Forms.Button btnSpeedRacer;
	}
}

