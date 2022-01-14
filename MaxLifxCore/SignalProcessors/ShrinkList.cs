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
    class ShrinkList : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Length of output  list", Socket = NumberSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Shrunk list", Socket = ListSocket}
                    },
                ComponentJsName = "ShrinkListComponent",
                ComponentName = "Shrink List",
                HelpText = "Shrinks a list to a specific size by removing elements.",
            };
        }


        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var inputList = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug);
            var finalListLength = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);

            var outputList = new List<ushort>();

            if (inputList.Count <= finalListLength) return inputList;
            else
            {
                float proportionToDrop = inputList.Count / (float)finalListLength;

                

                float pos = 0;
                while(outputList.Count() < finalListLength)
                {
                    outputList.Add(inputList[(int)(Math.Floor(pos))]);
                    pos = pos + proportionToDrop;
                }
            }

            return outputList;
        }

    }



}
