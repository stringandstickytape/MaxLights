

using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class VideoCapture : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "X", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Y", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Width", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp4", InputName = "num4", Label = "Height", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp5", InputName = "num5", Label = "X step", Socket = FloatSocket},
                        new DiagramInput { JsToken = "inp6", InputName = "num6", Label = "Y step", Socket = FloatSocket},
                        new DiagramInput { JsToken = "inp7", InputName = "num7", Label = "Items to Generate", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp8", InputName = "num8", Label = "Video Filename", Socket = StringSocket},
                        new DiagramInput { JsToken = "inp9", InputName = "num9", Label = "Start Frame", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp10", InputName = "num10", Label = "End Frame", Socket = NumberSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "Hue", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out2", OutputName = "out2", Label = "Saturation", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out3", OutputName = "out3", Label = "Brightness", Socket = ListSocket}
                    },
                PreCustomControlJS = ".addControl(new DefaultLayoutControl(this.editor, 'defaultLayout'))",
                CustomControlJS = ".addControl(new String2Control(this.editor, 'summaryInfo', true))", // the "0x0 to 100x100" bit
                CustomWorker = @"     
        var x =  parseInt(inputs.num1.length?inputs.num1[0]:node.data['X']);
        var y =  parseInt(inputs.num2.length?inputs.num2[0]:node.data['Y']);
        var w =  parseInt(inputs.num3.length?inputs.num3[0]:node.data['Width']);
        var h =  parseInt(inputs.num4.length?inputs.num4[0]:node.data['Height']);
        var sx = parseInt(inputs.num5.length?inputs.num5[0]:node.data['X step']);
        var sy = parseInt(inputs.num6.length?inputs.num6[0]:node.data['Y step']);
        var g =  parseInt(inputs.num7.length?inputs.num7[0]:node.data['Items to Generate']);

        var outString = '';

        if(x===undefined||y===undefined||w===undefined||h===undefined||sx===undefined||sy===undefined||g===undefined) {
            outString = 'Set inputs...';
        }   
        else {
            var l= Math.round(x - (w/2));
            var t= Math.round(y - (h/2));
            var r =Math.round( x + sx * (g - 1) + (w/2));
            var b =Math.round( y + sy * (g - 1) + (h/2));

            outString = '' + l + ',' + t + ' to ' + r + ',' + b;
        }
        
        this.editor.nodes.find(n => n.id == node.id).controls.get('summaryInfo').setValue(outString);
        ",
                ComponentJsName = "VideoCaptureComponent",
                ComponentName = "Video Capture",
                HelpText = "Captures colours from a video file.",
            };
        }
        private Queue<ushort> _buffer;
        private ushort _bufferCapacity;
        private ScreenCaptureEngine _screenCaptureEngine;
        private Bitmap bitmap = null;
        private int frameCtr = 0;

        public MediaFile CurrentMediaFile { get; private set; }
        public bool MediaFileStartSkipCompleted { get; set; }

        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var imageData = GetNextFrame(controller, light, debug);

            var itemsToGenerate = gen[6].GetLatestValue(controller, light, OutputSocketName2[6], debug);

            if (imageData.Data.Length == 0)
            {
                return Enumerable.Repeat((ushort)0, itemsToGenerate).ToList();
            }

            GetHsbsFromFrame(controller, light, debug, out var hues, out var sats, out var bris, itemsToGenerate, imageData);

            switch (outputSocketName)
            {
                case "out2": // sat
                    return sats;
                case "out3": // bri
                    return bris;
                case "out1": // hue
                    return hues;
                default:
                    throw new NotImplementedException();
            }
        }



        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var imageData = GetNextFrame(controller, light, debug);

            var itemsToGenerate = gen[6].GetLatestValue(controller, light, OutputSocketName2[6], debug);

            if (bitmap == null)
            {
                return Enumerable.Repeat(new HsbUshort(), itemsToGenerate).ToList();
            }

            GetHsbsFromFrame(controller, light, debug, out var hues, out var sats, out var bris, itemsToGenerate, imageData);

            var num = Math.Min(Math.Min(hues.Count, sats.Count), bris.Count);

            return Enumerable.Range(0, num).Select(x => new HsbUshort { H = hues[x], S = sats[x], B = bris[x] }).ToList();
        }

        private void GetHsbsFromFrame(AppController controller, Light light, StringBuilder debug, out List<ushort> hues, out List<ushort> sats, out List<ushort> bris, ushort itemsToGenerate, ImageData imageData)
        {
            hues = new List<ushort>();
            sats = new List<ushort>();
            bris = new List<ushort>();

            var width = gen[2].GetLatestValue(controller, light, OutputSocketName2[2], debug);
            var height = gen[3].GetLatestValue(controller, light, OutputSocketName2[3], debug);

            float x = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);
            float y = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);

            var xStep = gen[4].GetLatestFloatValue(controller, light, OutputSocketName2[4], debug);
            var yStep = gen[5].GetLatestFloatValue(controller, light, OutputSocketName2[5], debug);

            for (var ctr = 0; ctr < itemsToGenerate; ctr++)
            {
                var rect = new System.Drawing.Rectangle((int)x, (int)y, width, height);
                
                //BitmapData bmd = bitmap.LockBits(rect,
                //    System.Drawing.Imaging.ImageLockMode.ReadWrite,
                //    bitmap.PixelFormat);

                var colour = GetColourForRectFromBitmapData(rect, imageData);

                //bitmap.UnlockBits(bmd);

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

                x += xStep;
                y += yStep;
            }
        }



        private ImageData GetNextFrame(AppController controller, Light light, StringBuilder debug)
        {
            ImageData imageData;
            var lastFrameNo = gen[9].GetLatestValue(controller, light, OutputSocketName2[9], null);

            if (lastFrameNo > 0 && CurrentMediaFile.Video.Position.TotalSeconds * CurrentMediaFile.Video.Info.AvgFrameRate > lastFrameNo)
                MediaFileStartSkipCompleted = false;

            if (!MediaFileStartSkipCompleted)
            {
                if(CurrentMediaFile.Video.TryGetFrame(TimeSpan.FromSeconds((float)(gen[8].GetLatestValue(controller, light, OutputSocketName2[8], null))/ ((float)CurrentMediaFile.Video.Info.AvgFrameRate)), out imageData))
                {
                    MediaFileStartSkipCompleted = true;
                    return imageData;
                }
                
            }
            
            if (CurrentMediaFile.Video.TryGetNextFrame(out imageData))
            {
                return imageData;
            }
            MediaFileStartSkipCompleted = false;

            return new ImageData();

        }

        public void SetupGenerator(DiagramNode node, Diagram diagram, AppController controller)
        {
            var filename = gen[7].GetLatestStringValue(controller, null);
            CurrentMediaFile = MediaFile.Open(filename);
            
        }

        public void EndLoop()
        {

            //if (bitmap != null) bitmap.Dispose();
            //bitmap = null;
            //_screenCaptureEngine.ClearFrame();



            //int i = 0;
            //var file = MediaFile.Open(@"h:\Alan Partridge's Midmorning Matters - S02E04.mp4");
            //while (file.Video.TryGetNextFrame(out var imageData))
            //{
            //    var b = imageData;
            //    b.ToBitmap().SaveAsPng($@"H:\frame_{i++}.png");
            //    // See the #Usage details for example .ToBitmap() implementation
            //    // The .Save() method may be different depending on your graphics library
            //}

        }

        public unsafe Color? GetColourForRectFromBitmapData(Rectangle rect, ImageData CurrentImageData)
        {
            Color? all;
            int rTot = 0, bTot = 0, gTot = 0;

            int PixelSize = CurrentImageData.PixelFormat == FFMediaToolkit.Graphics.ImagePixelFormat.Argb32 || CurrentImageData.PixelFormat == FFMediaToolkit.Graphics.ImagePixelFormat.Rgba32 ? 4 : 3;

            int ct = 0;

            {
                for (int y = rect.Y; y < rect.Height + rect.Y; y += 1)
                {
                    var d = y * CurrentImageData.Stride;

                    for (int x = rect.X; x < rect.Width + rect.X; x += 1)
                    {
                        if((d + x * PixelSize) > CurrentImageData.Data.Length) continue;
                        bTot += CurrentImageData.Data[d + x * PixelSize];
                        gTot += CurrentImageData.Data[d + x * PixelSize + 1];
                        rTot += CurrentImageData.Data[d + x * PixelSize + 2];
                        ct++; 
                    }
                }
            }

            all = ct > 0 ? Color.FromArgb(rTot / ct, gTot / ct, bTot / ct) : Color.Black;
            return all;
        }
    }

}
