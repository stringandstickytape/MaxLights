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
    class BpmTrigger : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                {
                    new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Multiplier", Socket = NumberSocket},
                },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "Number", Socket = NumberSocket}

                    },
                ComponentJsName = "BpmTrigger",
                ComponentName = "BPM Trigger",
                HelpText = "Returns 0 once per beat, then returns 65535 until the next beat.  For use as a trigger.",
            };
        }

        private float[,] noiseCache;
        private int positionCounter = 0;
        private Random _r;

        private long lastBeat = DateTime.UtcNow.Ticks;
        

        private bool inBeat;
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            var multiplier = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);
            if (multiplier == 0) multiplier = 1;
            var now = DateTime.UtcNow.Ticks;

            var bpm = controller.SpectrumAnalyserEngine.BPM;
            var mspb = 60 / bpm * 2*1000 / multiplier;

            var msSinceLastBeat = (now - lastBeat) / 10000;

            if (msSinceLastBeat > mspb)
            {
                lastBeat = now;
                return 0;
            }
            else return 65535;
        }

    }



}
