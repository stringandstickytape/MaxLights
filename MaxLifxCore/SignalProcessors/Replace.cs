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
    class Replace : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "In List", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Replace This Many", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Starting At", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp4", InputName = "num4", Label = "With Values From", Socket = ListSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Output", Socket = ListSocket}
                    },
                ComponentJsName = "ReplaceComponent",
                ComponentName = "Replace",
                HelpText = "Replaces a number of numbers at a certain point in the list, with the corresponding numbers in another list.",
            };
        }


        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null) { throw new NotImplementedException(); }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var inList = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0],debug);
            var itemsToReplace = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            var startingAt = gen[2].GetLatestValue(controller, light, OutputSocketName2[2], debug);
            var substList = gen[3].GetLatestListValues(controller, light, OutputSocketName2[3],debug);

            return inList.Take(startingAt).Concat(substList.Skip(startingAt).Take(itemsToReplace)).Concat(inList.Skip(startingAt + itemsToReplace)).ToList();
        }

    }



}
