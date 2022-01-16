using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using MaxLifxCore;
using MaxLifxCore.Controls;
using MaxLifxCoreBulbController.Controllers;
using MaxLifxCoreBulbController.Payload;
using MaxLights;
using Microsoft.CSharp;
using Newtonsoft.Json;
using RGB.NET.Core;
using RGB.NET.Devices.Asus;
using RGB.NET.Devices.Corsair;
using RGB.NET.Devices.DMX;
using RGB.NET.Devices.DMX.E131;
using RGB.NET.Devices.Logitech;
using RGB.NET.Devices.Msi;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaxLifxCore
{
    public partial class Form1 : Form
    {
        public MaxLifxCoreBulbController.Controllers.MaxLifxCoreBulbController Controller = new MaxLifxCoreBulbController.Controllers.MaxLifxCoreBulbController();

        public AppController _appController;

        private SpectrumAnalyserEngine engine;

        public Bitmap bitmap { get; set; }

        public readonly decimal Version = 0.9m;
        public Form1()
        {
            try
            {
                CheckForNewVersion();
            }
            catch (Exception ex)
            {

            }

            InitializeComponent();
            bitmap = new Bitmap(pictureBox1.Width, 1);
            pictureBox1.Image = bitmap;
            
            _appController = new AppController(() => {
                if (_appController.AppSettings.Port == 0) _appController.AppSettings.Port = int.Parse(tbPort.Text);
                tbPort.Text = _appController.AppSettings.Port.ToString();
                cbA.Checked = _appController.AppSettings?.EnableASUS ?? false;
                cbL.Checked = _appController.AppSettings?.EnableLogitech ?? false;
                cbM.Checked = _appController.AppSettings?.EnableMSI ?? false;
                cbC.Checked = _appController.AppSettings?.EnableCorsair ?? false;
                
                return 1;
            });

            _appController.LoadSettings();

            _appController.Form = this;

            foreach (var luminaire in _appController.AppSettings.StaticWledDevices)
            {
                _appController.Luminaires.Add(luminaire);
            }

            foreach (var luminaire in _appController.AppSettings.StaticLifxDevices)
            {
                _appController.Luminaires.Add(luminaire);
            }

            InitRgbNet();

            Controller.SetupNetwork();

            _appController.Luminaires.CollectionChanged += listChanged;

            Controller.UdpDiscoveryAsyncListen(ref _appController.Luminaires, ref _appController.LockObj, () =>
            {
                this.listChanged(null, null);
                return 0;
            });


            var serverCore = new Webserver.WebserverCore(ref _appController);
            serverCore.InitWebserver();

            engine = new SpectrumAnalyserEngine(ref _appController);
            engine.StartCapture();

            _appController.SpectrumAnalyserEngine = engine;

            _appController.Run(ref Controller);

            UpdateUIBulbCount();
        }

        private void CheckForNewVersion()
        {
            string sURL;
            sURL = @"https://api.github.com/repos/stringandstickytape/MaxLights/releases";
            string response;

            var webRequest = WebRequest.Create(sURL) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "YourAppName";

            decimal maxVersion = -1;

            using (var responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                response = responseReader.ReadToEnd();

            dynamic data = JsonConvert.DeserializeObject<dynamic>(response);

            var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            dynamic releaseDetails = null;

            foreach (var x in data)
            {
                string release = x.tag_name;

                var thisVersion = decimal.Parse(release, NumberStyles.Any, ci);
                if (maxVersion < thisVersion)
                {
                    maxVersion = thisVersion;
                    releaseDetails = x;
                }
            }

            if (Version < maxVersion)
            {
                var dialogResult =
                    MessageBox.Show(
                        "Update found. Quit MaxLights and browse to GitHub?\r\n\r\nv" +
                        maxVersion +
                        ":\r\n" + releaseDetails.body, "Update?",
                        MessageBoxButtons.YesNo);

                if (dialogResult == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo($"https://github.com/stringandstickytape/MaxLights/releases") { UseShellExecute = true });
                    Application.Exit();
                }
            }
        }

        private void InitRgbNet()
        {
            if (!_appController.AppSettings.EnableASUS && !_appController.AppSettings.EnableLogitech && !_appController.AppSettings.EnableMSI && !_appController.AppSettings.EnableCorsair) return;



            _appController.surface = new RGBSurface();

            if(_appController.AppSettings.EnableMSI) AttachProvider(_appController.surface, new MsiDeviceProvider());
            if (_appController.AppSettings.EnableASUS) AttachProvider(_appController.surface, new AsusDeviceProvider());
            if (_appController.AppSettings.EnableCorsair) AttachProvider(_appController.surface, new CorsairDeviceProvider());
            if (_appController.AppSettings.EnableLogitech) AttachProvider(_appController.surface, new LogitechDeviceProvider());

            //var d = new DMXDeviceProvider();
            //var dd = new E131DMXDeviceDefinition("192.168.2.4");
            //dd.DeviceType = RGBDeviceType.Unknown;
            //dd.Universe = 1;
            //dd.CID = new byte[] { 1, 2, 3, 4, 5 };
            //
            //var fR = new Func<RGB.NET.Core.Color, byte>((c) => { return (byte)(c.R * 255); });
            //var fG = new Func<RGB.NET.Core.Color, byte>((c) => { return (byte)(c.G * 255); });
            //var fB = new Func<RGB.NET.Core.Color, byte>((c) => { return (byte)(c.B * 255); });
            //
          ////  var l = new Led(IRGBDevice device, LedId id, Point location, Size size, object ? customData = null)
            //
            //
            //for (var i = 0; i < 170; i++)
            //    dd.AddLed(LedId.LedStripe1+i, (3*i, fR), (3*i+1, fG), (3*i+2, fB));
            //
            //
            //
            //d.AddDeviceDefinition(dd);
            //
            //AttachProvider(_appController.surface, d);
            //LogitechZoneRGBDevice

            var devs = _appController.surface.GetDevices(RGBDeviceType.All);
            //_appControllerSurface.Attach(LogitechDeviceProvider.Instance);
            //
            //
            //_appControllerSurface.LoadDevices(LogitechDeviceProvider.Instance);
            //_appControllerSurface.LoadDevices(CorsairDeviceProvider.Instance);
            //_appControllerSurface.LoadDevices(MsiDeviceProvider.Instance);
            ////_appController.surface.LoadDevices(AsusDeviceProvider.Instance);
            var devices = _appController.surface.Devices;
            ////var d = _appController.surface.Devices.ToList()[5];
            //
            foreach (var device in _appController.surface.Leds.GroupBy(x => x.Device))
            {
                _appController.Luminaires.Add(new RgbNetDevice

                {
                    Label = device.Key.DeviceInfo.DeviceName,
                    Zones = _appController.surface.Leds.Where(x => x.Device == device.Key).Count(),
                    RgbNetLeds = _appController.surface.Leds.Where(x => x.Device == device.Key).OrderBy(x => x.Location.X).ThenBy(x => x.Location.Y).ToList()

                }
                    );

            }
        }

        private static void AttachProvider(RGBSurface surface, IRGBDeviceProvider provider)
        {
            try
            {
                provider.Initialize(throwExceptions: true);
                surface.Attach(provider.Devices);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"RGB.Net Initialisation Exception:\r\n{ex.ToString()}");
            }
        }




        private delegate void Delegate();
        private delegate void IntDelegate(int x, System.Drawing.Color c);


        private void UpdateUIBulbCount()
        {
            lumPanel.Controls.Clear();
            var orderedList = _appController.Luminaires.OrderBy(x => x.GetType().Name).ThenBy(x => x.Label).ToList();
            foreach (var l in orderedList)
            {
                var indivLumPanel = new Panel();
                lumPanel.Controls.Add(indivLumPanel);

                indivLumPanel.Location = new System.Drawing.Point { X = 0, Y = orderedList.IndexOf(l) * 36 };
                indivLumPanel.Height = 36;
                indivLumPanel.Width = lumPanel.Width;

                var lbl = $"{l.Label} {l.IpAddress}{(l.MacAddress == null ? "" : $" {l.MacAddress}")} {l.Zones} LEDs/Zones";

                indivLumPanel.Controls.Add(new Label { Text = lbl, Location = new System.Drawing.Point { X = 0, Y = 0 }, Width = indivLumPanel.Width / 2, Height = 36, TextAlign = ContentAlignment.TopLeft });
                indivLumPanel.Controls.Add(new Label { Text = $"{l.GetType().Name} {(l.IsStatic ? "Static" : "Dynamic")}", Location = new System.Drawing.Point { X = indivLumPanel.Width / 2, Y = 4 }, Width = indivLumPanel.Width / 3, TextAlign = ContentAlignment.TopLeft });

                if (l.IsStatic)
                {
                    var button = new Button { Text = "Delete", Tag = l, Location = new System.Drawing.Point { X = indivLumPanel.Width / 6 * 5, Y = 0 }, Width = indivLumPanel.Width / 12, Height = 36 };
                    button.Click += DeleteLuminaireClicked;
                    indivLumPanel.Controls.Add(button);
                } else 
                if(l is LifxDevice)
                {
                    var button3 = new Button { Text = "Control", Tag = l, Location = new System.Drawing.Point { X = indivLumPanel.Width / 6 * 5, Y = 0 }, Width = indivLumPanel.Width / 12, Height = 36 };
                    button3.Click += ControlLifxLuminarieClicked;
                    indivLumPanel.Controls.Add(button3);
                }

                var button2 = new Button { Text = "Monitor", Tag = l, Location = new System.Drawing.Point { X = indivLumPanel.Width / 12 * 11, Y = 0 }, Width = indivLumPanel.Width / 12, Height = 36 };
                button2.Click += MonitorLuminaireClicked;
                indivLumPanel.Controls.Add(button2);
            }
        }

        private void ControlLifxLuminarieClicked(object? sender, EventArgs e)
        {
            LifxDevice l = (LifxDevice)(((Button)sender).Tag);
            var f = new LifxControl(l, Controller);
            f.Show();
        }

        private void MonitorLuminaireClicked(object? sender, EventArgs e)
        {
            ILuminaireDevice l = (ILuminaireDevice)(((Button)sender).Tag);

            foreach (var lo in _appController.Luminaires)
                if(l!=lo)
                    lo.Monitor = false;

            var bitmap2 = new Bitmap(l.Zones, 1);
            var oldBitmap = (Bitmap)(pictureBox1.Image);
            pictureBox1.Image = bitmap2;
            oldBitmap.Dispose();
            bitmap = bitmap2;
            l.Monitor = !l.Monitor;
        }

        private void DeleteLuminaireClicked(object sender, EventArgs e)
        {
            var luminaireToDelete = ((Button)sender).Tag;

            if (_appController.AppSettings.StaticLifxDevices.Contains(luminaireToDelete))
            {
                _appController.AppSettings.StaticLifxDevices.Remove((LifxDevice)luminaireToDelete);
                _appController.Luminaires.Remove((LifxDevice)luminaireToDelete);
            }

            if (_appController.AppSettings.StaticWledDevices.Contains(luminaireToDelete))
            {
                _appController.AppSettings.StaticWledDevices.Remove((WledUdpDevice)luminaireToDelete);
                _appController.Luminaires.Remove((WledUdpDevice)luminaireToDelete);
            }

            listChanged(null, null);

            _appController.SaveSettings();
        }

        private void listChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            Delegate d = UpdateUIBulbCount;

            try
            {
                if (!IsHandleCreated) return;
                Invoke(d);
            }
            catch (Exception e)
            {

            }
        }

        private void SetPixel2(int x, System.Drawing.Color c)
        {
            bitmap.SetPixel(x, 0, c);
            
        }

        public void SetPixel(object sender, object args)
        {
            IntDelegate d = SetPixel2;

            try
            {
                if (!IsHandleCreated) return;
                Invoke(d,(int)sender, (System.Drawing.Color)args);
            }
            catch (Exception e)
            {

            }
        }

        private void PbRefresh2()
        {
            pictureBox1.Refresh();
        }

        public void PbRefresh(object sender, NotifyCollectionChangedEventArgs args)
        {
            Delegate d = PbRefresh2;

            try
            {
                if (!IsHandleCreated) return;
                Invoke(d);
            }
            catch (Exception e)
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_appController.LatestDiagram == null) return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON File | *.json";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, JsonConvert.SerializeObject(_appController.LatestDiagram));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON File | *.json";
            ofd.FilterIndex = 0;
            ofd.DefaultExt = "xml";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _appController.LoadFromJson(ofd.FileName, false);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_appController.AppSettings.Port == 0) _appController.AppSettings.Port = 8080;
            //System.Diagnostics.Process.Start($"http://localhost:{_appController.AppSettings.Port.ToString()}/toggle");

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo($"http://localhost:{_appController.AppSettings.Port.ToString()}/toggle") { UseShellExecute = true });
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _appController.Stop();
            engine.StopCapture();
        }

        private void tbPort_TextChanged(object sender, EventArgs e)
        {
            _appController.AppSettings.Port = int.Parse(tbPort.Text);
            _appController.SaveSettings();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var f = new NewDevice();
            var r = f.ShowDialog();

            var name = f.Controls["tbName"].Text;
            var ipAddress = f.Controls["tbIpAddress"].Text;
            var deviceType = f.Controls["cbDeviceType"].Text == "WLED" ? typeof(WledUdpDevice) : typeof(LifxDevice);
            var zones = int.Parse(f.Controls["numZones"].Text);

            ILuminaireDevice dev;

            if (deviceType == typeof(WledUdpDevice))
            {
                dev = new WledUdpDevice
                {
                    Label = name,
                    IpAddress = ipAddress,
                    Zones = zones,
                    IsStatic = true,
                    Port = (int)((NumericUpDown) f.Controls["nNewDevicePort"]).Value
                };
                _appController.AppSettings.StaticWledDevices.Add((WledUdpDevice)dev);
            }
            else
            {
                dev = new LifxDevice
                {
                    Label = name,
                    IpAddress = ipAddress,
                    Zones = zones,
                    IsStatic = true,
                    Port = (int)((NumericUpDown)f.Controls["nNewDevicePort"]).Value
                };
                _appController.AppSettings.StaticLifxDevices.Add((LifxDevice)dev);
            }

            _appController.Luminaires.Add(dev);
            _appController.SaveSettings();
        }



        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON File | *.json";
            ofd.FilterIndex = 0;
            ofd.DefaultExt = "xml";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _appController.LoadFromJson(ofd.FileName, true);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Life is short.
            Environment.Exit(0);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(((System.Windows.Forms.LinkLabel)sender).Text) { UseShellExecute = true });
        }

        private void cbM_CheckedChanged(object sender, EventArgs e)
        {
            _appController.AppSettings.EnableMSI = cbM.Checked;
            _appController.SaveSettings();
        }

        private void cbA_CheckedChanged(object sender, EventArgs e)
        {
            _appController.AppSettings.EnableASUS = cbA.Checked;
            _appController.SaveSettings();
        }

        private void cbC_CheckedChanged(object sender, EventArgs e)
        {
            _appController.AppSettings.EnableCorsair = cbC.Checked;
            _appController.SaveSettings();
        }

        private void cbL_CheckedChanged(object sender, EventArgs e)
        {
            _appController.AppSettings.EnableLogitech = cbL.Checked;
            _appController.SaveSettings();
        }

        //private void button5_Click(object sender, EventArgs e)
        //{
        //    var selectedItem = listBox1.SelectedItem.ToString();
        //    var i = _appController.AppSettings.StaticLifxDevices.Where(l => $"{l.Label} : {l.IpAddress} : {l.MacAddress}" == selectedItem).ToList();
        //    var j = _appController.AppSettings.StaticWledDevices.Where(l => $"{l.Label} : {l.IpAddress} : {l.MacAddress}" == selectedItem).ToList();
        //
        //    foreach (var ii in i)
        //    {
        //        _appController.AppSettings.StaticLifxDevices.Remove(ii);
        //        _appController.Luminaires.Remove(ii);
        //    }
        //
        //    foreach (var jj in j)
        //    {
        //        _appController.AppSettings.StaticWledDevices.Remove(jj);
        //        _appController.Luminaires.Remove(jj);
        //    }
        //
        //    _appController.SaveSettings();
        //}
    }


}
