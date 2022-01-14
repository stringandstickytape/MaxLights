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
    class HsbTranslate : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Number List to Translate", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Palette", Socket = HsbSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Translated HSB list", Socket = HsbSocket}
                    },
                ComponentJsName = "HsbTranslateListComponent",
                ComponentName = "Number List Translate to HSB",
                HelpText = "Translates a list of numbers according to a list of HSBs.",
            };
        }


        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var listToTranslate = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug);
            var palette = gen[1].GetLatestHsbListValues(controller, light, OutputSocketName2[1], debug);

            var outputList = new List<HsbUshort>();

            var l1step = (65536 - 1) / (palette.Count - 1f);
            var el1 = Enumerable.Range(0, palette.Count).Select(x => (double)(x * l1step));

            var lh = MathNet.Numerics.Interpolate.Common(el1, palette.Select(x => (double)x.H));
            var ls = MathNet.Numerics.Interpolate.Common(el1, palette.Select(x => (double)x.S));
            var lb = MathNet.Numerics.Interpolate.Common(el1, palette.Select(x => (double)x.B));

            outputList = listToTranslate.Select(x =>
                new HsbUshort
                {
                    H = (ushort)lh.Interpolate(x),
                    S = (ushort)ls.Interpolate(x),
                    B = (ushort)lb.Interpolate(x),
                }).ToList();

            /*var finalListLength = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            var interpolate = gen[2].GetLatestBoolValue(controller, light, debug);

            

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
            */

            return outputList;
        }

    }



}
