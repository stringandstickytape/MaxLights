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

    class HueAntialias : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List", Socket = ListSocket}
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "List", Socket = ListSocket}
                    },
                ComponentJsName = "HueAntialiasComponent",
                ComponentName = "Antialias Hue",
                HelpText = "Averages out the numbers in a list, wrapping betwen values 0 and 65535.",
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
            var outList = new ushort[inList.Count];

            if (inList.Count == 1) outList[0] = inList[0];
            else
                for (var ctr = 0; ctr < inList.Count; ctr++)
                {
                    if (ctr == 0)
                    {
                        var thisCol = inList[ctr];
                        var nextCol = inList[ctr + 1];

                        outList[ctr] = Utils.HueBetween(thisCol, nextCol, .5);

                    }
                    else if (ctr == inList.Count - 1)
                    {
                        outList[ctr] = inList[ctr];
                    }
                    else
                    {
                        var prev = Utils.HueBetween(inList[ctr - 1], inList[ctr], .5);
                        var next = Utils.HueBetween(inList[ctr], inList[ctr + 1], .5);
                        var thisNew = Utils.HueBetween(prev, next, .5);
                        outList[ctr] = thisNew;
                    }
                }

            return outList.ToList();
        }
    }
}