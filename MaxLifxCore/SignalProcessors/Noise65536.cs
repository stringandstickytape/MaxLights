
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
    class Noise65536 : SignalProcessorBase, ISignalGenerator
    {
        ushort? currentVal = null;


        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Lower Limit", Socket = NumberSocket },
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Upper Limit", Socket = NumberSocket },
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out", OutputName = "num", Label = "Number", Socket = NumberSocket}
                    },
                ComponentJsName = "NoiseNumberComponent",
                ComponentName = "Random",
                HelpText = "Generates a random value between two values (and always between 0-65535).",
            };
        }
        private Random _r;
        //public SignalGenerators.ISignalGenerator Initialise(Random r, DateTime d, double interval, int nodeId) { NodeId = nodeId; _r = r; return this; }
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            if (currentVal == null)
            {
                var lower = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);
                var upper = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug) + 1;
                currentVal = (ushort)Rnd.Next(lower, upper);
                debug?.AppendLine($"Random => {currentVal}");
            }
            return currentVal.Value;
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }
        public void EndLoop()
        {
            currentVal = null;
        }
    }

}