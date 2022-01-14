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
    class UshortListGenerator : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Item", Socket = NumberSocket },
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Items to Generate", Socket = NumberSocket }
                },
                Outputs = new List<DiagramOutput>()
                {
                        new DiagramOutput { JsToken = "out1", OutputName = "list", Label = "List of Numbers", Socket = ListSocket}
                },
                ComponentJsName = "ListGeneratorComponent",
                ComponentName = "Number To List",
                HelpText = "Creates a list of numbers from a single number.",
            };
        }

        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var itemsToGenerate = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);

            var outList = new List<ushort>();
            for(var i = 0; i < itemsToGenerate; i++)
            {
                outList.Add(gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug));
            }
            debug?.AppendLine($"ListGenerator => {string.Join(",", outList.Select(x => x.ToString()))}");
            return outList;
        }

    }



}
