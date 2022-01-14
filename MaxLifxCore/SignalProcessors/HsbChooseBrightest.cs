using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class HsbChooseBrightest : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Hsb 1", Socket = HsbSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Hsb 2", Socket = HsbSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "HSB", Socket = HsbSocket},
                    },
                ComponentJsName = "HsbChooseBrightest",
                ComponentName = "HSB Choose Brightest",
                HelpText = "Compares each pair of values from two input HSB lists, and chooses the brightest.",
            };
        }


        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var hsb1 = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug);
            var hsb2 = gen[1].GetLatestHsbListValues(controller, light, OutputSocketName2[1], debug);
            
            var retVal = new List<HsbUshort>();

            var ct = Math.Min(hsb1.Count, hsb2.Count);

            for (var i = 0; i < ct; i++)
            {
                retVal.Add(hsb1[i].B > hsb2[i].B ? hsb1[i] : hsb2[i]);
            }

            return retVal;
        }
    }



}
