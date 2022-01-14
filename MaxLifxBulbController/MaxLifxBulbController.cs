using MaxLifx.Util;
using MaxLifx.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MaxLifx.Packets;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MaxLifx.Controllers
{
    public class LabelAndColourPayload
    {
        public string Label;
        public SetColourPayload Payload;
    }
    public class MaxLifxBulbController
    {
        // Network details
        UdpClient _receivingUdpClient;
        string _localIp = Utils.LocalIPAddress();
        Socket _sendingSocket;
        IPAddress _sendToAddress;
        IPEndPoint _sendingEndPoint;
        ObservableCollection<ILuminaireDevice> luminaires;
        Func<int> func;
        UdpClient udpClient, udpClient2;
        object lockObj;


        public event EventHandler ColourSet;

        public void SetColour(ILuminaireDevice bulb, int zone, SetColourPayload payload, bool updateBox)
        {

            if (bulb.Zones > 1)
            {
                var newPayload = new SetColourZonesPayload()
                {
                    Brightness = payload.Brightness,
                    Hue = payload.Hue,
                    Kelvin = payload.Kelvin,
                    Saturation = payload.Saturation,
                    TransitionDuration = payload.TransitionDuration,
                    start_index = new byte[] { (byte)zone },
                    end_index = new byte[] { (byte)zone },
                    apply = new byte[] { 0 }
                };

                SendPayloadToMacAddress(newPayload, bulb.MacAddress, bulb.IpAddress);
            }
            else
                SendPayloadToMacAddress(payload, bulb.MacAddress, bulb.IpAddress);

            // this updates the bulb monitor, skip for multizone lights
            if (updateBox)
            {
                ColourSet?.Invoke(new LabelAndColourPayload() { Label = bulb.Label, Payload = payload }, null);
            }
        }


        Dictionary<string, byte[]> macAddressCache = new Dictionary<string, byte[]>();
        public void SendPayloadToMacAddress(IPayload Payload, string macAddress, string ipAddress, UdpClient persistentClient = null)
        {
            var fullMac = $"{macAddress}0000";
            if (!macAddressCache.ContainsKey(fullMac))
                macAddressCache.Add(fullMac, Utils.StringToByteArray(fullMac));

            SendPayloadToMacAddress(Payload, macAddressCache[fullMac], ipAddress, persistentClient);
        }

        public void SendPayloadToMacAddress(IPayload Payload, byte[] targetMacAddress, string ipAddress, UdpClient persistentClient = null)
        {
            IPAddress sendToAddress = IPAddress.Parse(ipAddress);
            IPEndPoint sendingEndPoint = new IPEndPoint(sendToAddress, 56700);

            byte[] sendData = PacketFactory.GetPacket(targetMacAddress, Payload);

            if (persistentClient == null)
            {
                var a = new UdpClient();
                a.Connect(sendingEndPoint);
                a.Send(sendData, sendData.Length);
                a.Close();
            }
            else persistentClient.Send(sendData, sendData.Length);
        }

        public static UdpClient GetPersistentClient(string macAddress, string ipAddress)
        {
            var targetMacAddress = Utils.StringToByteArray(macAddress + "0000");
            IPAddress sendToAddress = IPAddress.Parse(ipAddress);
            IPEndPoint sendingEndPoint = new IPEndPoint(sendToAddress, 56700);
            var a = new UdpClient();
            a.Connect(sendingEndPoint);
            return a;
        }

        public void Stop()
        {
            if (discoveryThread != null && discoveryThread.IsAlive)
                discoveryThread.Abort();
        }

        Thread discoveryThread;
        public void UdpDiscoveryAsyncListen(ref ObservableCollection<ILuminaireDevice> Luminaires, ref object LockObj, Func<int> _func)
        {
            lockObj = LockObj;
            luminaires = Luminaires;
            func = _func;
            byte[] sendData;
            
            var gspTime = DateTime.Now.AddDays(-1);

           //if (luminaires.Any())
           //    lock (lockObj)
           //    {
           //        luminaires.Clear();
           //    }

            GetServicePayload payload = new GetServicePayload();
            sendData = PacketFactory.GetPacket(new byte[8], payload);

            var ep1 = new IPEndPoint(IPAddress.Any, 56700);

            if (udpClient == null)
            {
                udpClient = new UdpClient();
                udpClient.ExclusiveAddressUse = false;
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpClient.Client.Blocking = false;
                udpClient.Client.Bind(ep1);
            }

            udpClient.BeginReceive(new AsyncCallback(UdpDiscoveryListen), null);
            udpClient.Send(sendData, sendData.Length, _sendingEndPoint);

            discoveryThread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        // Poll for bulbs every 5 seconds
                        if ((DateTime.Now - gspTime).TotalMilliseconds > 5000)
                        {
                            udpClient.SendAsync(sendData, sendData.Length, _sendingEndPoint);

                            gspTime = DateTime.Now;
                        }
                        Thread.Sleep(1000);
                    }
                } catch (ThreadAbortException ex)
                {

                }
            });

            discoveryThread.Start();
        }

        public void UdpDiscoveryListen(IAsyncResult res)
        {
            byte[] sendData;
            ILuminaireDevice bulb;

            GetServicePayload payload = new GetServicePayload();
            sendData = PacketFactory.GetPacket(new byte[8], payload);

            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 56700);

            var receivebytes = udpClient.EndReceive(res, ref remoteEndPoint);
            udpClient.BeginReceive(new AsyncCallback(UdpDiscoveryListen), null);

            // Get the MAC address of the bulb replying
            var macAddress = Utils.ByteArrayToString(receivebytes).Substring(16, 12);
            //Debug.WriteLine(macAddress);

            if (macAddress != "000000000000")
            {
                if (macAddress == "D073D5263A62")
                    Debug.WriteLine("!");
                //System.Diagnostics.Debug.WriteLine($"{macAddress} : {receivebytes[32] + ((receivebytes[33] * 256))}");
                var clientEndPoint = new IPEndPoint(remoteEndPoint.Address, 56700);
                switch (receivebytes[32])
                {
                    case 22:
                        var brightness = receivebytes[34] << 8 + receivebytes[35];
                        if (macAddress == "D073D5263A62")
                            Debug.WriteLine("!");
                        break;

                    case 3:
                        var newBulb = new LifxDevice() { MacAddress = macAddress, IpAddress = remoteEndPoint.Address.ToString() };
                        Debug.WriteLine(macAddress);
                        lock (lockObj)
                        {
                            if (luminaires.Count(x => x.MacAddress == macAddress) == 0)
                            {
                                // Send a GetLabel
                                GetLabelPayload labelPayload = new GetLabelPayload();
                                sendData = PacketFactory.GetPacket(Utils.StringToByteArray(newBulb.MacAddress + "0000"), labelPayload);
                                //SendDataPacket(sendData, remoteEndPoint);
                                SendDataPacket(sendData, clientEndPoint);

                                luminaires.Add(newBulb);
                                if (macAddress == "D073D5263A62")
                                {
                                    Debug.WriteLine("!");
                                }
                            }
                            else
                            {

                               //GetLabelPayload labelPayload = new GetLabelPayload();
                               //sendData = PacketFactory.GetPacket(Utils.StringToByteArray(newBulb.MacAddress + "0000"), labelPayload);
                               ////SendDataPacket(sendData, remoteEndPoint);
                               //SendDataPacket(sendData, clientEndPoint);

                               //sendData = PacketFactory.GetPacket(Utils.StringToByteArray(newBulb.MacAddress + "0000"), new GetLightPowerPayload());
                               //SendDataPacket(sendData, clientEndPoint);
                            
                                //Debug.WriteLine("Bulb already added");
                            }

                        }
                        break;
                    case 25: // State Label
                        if (macAddress == "D073D5263A62")
                            Debug.WriteLine("!");
                        lock (lockObj)
                        {
                            bulb = luminaires.FirstOrDefault(x => x.MacAddress == macAddress);
                        }

                        if (bulb != null)
                        {
                            // Parse the received label and mark it against the bulb
                            var label1 = Utils.HexToAscii(Utils.ByteArrayToString(receivebytes).Substring(36 * 2));
                            bulb.Label = label1.Substring(0, label1.IndexOf('\0'));

                            int r;

                            lock (lockObj)
                            {
                                r = func.Invoke();
                            }
                            
                            GetColourZonesPayload ColourZonesPayload = new GetColourZonesPayload();

                            sendData = PacketFactory.GetPacket(Utils.StringToByteArray(bulb.MacAddress + "0000"), ColourZonesPayload);
                            SendDataPacket(sendData, clientEndPoint);

                            sendData = PacketFactory.GetPacket(Utils.StringToByteArray(bulb.MacAddress + "0000"), new GetLightPowerPayload());
                            SendDataPacket(sendData, clientEndPoint);
                        }
                        else
                        {
                            Debug.WriteLine("Ignoring Bulb Label");
                        }
                        break;
                    case 247:
                    case 250:

                        if (receivebytes[33] == 1) // 503 Zones
                        {
                            lock (lockObj)
                            {
                                bulb = luminaires.FirstOrDefault(x => x.MacAddress == macAddress);
                            }

                            if (bulb != null)
                            {
                                bulb.Zones = receivebytes[36];
                            }
                        }
                        break;
                    case 107:
                        if (macAddress == "D073D502A772")
                            Debug.WriteLine("!");
                        Debug.WriteLine($"107:{macAddress}");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                var a = 1;
            }
        }

        public static void SendDataPacket(byte[] sendData, IPEndPoint remoteEndPoint)
        {
            var port = 56701;
            using (var u2 = new UdpClient())
            {
                u2.ExclusiveAddressUse = false;
                u2.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                u2.Client.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), port++));
                u2.Connect(remoteEndPoint.Address, 56700);
                u2.Send(sendData, sendData.Length);
            }
        }

        // The following is taken from https://github.com/PhilWheat/LIFX-Control
        public void SetupNetwork()
        {
            var pos = _localIp.LastIndexOf('.');
            if (pos >= 0)
                _localIp = _localIp.Substring(0, pos);
            _localIp = _localIp + ".255";

            SetupNetwork(_localIp);

        }

        private void SetupNetwork(string ip)
        {
            _localIp = ip;
            _sendToAddress = IPAddress.Parse(ip);
            _sendingEndPoint = new IPEndPoint(_sendToAddress, 56700);
        }
    }
}