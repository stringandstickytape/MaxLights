using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class HsbCombine : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Hue", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Saturation", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Brightness", Socket = ListSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "HSB", Socket = HsbSocket},
                    },
                ComponentJsName = "HsbCombineComponent",
                ComponentName = "HSB Combine",
                HelpText = "Combines three lists of Ushorts into a single list of HSBs.",
            };
        }


        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var h = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug);
            var s = gen[1].GetLatestListValues(controller, light, OutputSocketName2[1], debug);
            var b = gen[2].GetLatestListValues(controller, light, OutputSocketName2[2], debug);
            var retVal = new List<HsbUshort>();

            for (var i = 0; i < Math.Min(Math.Min(b.Count,s.Count),h.Count); i++)
                retVal.Add(new HsbUshort { H = h[i], S = s[i], B = b[i] });

            return retVal;
        }
    }



}
