using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxLifxCore.SignalGenerators
{
    public interface ISignalGenerator
    {
        ISignalGenerator Initialise(Random r, DateTime startTime, double interval, int nodeId);

        bool GetLatestBoolValue(AppController controller, Light light, StringBuilder debug = null);

        string GetLatestStringValue(AppController controller, Light light, StringBuilder debug = null);

        ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null);

        float GetLatestFloatValue(AppController controller, Light light, string socketName, StringBuilder debug = null);

        List<ushort> GetLatestListValues(AppController controller, Light light, string socketName, StringBuilder debug = null);

        List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null);

        int NodeId { get; set; }
        void SetupGenerator(DiagramNode node, Diagram diagram, AppController controller);
        void EndLoop();
    }
}
