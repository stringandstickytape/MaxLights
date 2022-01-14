using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class HsbJoin : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "HSB #1", Socket = HsbSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "HSB #2", Socket = HsbSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "(optional) HSB #3", Socket = HsbSocket},
                        new DiagramInput { JsToken = "inp4", InputName = "num4", Label = "(optional) HSB #4", Socket = HsbSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "HSB #1", Socket = HsbSocket},
                    },
                ComponentJsName = "HSBJoinComponent",
                ComponentName = "HSB Join",
                HelpText = "Joins multiple HSB lists into one.",
            };
        }


        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var inputList1 = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug);
            var inputList2 = gen[1]?.GetLatestHsbListValues(controller, light, OutputSocketName2[1], debug);
            var inputList3 = gen[2]?.GetLatestHsbListValues(controller, light, OutputSocketName2[2], debug);
            var inputList4 = gen[3]?.GetLatestHsbListValues(controller, light, OutputSocketName2[3], debug);

            if (inputList2 != null)  inputList1.AddRange(inputList2);
            if(inputList3 != null) inputList1.AddRange(inputList3);
            if (inputList4 != null) inputList1.AddRange(inputList4);

            return inputList1;
        }
    }



}
