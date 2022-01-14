using ColorThiefDotNet;

using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class ImageToPaletteComponent : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Filename", Socket = StringSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Maximum # of colours to generate", Socket = NumberSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "Hue", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out2", OutputName = "out2", Label = "Saturation", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out3", OutputName = "out3", Label = "Brightness", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out4", OutputName = "out4", Label = "HSB", Socket = HsbSocket}

                    },
                ComponentJsName = "ImageToPaletteComponent",
                ComponentName = "Image to Palette",
                HelpText = "Creates a palette of colours from a single image.",
            };
        }
        private ColorThief colorThief;
        private List<QuantizedColor> palette;

        private List<ushort> outputH = new List<ushort>();
        private List<ushort> outputS = new List<ushort>();
        private List<ushort> outputB = new List<ushort>();

        private Bitmap rescale = new Bitmap(64, 64);
        private Graphics graph;
        private Bitmap source;

        public SignalGenerators.ISignalGenerator Initialise(Random r, DateTime d, double interval, int nodeId)
        {
            NodeId = nodeId; graph = Graphics.FromImage(rescale); return this;
        }

        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            GetColorThiefHSVs(controller, light, debug);

            switch (outputSocketName)
            {
                case "out2": // s
                    return outputS;
                case "out1": // h
                    return outputH;
                case "out3": // v
                    return outputB;
                default:
                    throw new NotImplementedException();
            }
        }

        private void GetColorThiefHSVs(AppController controller, Light light, StringBuilder debug)
        {
            if (colorThief == null) colorThief = new ColorThief();

            var filename = gen[0].GetLatestStringValue(controller, light, debug);
            var colsToGen = gen[1].GetLatestValue(controller, light, OutputSocketName2[1]);

            if (source == null)
            {
                source = new Bitmap(filename);
                graph.InterpolationMode = InterpolationMode.High;
                graph.CompositingQuality = CompositingQuality.HighQuality;
                graph.SmoothingMode = SmoothingMode.AntiAlias;
                graph.DrawImage(source, 0, 0, 64, 64);

                if (colsToGen < 3)
                    palette = colorThief.GetPalette(rescale);
                else palette = colorThief.GetPalette(rescale, colsToGen);
            }

            var cols = palette.Select(x => System.Drawing.Color.FromArgb(x.Color.R, x.Color.G, x.Color.B));

            outputH = new List<ushort>();
            outputS = new List<ushort>();
            outputB = new List<ushort>();

            foreach (var col in cols)
            {
                Utils.ColorToHSV(col, out var hue, out var sat, out var val);
                outputH.Add((ushort)(hue * 65535));
                outputS.Add((ushort)(sat * 65535));
                outputB.Add((ushort)(val * 65535));
            }
        }

        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            GetColorThiefHSVs(controller, light, debug);

            var vals = Math.Min(Math.Min(outputH.Count, outputS.Count), outputB.Count);

            return Enumerable.Range(0, vals).Select(x => new HsbUshort { H = outputH[x], S = outputS[x], B = outputB[x] }).ToList();
        }


        public void EndLoop()
        {
            base.EndLoop();
        }
    }



}
