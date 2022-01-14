using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class UshortIfLessThan : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Number", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "LessThan", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Then", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp4", InputName = "num4", Label = "Else", Socket = ListSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Output", Socket = ListSocket}
                    },
                ComponentJsName = "NumberIfLessThanComponent",
                ComponentName = "If Less Than",
                HelpText = "Compares two numbers and returns one of two lists depending on the result.",
            };
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var v1 = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);
            var v2 = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            if (v1 < v2)
                return gen[2].GetLatestListValues(controller, light, OutputSocketName2[2], debug);
            else return gen[3].GetLatestListValues(controller, light, OutputSocketName2[3], debug);
        }
    }



}
