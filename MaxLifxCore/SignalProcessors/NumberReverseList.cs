
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
    class NumberReverseList : SignalProcessorBase, ISignalGenerator
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
                ComponentJsName = "NumberReverseListComponent",
                ComponentName = "Reverse List",
                HelpText = "Reverses the order of numbers in a list.",
            };
        }
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
            //var retVal = (ushort)(gen[0].GetLatestValue(controller, debug) / 2);
            //debug?.AppendLine($"Halve => {retVal}");
            //return retVal;
        }
        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var l = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug);
            l.Reverse();
            return l;

        }
    }

}