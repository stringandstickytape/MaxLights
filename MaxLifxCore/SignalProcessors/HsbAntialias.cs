using ColorThiefDotNet;
using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using NAudio.CoreAudioApi;
using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MaxLifxCore.SignalProcessors
{

    class HsbAntialias : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "HSB", Socket = HsbSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Iterations", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Don\\'t smooth H", Socket = BooleanSocket},
                        new DiagramInput { JsToken = "inp4", InputName = "num4", Label = "Don\\'t smooth S", Socket = BooleanSocket},
                        new DiagramInput { JsToken = "inp5", InputName = "num5", Label = "Don\\'t smooth B", Socket = BooleanSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "HSB", Socket = HsbSocket}
                    },
                ComponentJsName = "HSBAntialiasComponent",
                ComponentName = "HSB Antialias",
                HelpText = "Antialises the components of a list of HSBs.",
            };
        }

        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var inList = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug).ToArray();
            var iter = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            var dontSmoothH = gen[2].GetLatestBoolValue(controller, light, debug);
            var dontSmoothS = gen[3].GetLatestBoolValue(controller, light, debug);
            var dontSmoothB = gen[4].GetLatestBoolValue(controller, light, debug);

            var outList = new HsbUshort[inList.Length];

            if (inList.Length == 1) outList[0] = inList[0];
            else if (iter == 0)
            {
                outList = inList;
            }
            else
            {
                for (var itera = 0; itera < iter; itera++)
                {
                    for (var ctr = 0; ctr < inList.Length; ctr++)
                    {
                        var oldH = inList[ctr].H;
                        var oldS = inList[ctr].S;
                        var oldB = inList[ctr].B;

                        var newHsb = new HsbUshort();

                        if (ctr == 0)
                        {
                            newHsb.H = dontSmoothH ? oldH : Utils.HueBetween(oldH, inList[ctr + 1].H, .5);
                            newHsb.S = dontSmoothS ? oldS : (ushort)(((oldS * 2) + inList[ctr + 1].S) / 3);
                            newHsb.B = dontSmoothB ? oldB : (ushort)(((oldB * 2) + inList[ctr + 1].B) / 3);
                        }
                        else if (ctr == inList.Length - 1)
                        {
                            newHsb.H = dontSmoothH ? oldH : Utils.HueBetween(oldH, inList[ctr - 1].H, .5);
                            newHsb.S = dontSmoothS ? oldS : (ushort)(((oldS * 2) + inList[ctr - 1].S) / 3);
                            newHsb.B = dontSmoothB ? oldB : (ushort)(((oldB * 2) + inList[ctr - 1].B) / 3);
                        }
                        else
                        {
                            newHsb.H = dontSmoothH ? oldH : Utils.HueBetween(Utils.HueBetween(oldH, inList[ctr - 1].H, .5), Utils.HueBetween(oldH, inList[ctr + 1].H, .5), .5);
                            newHsb.S = dontSmoothS ? oldS : (ushort)(((oldS * 2) + inList[ctr - 1].S + inList[ctr + 1].S) >> 2);
                            newHsb.B = dontSmoothB ? oldB : (ushort)(((oldB * 2) + inList[ctr - 1].B + inList[ctr + 1].B) >> 2);

                        }
                        outList[ctr] = newHsb;
                    }
                    Array.Copy(outList, inList, outList.Length);
                   
                }
            }

            return outList.ToList();
        }

    }
}