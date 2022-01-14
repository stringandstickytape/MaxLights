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
    class HsbExpand : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "HSB List", Socket = HsbSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Length of output list", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Interpolate", Socket = BooleanSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "HSB list", Socket = HsbSocket}
                    },
                ComponentJsName = "HsbExpandListComponent",
                ComponentName = "HSB Expand List",
                HelpText = "Expands an HSB list by adding elements.",
            };
        }


        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var inputList = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug);
            var finalListLength = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            var interpolate = gen[2].GetLatestBoolValue(controller, light, debug);

            var outputList = new List<HsbUshort>();

            if (inputList.Count >= finalListLength) return inputList;
            else
            {
                if (interpolate)
                {
                    var l1step = (finalListLength - 1) / (inputList.Count - 1f);
                    var el1 = Enumerable.Range(0, inputList.Count).Select(x => (double)(x * l1step));

                    var lh = MathNet.Numerics.Interpolate.Common(el1, inputList.Select(x => (double)x.H));
                    var ls = MathNet.Numerics.Interpolate.Common(el1, inputList.Select(x => (double)x.S));
                    var lb = MathNet.Numerics.Interpolate.Common(el1, inputList.Select(x => (double)x.B));

                    outputList = Enumerable.Range(0, finalListLength).Select(x =>
                        new HsbUshort
                        {
                            H = (ushort)lh.Interpolate(x),
                            S = (ushort)ls.Interpolate(x),
                            B = (ushort)lb.Interpolate(x),
                        }).ToList();


                }
                else
                {
                    float proportionToExtend;

                    int pos = 0;
                    while (outputList.Count() < finalListLength)
                    {
                        proportionToExtend = (finalListLength - outputList.Count) / (inputList.Count - pos);

                        outputList.Add(inputList[pos]);
                        outputList.AddRange(Enumerable.Repeat(interpolate ? inputList[pos] : new HsbUshort(), (int)Math.Floor(proportionToExtend) - 1).ToList());

                        pos = pos + 1;
                    }
                }
            }

            return outputList;
        }

    }



}
