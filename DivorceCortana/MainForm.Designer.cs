namespace DivorceCortana
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.btnSystemAppOwnership = new System.Windows.Forms.Button();
			this.btnFind = new System.Windows.Forms.Button();
			this.txtFind = new System.Windows.Forms.TextBox();
			this.textBox1 = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// btnSystemAppOwnership
			// 
			this.btnSystemAppOwnership.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSystemAppOwnership.Location = new System.Drawing.Point(357, 9);
			this.btnSystemAppOwnership.Name = "btnSystemAppOwnership";
			this.btnSystemAppOwnership.Size = new System.Drawing.Size(167, 23);
			this.btnSystemAppOwnership.TabIndex = 7;
			this.btnSystemAppOwnership.Text = "Give SystemApp Ownership";
			this.btnSystemAppOwnership.UseVisualStyleBackColor = true;
			// 
			// btnFind
			// 
			this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFind.Location = new System.Drawing.Point(721, 11);
			this.btnFind.Name = "btnFind";
			this.btnFind.Size = new System.Drawing.Size(39, 23);
			this.btnFind.TabIndex = 6;
			this.btnFind.Text = "Find";
			this.btnFind.UseVisualStyleBackColor = true;
			// 
			// txtFind
			// 
			this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.txtFind.Location = new System.Drawing.Point(555, 12);
			this.txtFind.Name = "txtFind";
			this.txtFind.Size = new System.Drawing.Size(163, 20);
			this.txtFind.TabIndex = 5;
			// 
			// textBox1
			// 
			this.textBox1.DetectUrls = false;
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Location = new System.Drawing.Point(0, 0);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(785, 483);
			this.textBox1.TabIndex = 4;
			this.textBox1.Text = "";
			this.textBox1.WordWrap = false;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(785, 483);
			this.Controls.Add(this.btnSystemAppOwnership);
			this.Controls.Add(this.btnFind);
			this.Controls.Add(this.txtFind);
			this.Controls.Add(this.textBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "Main Form";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnSystemAppOwnership;
		private System.Windows.Forms.Button btnFind;
		private System.Windows.Forms.TextBox txtFind;
		private System.Windows.Forms.RichTextBox textBox1;


	}
}

