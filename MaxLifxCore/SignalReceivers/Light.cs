using MaxLifxCoreBulbController.Controllers;
using MaxLifxCore.SignalGenerators;
using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MaxLifxCoreBulbController;
using MaxLifxCoreBulbController.Payload;

namespace MaxLifxCore.SignalReceivers
{
    public class Light
    {
        public bool IsOn { get; set; }
        public ILuminaireDevice Luminaire { get; set; }
        private MaxLifxCoreBulbController.Controllers.MaxLifxCoreBulbController _controller;
        private DateTime _startTime;
        private DateTime _latestUpdate;
        public double _interval { get; set; }

        private AppController _appController;
        public Light(ILuminaireDevice bulb, MaxLifxCoreBulbController.Controllers.MaxLifxCoreBulbController controller, AppController appController, DateTime startTime)
        {
            Luminaire = bulb;
            _controller = controller;
            _startTime = startTime;
            _latestUpdate = DateTime.Now;
            _appController = appController;
            ExtendedMultizonePayloadCache = new ExtendedMultizonePayloadCache();
        }

        public bool IsExpired()
        {
            return DateTime.Now > _latestUpdate.AddMilliseconds(_interval);
        }

        public ISignalGenerator gen1, gen2, gen3, gen5, gen6, gen7, gen8;

        public string pos1, pos2, pos3, pos5, pos6, pos7, pos8;

        public List<ushort> prevHues, prevSats, prevBris;
        public List<HsbUshort> prevHSBs;
        public bool prevHsbMode;

        private UdpClient wledUdpClient = new UdpClient();

        public void SetBriInput(ISignalGenerator bri) { gen3 = bri; }
        public void SetSatInput(ISignalGenerator sat) { gen2 = sat; }
        public void SetHueInput(ISignalGenerator hue) { gen1 = hue; }

        public ExtendedMultizonePayloadCache ExtendedMultizonePayloadCache { get; private set; }
        private Dictionary<string, UdpClient> reusableMultizoneClientDictionary = new Dictionary<string, UdpClient>();

        public void SetInput(int input, ISignalGenerator bri, string parentOutputSocket) {
            switch (input)
            {
                case 1: gen1 = bri; pos1 = parentOutputSocket; break;
                case 2: gen2 = bri;  pos2 = parentOutputSocket;  break;
                case 3: gen3 = bri;  pos3 = parentOutputSocket; break;
                case 5: gen5 = bri;  pos5 = parentOutputSocket;  break;
                case 6: gen6 = bri; pos6  = parentOutputSocket;  break;
                case 7: gen7 = bri; pos7 = parentOutputSocket; break;
                case 8: gen8 = bri; pos8 = parentOutputSocket; break;
            }
        }

