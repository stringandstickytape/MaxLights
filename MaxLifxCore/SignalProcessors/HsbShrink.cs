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
    class HsbShrink : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "HSB List", Socket = HsbSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Length of output  list", Socket = NumberSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "HSB list", Socket = HsbSocket}
                    },
                ComponentJsName = "HsbShrinkListComponent",
                ComponentName = "HSB Shrink List",
                HelpText = "Shrinks an HSB list by removing elements.",
            };
        }


        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var inputList = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug);
            var finalListLength = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);

            var outputList = new List<HsbUshort>();

            if (inputList.Count <= finalListLength) return inputList;
            else
            {
                float proportionToDrop = inputList.Count / (float)finalListLength;

                float pos = 0;
                while (outputList.Count() < finalListLength)
                {
                    outputList.Add(inputList[(int)(Math.Floor(pos))]);
                    pos = pos + proportionToDrop;
                }
            }

            return outputList;
        }
    }



}
