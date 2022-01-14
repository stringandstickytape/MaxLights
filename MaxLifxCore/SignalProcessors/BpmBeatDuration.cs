using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class BpmBeatDuration : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                {
                },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "Number", Socket = NumberSocket}

                    },
                ComponentJsName = "BpmBeatDuration",
                ComponentName = "BPM Beat Duration (ms)",
                HelpText = "Returns the current BPM beat duration in milliseconds.",
            };
        }

        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {

            return (ushort)(60 / controller.SpectrumAnalyserEngine.BPM * 1000);
        }
    }
}
