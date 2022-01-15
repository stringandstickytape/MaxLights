using MaxLifxCoreBulbController.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxLifxCore
{
    public  class AppSettings
    {
        public int Port { get; set; }
        public List<LifxDevice> StaticLifxDevices { get; set; }
        public List<WledUdpDevice> StaticWledDevices { get; set; }
        public bool EnablePCHardware { get; set; }
        public bool EnableMSI { get; set; }
        public bool EnableASUS { get; set; }
        public bool EnableCorsair { get; set; }
        public bool EnableLogitech { get; set; }
    }
}
