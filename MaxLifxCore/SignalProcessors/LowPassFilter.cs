using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class LowPassFilter : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Threshold", Socket = NumberSocket}
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "List", Socket = ListSocket}
                    },
                ComponentJsName = "LowPassFilterComponent",
                ComponentName = "Low Pass Filter",
                HelpText = "Filters out values lower than a certain threshold, and sets them to 0.",
            };
        }

        private Random _r;
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }
        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var threshold = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            return gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug).Select(x => x < threshold ? (ushort) 0 : (ushort)x).ToList();
        }

    }



}
