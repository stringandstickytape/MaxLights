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
    class Add : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Number 1", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Number 2", Socket = NumberSocket}
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Number", Socket = NumberSocket}
                    },
                ComponentJsName = "NumberAddComponent",
                ComponentName = "Add Numbers",
                HelpText = "Adds two numbers.",
            };
        }

        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            var retVal = (ushort)(gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug) + gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug));
            debug?.AppendLine($"Add => {retVal}");
            return retVal;
        }
        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var divisor = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            var inputList = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug);
            return inputList.Select(x => (ushort)(x + divisor)).ToList();
        }


    }



}
