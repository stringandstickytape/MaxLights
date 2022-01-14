using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MaxLifxCoreBulbController.Controllers
{
    public class WledUdpDevice : ILuminaireDevice
    {
        public bool Monitor { get; set; }
        public bool DeviceSupportsExtendedMultizone
        {
            get
            {
                return true;
            }
        }

        public bool IsStatic { get; set; }


        private string _macAddress;
        public string MacAddress { get { return _macAddress; } set { _macAddress = value; } }
        public string IpAddress { get; set; }
        private string _label;

        private int _zones;
        public int Zones
        {
            get
            {
                return _zones;
            }
            set
            {
                _zones = value;
            }
        }
        public string Label
        {
            get
            {
                if (String.IsNullOrEmpty(_label))
                    return MacAddress;
                return _label;
            }
            set { _label = value; }
        }

        public int Port { get; set; }

        public override string ToString()
        {
            return $"{Label} : {(IsStatic ? "Static" : "Dynamic")} Wled UDP Device at {IpAddress}:{Port}";
        }
    }
}
