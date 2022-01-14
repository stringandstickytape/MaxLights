using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxLifxCore.SignalGenerators
{
    abstract class SignalGeneratorBase : ISignalGenerator
    {
        public DateTime StartTime { get; internal set; }
        public double Interval { get; internal set; }

        internal ushort _currentValue { get; set; }

        public Random Random { get; internal set; }

        public DateTime NextDue { get; internal set; }

        public string GetLatestStringValue(AppController controller, Light light, StringBuilder debug = null) { throw new NotImplementedException(); }

        public bool GetLatestBoolValue(AppController controller, Light light, StringBuilder debug = null) { throw new NotImplementedException(); }

        public int NodeId { get; set; }
        public ISignalGenerator Initialise(Random r, DateTime d, double interval, int nodeId) 
        {
            Random = r;
            StartTime = DateTime.Now;
            NextDue = StartTime;
            Interval = interval;
            NodeId = nodeId;
            return this;
        }

        public float GetLatestFloatValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            return 0;
        }

        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            return 0;
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        { 
            throw new NotImplementedException(); 
        }

        public void SetupGenerator(DiagramNode node, Diagram diagram, AppController controller)
        {

        }

        public void EndLoop()
        {

        }
    }

}
