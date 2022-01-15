
using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class ImageCapture : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Image Filename", Socket = StringSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Row to take colours from", Socket = NumberSocket}
                    },
                Outputs = new List<DiagramOutput>()
                    {

                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Hue", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out2", OutputName = "num1", Label = "Saturation", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out3", OutputName = "num2", Label = "Brightness", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out4", OutputName = "num3", Label = "HSB", Socket = HsbSocket},
                    },
                ComponentJsName = "ImageCapture",
                ComponentName = "Image Capture",
                HelpText = "Takes colour information from a single row of a single image. Bounces back up the image if you read past the last line of the image.",
            };
        }
        private Queue<ushort> _buffer;
        private ushort _bufferCapacity;
        private Bitmap frame = null;
        private int frameCtr = 0;


        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            List<ushort> hues, sats, bris;
            GetHSBs(controller, light, debug, out hues, out sats, out bris);

            switch (outputSocketName)
            {
                case "num1": // sat
                    return sats;
                case "num2": // bri
                    return bris;
                case "num": // hue
                    return hues;
                default:
                    throw new NotImplementedException();
            }
        }

        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            List<HsbUshort> retVal = new List<HsbUshort>();
            GetHSBs(controller, light, debug, out var hues, out var sats, out var bris);

            for (var x = 0; x < hues.Count; x++)
                retVal.Add(new HsbUshort { H = hues[x], S = sats[x], B = bris[x] });

            return retVal;
        }

        private void GetHSBs(AppController controller, Light light, StringBuilder debug, out List<ushort> hues, out List<ushort> sats, out List<ushort> bris)
        {
            if (frame == null)
            {
                var filename = gen[0].GetLatestStringValue(controller, light, debug);
                frame = new Bitmap(filename);
            }

            var rowToUse = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);

            var r2 = rowToUse % (frame.Height * 2);
            if (r2 >= frame.Height)
                r2 = frame.Height - (rowToUse - (frame.Height-1));


            hues = new List<ushort>();
            sats = new List<ushort>();
            bris = new List<ushort>();
            for (var ctr = 0; ctr < frame.Width; ctr++)
            {
                var rect = new System.Drawing.Rectangle(ctr, r2, 1, 1);

                BitmapData bmd = frame.LockBits(rect,
                    System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    frame.PixelFormat);

                var colour = ScreenCaptureEngine.GetColourForRectFromBitmapData(rect, bmd, frame.PixelFormat, 1, 1);

                frame.UnlockBits(bmd);

                if (colour == null)
                {
                    hues.Add((ushort)0);
                    sats.Add((ushort)0);
                    bris.Add((ushort)0);
                }
                else
                {
                    Utils.ColorToHSV(colour.Value, out double hue, out double saturation, out double value);

                    hues.Add((ushort)(hue * 182.0444));
                    sats.Add((ushort)(saturation * 65535));
                    bris.Add((ushort)(value * 65535));
                }
            }
        }


        public void EndLoop()
        {
            base.EndLoop();
        }
    }



}
