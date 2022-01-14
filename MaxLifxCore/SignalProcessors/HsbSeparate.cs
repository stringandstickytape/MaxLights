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
    class HsbSeparate : SignalProcessorBase, ISignalGenerator
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
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "Hue", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out2", OutputName = "out2", Label = "Saturation", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out3", OutputName = "out3", Label = "Brightness", Socket = ListSocket}
                    },
                ComponentJsName = "HsbSeparateComponent",
                ComponentName = "HSB Separate",
                HelpText = "Separates a list of HSBs into three lists of Ushorts.",
            };
        }


        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var inputList = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug);


            switch (outputSocketName)
            {
                case "out1": return inputList.Select(x => x.H).ToList();
                case "out2": return inputList.Select(x => x.S).ToList();
                case "out3": return inputList.Select(x => x.B).ToList();
                default:
                    throw new NotImplementedException();
            }
        }
    }



}
