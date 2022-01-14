using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MaxLifx.Controllers
{
    public interface ILuminaireDevice
    {
        bool DeviceSupportsExtendedMultizone { get; }

        string MacAddress { get; set; }

        string IpAddress { get; set; }

        int Zones { get; set;  }
         
        string Label { get; set; }
    }

    public class LifxDevice : ILuminaireDevice
    {
        private bool _deviceSupportsExtendedMultizone = false;

        public bool DeviceSupportsExtendedMultizone
        {
            get
            {
                if (_zones == 0) throw new InvalidOperationException();
                return _deviceSupportsExtendedMultizone;
            }
        }
        

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
    }

    public class RgbNetDevice : ILuminaireDevice
    {
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

        public string IpAddress
        {
            get { return $"{Label} : {Zones}"; }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<Led> RgbNetLeds { get; set; }
    }
}
