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


    class Antialias : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Iterations", Socket = NumberSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "List", Socket = ListSocket}
                    },
                ComponentJsName = "AntialiasComponent",
                ComponentName = "Antialias",
                HelpText = "Averages out the numbers in a list.",
            };
        }

        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            var retVal = (ushort)(gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug) / 2);
            debug?.AppendLine($"Antialias => {retVal}");
            return retVal;
        }
        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var inList = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug);
            var iter = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            var outList = new ushort[inList.Count];

            if (inList.Count == 1) outList[0] = inList[0];
            else
            {
                for (var itera = 0; itera < iter; itera++)
                {
                    for (var ctr = 0; ctr < inList.Count; ctr++)
                    {
                        if (ctr == 0) { outList[ctr] = (ushort)(((inList[ctr] * 2) + inList[ctr + 1]) / 3); }
                        else if (ctr == inList.Count - 1) { outList[ctr] = (ushort)(((inList[ctr] * 2) + inList[ctr - 1]) / 3); }
                        else { outList[ctr] = (ushort)(((inList[ctr] * 2) + inList[ctr + 1] + inList[ctr - 1]) / 4); }
                    }

                    inList = new List<ushort>((ushort[])(outList.Clone()));
                }
            }

            return outList.ToList();
        }

    }

}