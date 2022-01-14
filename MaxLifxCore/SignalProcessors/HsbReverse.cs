using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class HsbReverse : SignalProcessorBase, ISignalGenerator
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
                         new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "Hue", Socket  = HsbSocket},
                    },
                ComponentJsName = "HsbReverseComponent",
                ComponentName = "HSB Reverse",
                HelpText = "Separates a list of HSBs into three lists of Ushorts.",
            };
        }


        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var l = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug);
            l.Reverse();
            return l;
        }
    }



}
