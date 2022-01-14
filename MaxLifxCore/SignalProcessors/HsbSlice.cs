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
    class HsbSlice : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "HSB", Socket = HsbSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Split at", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "(optional) Also split at", Socket = NumberSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "HSB #1", Socket = HsbSocket},
                        new DiagramOutput { JsToken = "out2", OutputName = "num1", Label = "HSB #2", Socket = HsbSocket},
                        new DiagramOutput { JsToken = "out3", OutputName = "num2", Label = "(optional) HSB #3", Socket = HsbSocket},
                    },
                ComponentJsName = "HSBSliceComponent",
                ComponentName = "HSB Slice",
                HelpText = "Splits a list of HSBs into two or three lists.",
            };
        }


        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var inputList = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug);
            var split1 = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            ushort? split2 = null;
            if(gen[2] != null) split2 = gen[2].GetLatestValue(controller, light, OutputSocketName2[2], debug);

            switch (outputSocketName)
            {
                case "num": return inputList.Take(split1).ToList();
                case "num1":
                        if (split2 != null)
                            return inputList.Skip(split1).Take(split2.Value - split1).ToList();
                        else return inputList.Skip(split1).ToList();
                case "num2": return inputList.Skip(split2.Value).ToList();
                default:
                    throw new NotImplementedException();
            }
        }
    }



}
