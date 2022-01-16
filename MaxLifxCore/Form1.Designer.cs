namespace MaxLifxCore
{
    partial class Form1
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
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.lumPanel = new System.Windows.Forms.Panel();
            this.button5 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button6 = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel6 = new System.Windows.Forms.LinkLabel();
            this.linkLabel7 = new System.Windows.Forms.LinkLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabel13 = new System.Windows.Forms.LinkLabel();
            this.linkLabel11 = new System.Windows.Forms.LinkLabel();
            this.linkLabel10 = new System.Windows.Forms.LinkLabel();
            this.linkLabel9 = new System.Windows.Forms.LinkLabel();
            this.linkLabel8 = new System.Windows.Forms.LinkLabel();
            this.cbM = new System.Windows.Forms.CheckBox();
            this.cbA = new System.Windows.Forms.CheckBox();
            this.cbC = new System.Windows.Forms.CheckBox();
            this.cbL = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(370, 82);
            this.button2.TabIndex = 1;
            this.button2.Text = "Load Diagram";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(388, 10);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(370, 127);
            this.button3.TabIndex = 2;
            this.button3.Text = "Save Diagram";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 168);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Web UI Port";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(144, 165);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(150, 31);
            this.tbPort.TabIndex = 4;
            this.tbPort.Text = "8080";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(300, 161);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(375, 38);
            this.button4.TabIndex = 5;
            this.button4.Text = "Launch Web UI";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button3_Click);
            // 
            // lumPanel
            // 
            this.lumPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lumPanel.AutoScroll = true;
            this.lumPanel.Location = new System.Drawing.Point(12, 294);
            this.lumPanel.Name = "lumPanel";
            this.lumPanel.Size = new System.Drawing.Size(1247, 944);
            this.lumPanel.TabIndex = 8;
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.Location = new System.Drawing.Point(1147, 218);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(112, 34);
            this.button5.TabIndex = 9;
            this.button5.Text = "Add Device";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button4_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.Location = new System.Drawing.Point(12, 260);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1247, 978);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(12, 96);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(370, 41);
            this.button6.TabIndex = 11;
            this.button6.Text = "Load and Remap Diagram";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel1.Location = new System.Drawing.Point(9, 24);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(245, 19);
            this.linkLabel1.TabIndex = 12;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://github.com/DarthAffe/RGB.NET";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel2.Location = new System.Drawing.Point(9, 43);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(344, 19);
            this.linkLabel2.TabIndex = 13;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "https://github.com/jasonpang/desktop-duplication-net";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel3.Location = new System.Drawing.Point(9, 62);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(254, 19);
            this.linkLabel3.TabIndex = 14;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "https://github.com/kenkendk/ceenhttpd";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel4.Location = new System.Drawing.Point(9, 100);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(274, 19);
            this.linkLabel4.TabIndex = 15;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "https://github.com/KSemenenko/ColorThief";
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel5
            // 
            this.linkLabel5.AutoSize = true;
            this.linkLabel5.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel5.Location = new System.Drawing.Point(276, 62);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Size = new System.Drawing.Size(219, 19);
            this.linkLabel5.TabIndex = 16;
            this.linkLabel5.TabStop = true;
            this.linkLabel5.Text = "https://numerics.mathdotnet.com/";
            // 
            // linkLabel6
            // 
            this.linkLabel6.AutoSize = true;
            this.linkLabel6.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel6.Location = new System.Drawing.Point(274, 24);
            this.linkLabel6.Name = "linkLabel6";
            this.linkLabel6.Size = new System.Drawing.Size(221, 19);
            this.linkLabel6.TabIndex = 17;
            this.linkLabel6.TabStop = true;
            this.linkLabel6.Text = "https://github.com/naudio/NAudio";
            this.linkLabel6.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel7
            // 
            this.linkLabel7.AutoSize = true;
            this.linkLabel7.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel7.Location = new System.Drawing.Point(9, 119);
            this.linkLabel7.Name = "linkLabel7";
            this.linkLabel7.Size = new System.Drawing.Size(301, 19);
            this.linkLabel7.TabIndex = 18;
            this.linkLabel7.TabStop = true;
            this.linkLabel7.Text = "https://github.com/WardBenjamin/SimplexNoise";
            this.linkLabel7.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.linkLabel13);
            this.groupBox1.Controls.Add(this.linkLabel11);
            this.groupBox1.Controls.Add(this.linkLabel10);
            this.groupBox1.Controls.Add(this.linkLabel9);
            this.groupBox1.Controls.Add(this.linkLabel8);
            this.groupBox1.Controls.Add(this.linkLabel7);
            this.groupBox1.Controls.Add(this.linkLabel6);
            this.groupBox1.Controls.Add(this.linkLabel5);
            this.groupBox1.Controls.Add(this.linkLabel4);
            this.groupBox1.Controls.Add(this.linkLabel3);
            this.groupBox1.Controls.Add(this.linkLabel2);
            this.groupBox1.Controls.Add(this.linkLabel1);
            this.groupBox1.Location = new System.Drawing.Point(764, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(502, 183);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MaxLights is built on the hard work of:";
            // 
            // linkLabel13
            // 
            this.linkLabel13.AutoSize = true;
            this.linkLabel13.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel13.Location = new System.Drawing.Point(9, 81);
            this.linkLabel13.Name = "linkLabel13";
            this.linkLabel13.Size = new System.Drawing.Size(265, 19);
            this.linkLabel13.TabIndex = 24;
            this.linkLabel13.TabStop = true;
            this.linkLabel13.Text = "https://github.com/openhardwaremonitor";
            this.linkLabel13.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel11
            // 
            this.linkLabel11.AutoSize = true;
            this.linkLabel11.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel11.Location = new System.Drawing.Point(11, 138);
            this.linkLabel11.Name = "linkLabel11";
            this.linkLabel11.Size = new System.Drawing.Size(272, 19);
            this.linkLabel11.TabIndex = 22;
            this.linkLabel11.TabStop = true;
            this.linkLabel11.Text = "https://github.com/radek-k/FFMediaToolkit";
            this.linkLabel11.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel10
            // 
            this.linkLabel10.AutoSize = true;
            this.linkLabel10.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel10.Location = new System.Drawing.Point(381, 81);
            this.linkLabel10.Name = "linkLabel10";
            this.linkLabel10.Size = new System.Drawing.Size(114, 19);
            this.linkLabel10.TabIndex = 21;
            this.linkLabel10.TabStop = true;
            this.linkLabel10.Text = "https://vuejs.org/";
            this.linkLabel10.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel9
            // 
            this.linkLabel9.AutoSize = true;
            this.linkLabel9.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel9.Location = new System.Drawing.Point(376, 43);
            this.linkLabel9.Name = "linkLabel9";
            this.linkLabel9.Size = new System.Drawing.Size(119, 19);
            this.linkLabel9.TabIndex = 20;
            this.linkLabel9.TabStop = true;
            this.linkLabel9.Text = "https://rete.js.org/";
            this.linkLabel9.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel8
            // 
            this.linkLabel8.AutoSize = true;
            this.linkLabel8.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.linkLabel8.Location = new System.Drawing.Point(9, 157);
            this.linkLabel8.Name = "linkLabel8";
            this.linkLabel8.Size = new System.Drawing.Size(263, 19);
            this.linkLabel8.TabIndex = 19;
            this.linkLabel8.TabStop = true;
            this.linkLabel8.Text = "https://github.com/SixLabors/ImageSharp";
            this.linkLabel8.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // cbM
            // 
            this.cbM.AutoSize = true;
            this.cbM.Location = new System.Drawing.Point(429, 222);
            this.cbM.Name = "cbM";
            this.cbM.Size = new System.Drawing.Size(69, 29);
            this.cbM.TabIndex = 20;
            this.cbM.Text = "MSI";
            this.cbM.UseVisualStyleBackColor = true;
            this.cbM.CheckedChanged += new System.EventHandler(this.cbM_CheckedChanged);
            // 
            // cbA
            // 
            this.cbA.AutoSize = true;
            this.cbA.Location = new System.Drawing.Point(504, 222);
            this.cbA.Name = "cbA";
            this.cbA.Size = new System.Drawing.Size(82, 29);
            this.cbA.TabIndex = 21;
            this.cbA.Text = "ASUS";
            this.cbA.UseVisualStyleBackColor = true;
            this.cbA.CheckedChanged += new System.EventHandler(this.cbA_CheckedChanged);
            // 
            // cbC
            // 
            this.cbC.AutoSize = true;
            this.cbC.Location = new System.Drawing.Point(592, 222);
            this.cbC.Name = "cbC";
            this.cbC.Size = new System.Drawing.Size(93, 29);
            this.cbC.TabIndex = 22;
            this.cbC.Text = "Corsair";
            this.cbC.UseVisualStyleBackColor = true;
            this.cbC.CheckedChanged += new System.EventHandler(this.cbC_CheckedChanged);
            // 
            // cbL
            // 
            this.cbL.AutoSize = true;
            this.cbL.Location = new System.Drawing.Point(691, 222);
            this.cbL.Name = "cbL";
            this.cbL.Size = new System.Drawing.Size(105, 29);
            this.cbL.TabIndex = 23;
            this.cbL.Text = "Logitech";
            this.cbL.UseVisualStyleBackColor = true;
            this.cbL.CheckedChanged += new System.EventHandler(this.cbL_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 223);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(411, 25);
            this.label2.TabIndex = 24;
            this.label2.Text = "Enable PC Hardware (requires restart after change):";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(872, 201);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 34);
            this.button1.TabIndex = 25;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1271, 1250);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbL);
            this.Controls.Add(this.cbC);
            this.Controls.Add(this.cbA);
            this.Controls.Add(this.cbM);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.lumPanel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.tbPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Name = "Form1";
            this.Text = "MaxLights";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Panel lumPanel;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.LinkLabel linkLabel7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.LinkLabel linkLabel13;
        private System.Windows.Forms.LinkLabel linkLabel11;
        private System.Windows.Forms.LinkLabel linkLabel10;
        private System.Windows.Forms.LinkLabel linkLabel9;
        private System.Windows.Forms.LinkLabel linkLabel8;
        private System.Windows.Forms.CheckBox cbM;
        private System.Windows.Forms.CheckBox cbA;
        private System.Windows.Forms.CheckBox cbC;
        private System.Windows.Forms.CheckBox cbL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
    }
}
