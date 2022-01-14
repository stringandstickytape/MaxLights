
namespace MaxLifxCore
{
    partial class NewDevice
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
            this.button1 = new System.Windows.Forms.Button();
            this.tbIpAddress = new System.Windows.Forms.TextBox();
            this.cbDeviceType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numZones = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nNewDevicePort = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numZones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nNewDevicePort)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(282, 205);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(184, 40);
            this.button1.TabIndex = 5;
            this.button1.Text = "Add Device";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbIpAddress
            // 
            this.tbIpAddress.Location = new System.Drawing.Point(156, 86);
            this.tbIpAddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbIpAddress.Name = "tbIpAddress";
            this.tbIpAddress.Size = new System.Drawing.Size(310, 31);
            this.tbIpAddress.TabIndex = 3;
            // 
            // cbDeviceType
            // 
            this.cbDeviceType.FormattingEnabled = true;
            this.cbDeviceType.Items.AddRange(new object[] {
            "WLED"});
            this.cbDeviceType.Location = new System.Drawing.Point(156, 45);
            this.cbDeviceType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbDeviceType.Name = "cbDeviceType";
            this.cbDeviceType.Size = new System.Drawing.Size(310, 33);
            this.cbDeviceType.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "IP Address";
            // 
            // numZones
            // 
            this.numZones.Location = new System.Drawing.Point(156, 166);
            this.numZones.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numZones.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numZones.Name = "numZones";
            this.numZones.Size = new System.Drawing.Size(310, 31);
            this.numZones.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 168);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Zones/LEDs";
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(156, 6);
            this.tbName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(310, 31);
            this.tbName.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 25);
            this.label3.TabIndex = 7;
            this.label3.Text = "Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 25);
            this.label4.TabIndex = 8;
            this.label4.Text = "Device Type";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 127);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 25);
            this.label5.TabIndex = 9;
            this.label5.Text = "Port";
            // 
            // nNewDevicePort
            // 
            this.nNewDevicePort.Location = new System.Drawing.Point(157, 127);
            this.nNewDevicePort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.nNewDevicePort.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.nNewDevicePort.Name = "nNewDevicePort";
            this.nNewDevicePort.Size = new System.Drawing.Size(310, 31);
            this.nNewDevicePort.TabIndex = 10;
            this.nNewDevicePort.Value = new decimal(new int[] {
            22001,
            0,
            0,
            0});
            // 
            // NewDevice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 255);
            this.Controls.Add(this.nNewDevicePort);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numZones);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbDeviceType);
            this.Controls.Add(this.tbIpAddress);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "NewDevice";
            this.Text = "NewDevice";
            ((System.ComponentModel.ISupportInitialize)(this.numZones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nNewDevicePort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbIpAddress;
        private System.Windows.Forms.ComboBox cbDeviceType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numZones;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nNewDevicePort;
    }
}