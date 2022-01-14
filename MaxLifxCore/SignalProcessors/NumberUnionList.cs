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
    class NumberUnionList : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List 1", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Lits 2", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Interleave", Socket = BooleanSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "list", Label = "List", Socket = ListSocket},

                    },
                ComponentJsName = "ListUnionComponent",
                ComponentName = "List Union",
                HelpText = "Joins two lists.",
            };
        }

        private Random _r;
        public SignalGenerators.ISignalGenerator Initialise(Random r, DateTime d, double interval,   int nodeId) { NodeId = nodeId; _r = r;  return this; }

        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }
        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var interleave = gen[2].GetLatestBoolValue(controller, light, debug);

            var l1 = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug);
            var l2 = gen[1].GetLatestListValues(controller, light, OutputSocketName2[1], debug);

            if (interleave)
            {
                var result = l1.Zip(l2, (f, s) => new[] { f, s })
                      .SelectMany(f => f);
                return result.ToList();
            }
            else return l1.Concat(l2).ToList();
        }

    }



}
