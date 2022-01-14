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
    class BpmGenerator : SignalProcessorBase, ISignalGenerator
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
                ComponentJsName = "BpmGenerator",
                ComponentName = "BPM Generator",
                HelpText = "Creates a linear value from 0 to 65535 related to BPM.",
            };
        }

        private float[,] noiseCache;
        private int positionCounter = 0;
        private Random _r;

        private long startBeatTicks, endBeatTicks;

        private bool inBeat;
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            var now = DateTime.UtcNow.Ticks;

            if (inBeat && endBeatTicks < now)
            {
                inBeat = false;
            }

            if (inBeat)
            {
                var diff = endBeatTicks - startBeatTicks;
                var currdiff = now - startBeatTicks;

                var ratio = 1-((float)currdiff / diff);
                var ratioUshort = (ushort)(ratio * 65535);
                //Debug.WriteLine(ratioUshort);
                return ratioUshort;
            }
            else
            {
                var bpm = controller.SpectrumAnalyserEngine.BPM;

                if (bpm == 0) bpm = 120;
                Debug.WriteLine(bpm);

                var spb = 60 / bpm * 2;

                startBeatTicks = DateTime.UtcNow.Ticks;
                endBeatTicks = startBeatTicks + (long)(spb * 10000000);
                inBeat = true;

                return 65535;
            }

            return 0;
        }

    }



}
