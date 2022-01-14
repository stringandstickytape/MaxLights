using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MaxLifxCoreBulbController.Controllers
{
    public interface ILuminaireDevice
    {
        bool DeviceSupportsExtendedMultizone { get; }

        string MacAddress { get; set; }

        string IpAddress { get; set; }

        int Zones { get; set;  }
         
        string Label { get; set; }

        int Port { get; set; }

        bool IsStatic { get; set; }
        bool Monitor { get; set; }
    }

    public class LifxDevice : ILuminaireDevice
    {

        public bool Monitor { get; set; }

        private bool _deviceSupportsExtendedMultizone = false;

        public int Port { get; set; }
        public bool DeviceSupportsExtendedMultizone
        {
            get
            {
                if (_zones == 0) throw new InvalidOperationException();
                return _deviceSupportsExtendedMultizone;
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

                if (_zones > 1) //(!_macAddress.StartsWith("D0"))
                    _deviceSupportsExtendedMultizone = true;
                else _deviceSupportsExtendedMultizone = false;
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
        public override string ToString()
        {
            return $"{Label} : {(IsStatic ? "Static" : "Dynamic")} Lifx UDP Device at {IpAddress}:{Port}, MAC address {_macAddress}";
        }
    }

    public class RgbNetDevice : ILuminaireDevice
    {
        public bool Monitor { get; set; }
        public bool IsStatic { get; set; }
        public int Zones { get; set; }
        public string Label { get; set; }

        public bool DeviceSupportsExtendedMultizone { get { throw new NotImplementedException(); }  }

        public string MacAddress
        {
            get { return $"{Label} : {Zones}"; }
            set
            {
                throw new NotImplementedException();
            }
        }
        public int Port { get; set; }

        public string IpAddress
        {
            get { return $"{Label} : {Zones}"; }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            return $"{Label} : RGB.NET Device with {Zones} LEDs";
        }

        public IEnumerable<Led> RgbNetLeds { get; set; }
    }
}