        public void Process()
        {
            StringBuilder debug = null;

            

            if (_appController.Debug)
            {
                debug = new StringBuilder();
            }
            List<ushort> hueVals = null, satVals = null, briVals = null;
            List<HsbUshort> hsbVals = null;
            bool hsbMode = false;

            var reverse = gen7.GetLatestBoolValue(_appController, this, debug);

            if (gen1 != null && gen2 != null && gen3 != null)
            {
                hueVals = gen1.GetLatestListValues(_appController, this, pos1, debug);
                satVals = gen2.GetLatestListValues(_appController, this, pos2, debug);
                briVals = gen3.GetLatestListValues(_appController, this, pos3, debug);

                if (reverse)
                {
                    hueVals = Enumerable.Reverse(hueVals).ToList();
                    satVals = Enumerable.Reverse(satVals).ToList();
                    briVals = Enumerable.Reverse(briVals).ToList();
                }
            }
            else
            {
                hsbVals = gen8.GetLatestHsbListValues(_appController, this, pos8, debug);
                hsbMode = true;
                if (reverse)
                    hsbVals = Enumerable.Reverse(hsbVals).ToList();//.Reverse();
            }

            var fade =  gen6.GetLatestBoolValue(_appController, this, debug);

            if (hsbMode)

                debug?.AppendLine("Renderer Input: " + string.Join(", ", hsbVals.ToList().Select(x => $"{x.H},{x.S},{x.B}").ToList()));
            else debug?.AppendLine($"Renderer Input: hues {string.Join(", ", hueVals.ToList().Select(x => $"{x}").ToList())} ; sats {string.Join(", ", satVals.ToList().Select(x => $"{x}").ToList())} ; bris {string.Join(", ", briVals.ToList().Select(x => $"{x}").ToList())}");

            if (Luminaire.Zones < 2 && Luminaire is LifxDevice)
            {
                if (Luminaire.Monitor)
                {
                    Color c2;
                    if (hsbMode) c2 = HSVColor.Create(((float)hsbVals[0].H) / 182.044f, ((float)hsbVals[0].S) / 65535, ((float)hsbVals[0].B) / 65535);
                    else c2 = HSVColor.Create(((float)hueVals[0]) / 182.044f, ((float)satVals[0]) / 65535, ((float)briVals[0]) / 65535);

                    _appController.Form.SetPixel(0, System.Drawing.Color.FromArgb((byte)(c2.R * 255), (byte)(c2.G * 255), (byte)(c2.B * 255)));
                }

                _controller.SetColour(
                    Luminaire,
                    0,
                    new SetColourPayload()
                    {
                        Brightness = hsbMode ? hsbVals[0].B : briVals[0],
                        Hue =  (int)(Math.Floor((hsbMode ? hsbVals[0].H : hueVals[0]) / 182.044)),
                        Saturation = hsbMode ? hsbVals[0].S : satVals[0],
                        TransitionDuration = fade ? (uint)_interval : 1,
                        Kelvin = 32767
                    },
                    false
                );
            }
            else
            {
                if (Luminaire is LifxDevice)
                {
                    for (int zoneCtr = 0; zoneCtr < Luminaire.Zones && 
                        ((hsbMode && zoneCtr < hsbVals.Count) || (zoneCtr < hueVals.Count)) &&
                        ((hsbMode) || (zoneCtr < satVals.Count)) &&
                        ((hsbMode) || (zoneCtr < briVals.Count)) 
                        ; zoneCtr++)
                    {
                        var payload = new SetColourPayload()
                        {
                            Brightness = hsbMode ? hsbVals[zoneCtr].B : briVals[zoneCtr],
                            Hue = (int)(Math.Floor((hsbMode ? hsbVals[zoneCtr].H : hueVals[zoneCtr]) / 182.044)),
                            Saturation = hsbMode ? hsbVals[zoneCtr].S : satVals[zoneCtr],
                            TransitionDuration = fade ? (uint)_interval : 1,
                            Kelvin = 32767
                        };

                        if (Luminaire.Monitor) 
                        {
                            Color c2;
                            if (hsbMode) c2 = HSVColor.Create(((float)hsbVals[zoneCtr].H) / 182.044f, ((float)hsbVals[zoneCtr].S) / 65535, ((float)hsbVals[zoneCtr].B) / 65535);
                            else c2 = HSVColor.Create(((float)hueVals[zoneCtr]) / 182.044f, ((float)satVals[zoneCtr]) / 65535, ((float)briVals[zoneCtr]) / 65535);

                            _appController.Form.SetPixel(zoneCtr, System.Drawing.Color.FromArgb((byte)(c2.R*255), (byte)(c2.G*255), (byte)(c2.B*255)));
                        }

                        if (Luminaire.DeviceSupportsExtendedMultizone)
                        {

                            ExtendedMultizonePayloadCache.Payloads.Add((Luminaire, zoneCtr), payload);
                        }
                        else
                        {
                            _controller.SetColour(
                                Luminaire,
                                zoneCtr,
                                payload,
                                false
                            );
                        }
                    }

                    if (Luminaire.DeviceSupportsExtendedMultizone)
                    {
                        ExtendedMultizonePayloadCache.Send(reusableMultizoneClientDictionary, _controller);
                        ExtendedMultizonePayloadCache.Clear();
                    }
                }
                else if (Luminaire is WledUdpDevice)
                {

                    int zoneMax = 0;
                    if (hsbMode) zoneMax = Math.Min(Luminaire.Zones, hsbVals.Count);
                    else zoneMax = Math.Min(Luminaire.Zones, Math.Min(hueVals.Count, Math.Min(satVals.Count, briVals.Count)));
                    
                    byte[] b = new byte[zoneMax * 3 + 4];
                    b[0] = 4;
                    b[1] = 2;
                    b[2] = 0; // start index high byte
                    b[3] = 0; // start index low byte

                    Color c;

                    for (int zoneCtr = 0; zoneCtr < zoneMax; zoneCtr++)
                    {
                        if (hsbMode) c = HSVColor.Create(((float)hsbVals[zoneCtr].H) / 182.044f, ((float)hsbVals[zoneCtr].S) / 65535, ((float)hsbVals[zoneCtr].B) / 65535);
                        else c = HSVColor.Create(((float)hueVals[zoneCtr]) / 182.044f, ((float)satVals[zoneCtr]) / 65535, ((float)briVals[zoneCtr]) / 65535);
                        
                        b[4+zoneCtr*3] = (byte)(c.R * 255);
                        b[5+zoneCtr*3] = (byte)(c.G * 255);
                        b[6+zoneCtr*3] = (byte)(c.B * 255);

                        if (Luminaire.Monitor) _appController.Form.SetPixel(zoneCtr, System.Drawing.Color.FromArgb((b[4 + zoneCtr * 3]), b[5 + zoneCtr * 3], b[6 + zoneCtr * 3]));
                    }

                    wledUdpClient.SendAsync(b, b.Length, Luminaire.IpAddress, Luminaire.Port);
                } else
                {
                    var leds = ((RgbNetDevice)Luminaire).RgbNetLeds;
                    var ledCtr = 0;

                    foreach (var led in leds)
                    {
                        if (!hsbMode)
                            if(hueVals.Count-1 < ledCtr || satVals.Count-1 < ledCtr || briVals.Count-1 < ledCtr) break;
                        if (hsbMode)
                            if (hsbVals.Count - 1 < ledCtr) break;
                        
                        if (hsbMode) led.Color = HSVColor.Create(((float)hsbVals[ledCtr].H) / 182.044f, ((float)hsbVals[ledCtr].S) / 65535, ((float)hsbVals[ledCtr].B) / 65535);
                        else led.Color = HSVColor.Create(((float)hueVals[ledCtr]) / 182.044f, ((float)satVals[ledCtr]) / 65535, ((float)briVals[ledCtr]) / 65535);

                        if (Luminaire.Monitor)
                        {
                            _appController.Form.SetPixel(ledCtr, System.Drawing.Color.FromArgb((byte)(led.Color.R * 255), (byte)(led.Color.G * 255), (byte)(led.Color.B * 255)));
                        }

                        ledCtr++;
                    }
                        
                    _appController.surface.Update();
                }

                if (Luminaire.Monitor) _appController.Form.PbRefresh(null, null);
            }

            _interval = gen5.GetLatestValue(_appController, this, pos5, debug?.AppendLine("Interval:"));

            _latestUpdate = DateTime.Now;

            if (hsbMode)
            {
                prevHsbMode = true;
                prevHSBs = hsbVals;
            }
            else
            {
                prevHsbMode = false;
                prevHues = hueVals;
                prevSats = satVals;
                prevBris = briVals;
            }
        }

        internal bool InputsAreSet()
        {
            return (gen1 != null && gen2 != null && gen3 != null) || gen8 != null;
        }
    }

    public struct HsbUshort
    {
        public ushort H;
        public ushort S;
        public ushort B;

    }
}
