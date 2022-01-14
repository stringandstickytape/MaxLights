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
    class ExpandList : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Length of output list", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Interpolate", Socket = BooleanSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Expanded List", Socket = ListSocket}
                    },
                ComponentJsName = "ExpandListComponent",
                ComponentName = "EExpand List",
                HelpText = "Expand a list to a specific size by inserting zeroes.",
            };
        }


        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var inputList = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug);
            var finalListLength = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            var interpolate = gen[2].GetLatestBoolValue(controller, light, debug);

            var outputList = new List<ushort>();

            if (inputList.Count >= finalListLength) return inputList;
            else
            {
                float proportionToExtend = finalListLength / (float)inputList.Count;



                int pos = 0;
                while (outputList.Count() < finalListLength)
                {
                    proportionToExtend = (finalListLength - outputList.Count) / (inputList.Count - pos);

                    outputList.Add(inputList[pos]);
                    outputList.AddRange(
                        Enumerable.Repeat(
                            interpolate ? 
                            inputList[pos] : 
                            (ushort)0, 
                            (int)Math.Floor(proportionToExtend)-1).ToList());
                    
                    pos = pos + 1;
                }
            }

            return outputList;
        }


    }



}
