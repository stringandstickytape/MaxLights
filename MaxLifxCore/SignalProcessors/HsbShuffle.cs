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
    class HsbShuffle : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "HSB", Socket = HsbSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "HSB", Socket = HsbSocket},
                    },
                ComponentJsName = "HsbShuffleComponent",
                ComponentName = "HSB Shuffle",
                HelpText = "Randomly reoders a list of HSBs.",
            };
        }
        private Random _r = new Random();

        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            return gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug).OrderBy(x => _r.Next(int.MinValue, int.MaxValue - 1)).ToList();
        }

    }



}
