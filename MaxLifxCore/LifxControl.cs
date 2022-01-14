using MaxLifxCoreBulbController.Controllers;
using MaxLifxCoreBulbController.Payload;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaxLights
{
    public partial class LifxControl : Form
    {
        private LifxDevice device;
        private MaxLifxCoreBulbController.Controllers.MaxLifxCoreBulbController controller;
        public LifxControl(LifxDevice l, MaxLifxCoreBulbController.Controllers.MaxLifxCoreBulbController c)
        {
            device = l;
            controller = c;
            
            InitializeComponent();
            this.Text = l.Label;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetLifxPowerState(false);
        }

        private void SetLifxPowerState(bool pVal)
        {
            var p = new SetPowerPayload(pVal);
            controller.SendPayloadToMacAddress(p, device.MacAddress, device.IpAddress);
            Thread.Sleep(1);
            controller.SendPayloadToMacAddress(p, device.MacAddress, device.IpAddress);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetLifxPowerState(true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var newPayload = new SetColourZonesPayload()
            {
                Brightness = (ushort)numericUpDown3.Value,
                Hue = (ushort)numericUpDown1.Value,
                Kelvin = (ushort)numericUpDown4.Value,
                Saturation = (ushort)numericUpDown2.Value,
                TransitionDuration = 1000,
                start_index = new byte[] { (byte)0 },
                end_index = new byte[] { (byte)(device.Zones-1) },
                apply = new byte[] { 1 }
            };

            controller.SendPayloadToMacAddress(newPayload, device.MacAddress, device.IpAddress);

        }
    }
}
