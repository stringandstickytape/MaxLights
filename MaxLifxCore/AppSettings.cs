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
    }
}
