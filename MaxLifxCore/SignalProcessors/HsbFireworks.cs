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
    class FireworkParticle
    {
        public double Pos { get; set; }
        public double Direction { get; internal set; }
        public ushort Hue { get; internal set; }

        public ushort Brightness { get; internal set; }
    }
    class HsbFireworks : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Length of output list", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Tracer Hue", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Trigger when ", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp4", InputName = "num4", Label = "is greater than or equal to", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp5", InputName = "num5", Label = "Start from Outside instead of Centre", Socket = BooleanSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "HSB list", Socket = HsbSocket}
                    },
                ComponentJsName = "HsbFireworksComponent",
                ComponentName = "HSB Fireworks",
                HelpText = "Generates a firework pattern.",
            };
        }
        private List<FireworkParticle> _fireworkParticles;

        private long startTicks = DateTime.UtcNow.Ticks;
        public SignalGenerators.ISignalGenerator Initialise(Random r, DateTime d, double interval, int nodeId) 
        {
            _fireworkParticles = new List<FireworkParticle>();
            return base.Initialise(r, d, interval, nodeId);
        }

        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            //var inputList = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug);
            var finalListLength = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);
            var hue = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            var triggerValue = gen[2].GetLatestValue(controller, light, OutputSocketName2[2], debug);
            var triggerComparisonValue = gen[3].GetLatestValue(controller, light, OutputSocketName2[3], debug);
            var startFromEnds = gen[4].GetLatestBoolValue(controller, light, debug);

            //var interpolate = gen[2].GetLatestBoolValue(controller, light, debug);

            var newTicks = DateTime.UtcNow.Ticks;
            var ticks = newTicks - startTicks;
            var msSinceLastFrame = ticks / 10000;
            var fSecSinceLastFrame = msSinceLastFrame / 1000f;
            var frac = fSecSinceLastFrame * 20;
            startTicks = newTicks;

            // Generate a new tracer?
            if (ticks > 0 && triggerValue >= triggerComparisonValue)
            {

                var centreOfParticleBurst = Rnd.Next(0, finalListLength);
                var numberOfParticlesOnEachSide = Rnd.Next(10,15);
                
                for(var p = 0; p < numberOfParticlesOnEachSide; p++)
                {
                    var d = Rnd.Next(1, 20);
                    _fireworkParticles.Add(new FireworkParticle { Hue = hue, Direction = d, Pos = centreOfParticleBurst, Brightness = 65535 });
                    _fireworkParticles.Add(new FireworkParticle { Hue = hue, Direction = 0-d, Pos = centreOfParticleBurst, Brightness = 65535 });
                }



                //var newTracer = new FireworkParticle { Hue = hue, Direction = Rnd.NextDouble() * 8 - 5 };
                //
                //if (!startFromEnds)
                //    newTracer.Pos = finalListLength / 2;
                //else
                //{
                //    if (newTracer.Direction > 0) newTracer.Pos = 0;
                //    else newTracer.Pos = finalListLength - 1;
                //}
                //
                //
                //if (newTracer.Direction > -1) 
                //    newTracer.Direction += 2;
                //_fireworkParticles.Add(newTracer);
            }

            // update tracer locations
            _fireworkParticles.ForEach((x) => { x.Pos = x.Pos += x.Direction * frac; x.Direction = x.Direction * 9 / 10; });

            // fade out particles that have stopped moving
            _fireworkParticles.Where(x => Math.Abs(x.Direction) < .05f).ToList().ForEach(x => {
                x.Hue = (ushort)(x.Hue*.9);
                x.Brightness = Rnd.Next(0,4) == 3
                ? (x.Brightness * 1.1f > 65535 ? (ushort)65535 : (ushort)(x.Brightness * 1.3f))
                : (ushort)(x.Brightness*.8f); 
            });

            // remove tracers that moved out of the list
            _fireworkParticles.RemoveAll(x => x.Pos <= -1 || x.Pos >= finalListLength || x.Brightness < .1f);

            HsbUshort[] h = new HsbUshort[finalListLength];

            _fireworkParticles.ForEach(x => { h[(int)x.Pos].H = x.Hue; h[(int)x.Pos].B = x.Brightness; h[(int)x.Pos].S = 65535; });

            var outputList = h.ToList();

            return outputList;
        }

    }



}
