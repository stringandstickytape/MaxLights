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
    class AutoGain : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Previous Values to Consider", Socket = NumberSocket, DefaultValue = "10"}
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "List", Socket = ListSocket},
                    },
                ComponentJsName = "AutoGain",
                ComponentName = "Auto Gain",
                HelpText = "Adjusts values to use the full range, based on previous value minimums and maximums.",
            };
        }

        private List<ushort> currentList = null;

        private Queue<List<ushort>> oldValues = null;
        private int? _listLength = null;


        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null) { throw new NotImplementedException(); }
        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            currentList = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug);

            if (_listLength == null)
                _listLength = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);

            if (oldValues == null)
                oldValues = new Queue<List<ushort>>();

            if (oldValues.Count > _listLength)
                oldValues.Dequeue();

            oldValues.Enqueue(currentList);

            List<ushort> mins = new List<ushort>();
            List<ushort> maxes = new List<ushort>();

            for (var ctr = 0; ctr < currentList.Count; ctr++)
            {
                mins.Add(oldValues.Min(x => x[ctr]));
                maxes.Add(oldValues.Max(x => x[ctr]));
            }

            if (mins.Count > 1)
            {
                var newOut = new List<ushort>();
                for (var ctr = 0; ctr < currentList.Count; ctr++)
                {
                    var range = maxes[ctr] - mins[ctr];
                    if (range == 0) newOut.Add(currentList[ctr]);
                    else newOut.Add((ushort)((int)(currentList[ctr] - mins[ctr]) * 65535 / range));
                }
                return newOut;
            }

            return currentList;
        }


        public new void EndLoop()
        {
            base.EndLoop();
        }

    }

}