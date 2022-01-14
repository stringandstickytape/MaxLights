using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class SimplexGenerator : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "# of values to generate", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Scaling", Socket = NumberSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "List", Socket = ListSocket}

                    },
                ComponentJsName = "SimplexGenerator",
                ComponentName = "Simplex Noise Generator",
                HelpText = "Creates simplex noise patterns.",
            };
        }
        private float[,] noiseCache;
        private int positionCounter = 0;
        private Random _r;
        public SignalGenerators.ISignalGenerator Initialise(Random r, DateTime d, double interval, int nodeId) { _r = r; NodeId = nodeId; return this; }


        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var ct = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);
            var scaling = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);

            if (scaling < 1) scaling = 1;

            if (noiseCache == null)
            {
                SimplexNoise.Noise.Seed = _r.Next();
                noiseCache = SimplexNoise.Noise.Calc2D(ct, 1000, 1f/scaling);
            }




            var outval = new List<ushort>();

            for (var i = 0; i < ct; i++)
                outval.Add((ushort)(noiseCache[i,positionCounter] * 256));

            positionCounter++;
            if (positionCounter == noiseCache.GetLength(1))
            {
                positionCounter = 0;
                noiseCache = null;
            }

            return outval;

        }

        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

        public void EndLoop()
        {
        }

    }



}
