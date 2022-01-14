using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class Tracer
    {
        public double Pos { get; set; }
        public double Direction { get; internal set; }
        public ushort Hue { get; internal set; }
    }
    class HsbTracers : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Length of output list", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Tracer Hue", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Trigger when Zero", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp4", InputName = "num4", Label = "Start from Outside instead of Centre", Socket = BooleanSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "HSB list", Socket = HsbSocket}
                    },
                ComponentJsName = "HsbTracerComponent",
                ComponentName = "HSB Tracers",
                HelpText = "Generates a tracer pattern.",
            };
        }
        private List<Tracer> _tracers;

        private long startTicks = DateTime.UtcNow.Ticks;
        public SignalGenerators.ISignalGenerator Initialise(Random r, DateTime d, double interval, int nodeId) 
        {
            _tracers = new List<Tracer>();
            return base.Initialise(r, d, interval, nodeId);
        }

        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            //var inputList = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug);
            var finalListLength = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);
            var hue = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            var triggerValue = gen[2].GetLatestValue(controller, light, OutputSocketName2[2], debug);
            var startFromEnds = gen[3].GetLatestBoolValue(controller, light, debug);

            //var interpolate = gen[2].GetLatestBoolValue(controller, light, debug);

            var newTicks = DateTime.UtcNow.Ticks;
            var ticks = newTicks - startTicks;
            var msSinceLastFrame = ticks / 10000;
            var fSecSinceLastFrame = msSinceLastFrame / 1000f;
            var frac = fSecSinceLastFrame * 20;
            startTicks = newTicks;

            // Generate a new tracer?
            if (ticks > 0 && triggerValue == 0)
            {
                var newTracer = new Tracer { Hue = hue, Direction = Rnd.NextDouble() * 8 - 5 };

                if (!startFromEnds)
                    newTracer.Pos = finalListLength / 2;
                else
                {
                    if (newTracer.Direction > 0) newTracer.Pos = 0;
                    else newTracer.Pos = finalListLength - 1;
                }
                

                if (newTracer.Direction > -1) 
                    newTracer.Direction += 2;
                _tracers.Add(newTracer);
            }

            // update tracer locations
            _tracers.ForEach((x) => { x.Pos = x.Pos += x.Direction * frac; /*if (_r.Next(50) == 1) x.Direction = 0 - x.Direction;*/ });

            // remove tracers that moved out of the list
            _tracers.RemoveAll(x => x.Pos <= -1 || x.Pos >= finalListLength);

            HsbUshort[] h = new HsbUshort[finalListLength];

            _tracers.ForEach(x => { h[(int)x.Pos].H = x.Hue; h[(int)x.Pos].B = 65000; h[(int)x.Pos].S = 65535; });

            var outputList = h.ToList();

            return outputList;
        }

    }



}
