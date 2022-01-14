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
    class RepeatList : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Number of times to repeat the list", Socket = NumberSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Shrunk list", Socket = ListSocket}
                    },
                ComponentJsName = "RepeatListComponent",
                ComponentName = "Repeat List",
                HelpText = "Produces a list from multiple copies of the input list",
            };
        }


        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var inputList = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug);
            var listCopies = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);



            var outputList = Enumerable.Repeat(inputList, listCopies).SelectMany(x => x).ToList();

            return outputList;
        }

    }



}
