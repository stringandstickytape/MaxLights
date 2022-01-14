
using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class Plasma : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "# of values to generate", Socket = NumberSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "HSB", Socket = HsbSocket}

                    },
                ComponentJsName = "PlasmaGenerator",
                ComponentName = "Plasma Generator",
                HelpText = "Creates plasma patterns.",
            };
        }
        private Queue<ushort> _buffer;
        private ushort _bufferCapacity;
        private ScreenCaptureEngine _screenCaptureEngine;



        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            //List<ushort> hues, sats, bris;
            var ct = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);

            byte thisPhase = beatsin8(6, -64, 64);                       // Setting phase change for a couple of waves.
            byte thatPhase = beatsin8(7, -64, 64);
            byte otherPhase = beatsin8(10, -64, 64);

            var SEGMENTspeed = 192;
            var SEGMENTintensity = 32;

            var outval = new List<HsbUshort>();

            for (int i = 0; i < ct; i++)
            {   // For each of the LED's in the strand, set color &  brightness based on a wave as follows:
                var w1int = (i * (1 + (int)(3 * (SEGMENTspeed >> 5)))) + (thisPhase) & 0xFF;
                var w2int = (i * (1 + (int)(2 * (SEGMENTspeed >> 5)))) + (thatPhase) & 0xFF;
                var w3int = (i * (1 + (int)(3.5 * (SEGMENTspeed >> 5)))) + (otherPhase) & 0xFF;
                var w1in = (byte)(w1int);
                var w2in = (byte)(w2int);
                var w3in = (byte)(w3int);

                byte colorIndex =

                    (byte)
                    (

                        cubicwave8(w1in) / 3  // factor=23 // Create a wave and add a phase change and add another wave with its own phase change.
                        +
                        sin8(w2in) / 3  // factor=15 // Hey, you can even change the frequencies if you wish.
                        + 
                        cos8(w3in) / 3
                    );
               // Debug.WriteLine(colorIndex);
                byte thisBright = qsub8(colorIndex, beatsin8(6, 0, (byte)((255 - SEGMENTintensity) | 0x01)));
                //Debug.WriteLine(thisBright);
                (byte, byte, byte) color = ColorFromPalette(colorIndex, thisBright, "LINEARBLEND");

                Utils.ColorToHSV2(color.Item1, color.Item2, color.Item3, out double hue, out double saturation, out double value);

                outval.Add(new HsbUshort { H = (ushort)(hue * 182.0444), S = (ushort)(saturation * 65535), B = (ushort)(value * 65535) });

                //setPixelColor(i, color.red, color.green, color.blue);
            }



            return outval;
        }

        private byte lsrX4(byte dividend)
        {
            return dividend >>= 4;
        }

        private List<System.Drawing.Color> pal = new List<System.Drawing.Color>() { System.Drawing.Color.Red, System.Drawing.Color.FromArgb(255,0,255,0), System.Drawing.Color.Blue };

        public (byte,byte,byte) ColorFromPalette( byte index, byte brightness, string blendType)
        {
            var indicesPerColor = 256f / (pal.Count-1);
            var colIndex1 = (int)Math.Floor(index / indicesPerColor);
            var col1 = pal[colIndex1];
            var col2 = pal[colIndex1 + 1];
            var interp = (index - colIndex1 * indicesPerColor) / indicesPerColor;

            byte red1 = (byte)(col1.R * (1 - interp) + col2.R * interp);
            byte green1 = (byte)(col1.G * (1 - interp) + col2.G * interp);
            byte blue1 = (byte)(col1.B * (1 - interp) + col2.B * interp);

            if( brightness != 255) {
                if( brightness > 0 ) {
                    brightness++; // adjust for rounding
                    // Now, since brightness is nonzero, we don't need the full scale8_video logic;
                    // we can just to scale8 and then add one (unless scale8 fixed) to all nonzero inputs.
                    if( red1 > 0 )   {
                        red1 = scale8( red1, brightness);
                    }
                    if( green1 > 0  ) {
                        green1 = scale8( green1, brightness);
                    }
                    if( blue1 > 0 )  {
                        blue1 = scale8( blue1, brightness);
                    }
                } else {
                    red1 = 0;
                    green1 = 0;
                    blue1 = 0;
                }
            }
    
            return (red1, green1, blue1);
        }

        byte qsub8(byte i, byte j)
        {
            int t = i - j;
            if (t < 0) t = 0;
            return (byte)t;
        }

        private byte cos8(byte theta)
        {


            //var r = ((Math.Cos((theta / 128f - 1) * Math.PI))+1)*255;
            //Debug.WriteLine($"{theta} => {r}");
            return sin8((byte)(theta+64));
        }

        private byte sin8(byte theta)
        {
            return (byte)(Math.Sin((theta / 128f)-1 * Math.PI)*255);
        }


        private byte beatsin8(int beats_per_minute, int lowest = 0, byte highest = 255,
                            int timebase = 0, byte phase_offset = 0)
        {
            byte beat = beat8(beats_per_minute, timebase);



            var beatf = (beat - 128) / 128f * Math.PI;
            var beatf2 = (Math.Sin(beatf) + 1) * 127.5;


            //if (beats_per_minute == 6 && lowest == 0)
            //    Debug.WriteLine(beatf2);
            byte beatsin = (byte)(beatf2);
            byte rangewidth = (byte)(highest - lowest);
            byte scaledbeat = scale8(beatsin, rangewidth);
            byte result = (byte)(lowest + scaledbeat);
            return result;
        }

        private byte cubicwave8(byte inb)
        {
            return ease8InOutCubic(triwave8(inb));
        }

        private byte scale8(byte i, byte scale)
        {
            return (byte)(i*scale/256);
        }

        private byte triwave8(byte inb)
        {
            if ( inb > 127 ) {
                inb = ((byte)(((byte)255)-inb));
            }
            byte outb = (byte)(inb << 1);
            return outb;
        }

        private byte ease8InOutCubic(byte i)
        {
            byte ii = scale8(i, i);
            byte iii = scale8(ii, i);

            int r1 = (3 * ii) - (2 * iii);
            
            /* the code generated for the above *'s automatically
               cleans up R1, so there's no need to explicitily call
               cleanup_R1(); */

            byte result = (byte)r1;

            // if we got "256", return 255:
            if ((r1 > 255))
            {
                result = 255;
            }
            return result;
        }



        private byte beat8(int beats_per_minute, int timebase = 0)
        {
            return (byte)(beat16(beats_per_minute, timebase) >> 8);
        }

        private int beat16(int beats_per_minute, int timebase = 0)
        {
            // Convert simple 8-bit BPM's to full Q8.8 accum88's if needed
            if (beats_per_minute < 256) beats_per_minute <<= 8;
            return beat88(beats_per_minute, timebase);
        }

        private int beat88(int beats_per_minute_88, int timebase = 0)
        {
            // BPM is 'beats per minute', or 'beats per 60000ms'.
            // To avoid using the (slower) division operator, we
            // want to convert 'beats per 60000ms' to 'beats per 65536ms',
            // and then use a simple, fast bit-shift to divide by 65536.
            //
            // The ratio 65536:60000 is 279.620266667:256; we'll call it 280:256.
            // The conversion is accurate to about 0.05%, more or less,
            // e.g. if you ask for "120 BPM", you'll get about "119.93".
            return (int)((((DateTime.UtcNow.Ticks / 10000) - timebase) * beats_per_minute_88 * 280) / 65536);
        }


        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

 


        public void EndLoop()
        {
            if (_screenCaptureEngine != null)
                _screenCaptureEngine.ClearFrame();
        }
    }



}
