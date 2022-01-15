
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
    class UshortScreenListCapture : SignalProcessorBase, ISignalGenerator
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
                        new DiagramInput { JsToken = "inp8", InputName = "num8", Label = "Monitor", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp9", InputName = "num9", Label = "Sample every n pixels horizontally", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp10", InputName = "num10", Label = "Sample every n pixels vertically", Socket = NumberSocket}
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "Hue", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out2", OutputName = "out2", Label = "Saturation", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out3", OutputName = "out3", Label = "Brightness", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out4", OutputName = "out4", Label = "HSB", Socket = HsbSocket}

                    },
                CustomControlJS = ".addControl(new TooltipControl(this.editor, 'summaryInfo', true))",

                CustomWorker = @"     
        var x =  parseInt(inputs.num1.length?inputs.num1[0]:node.data['X']);
        var y =  parseInt(inputs.num2.length?inputs.num2[0]:node.data['Y']);
        var w =  parseInt(inputs.num3.length?inputs.num3[0]:node.data['Width']);
        var h =  parseInt(inputs.num4.length?inputs.num4[0]:node.data['Height']);
        var sx = parseFloat(inputs.num5.length?inputs.num5[0]:node.data['X step']);
        var sy = parseFloat(inputs.num6.length?inputs.num6[0]:node.data['Y step']);
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
                ComponentJsName = "NumberScreenListCapture",
                ComponentName = "Screen Capture",
                HelpText = "Captures a list of colours from the screen.",
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
            List<ushort> hues, sats, bris;
            GetHsbs(controller, light, debug, out hues, out sats, out bris);
            var ct = Math.Min(Math.Min(hues.Count, sats.Count), bris.Count);
            return Enumerable.Range(0, ct).Select(x => new HsbUshort { H = hues[x], S = sats[x], B = bris[x] }).ToList();
        }
        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            List<ushort> hues, sats, bris;
            GetHsbs(controller, light, debug, out hues, out sats, out bris);

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

        private void GetHsbs(AppController controller, Light light, StringBuilder debug, out List<ushort> hues, out List<ushort> sats, out List<ushort> bris)
        {
            var width = gen[2].GetLatestValue(controller, light, OutputSocketName2[2], debug);
            var height = gen[3].GetLatestValue(controller, light, OutputSocketName2[3], debug);


            float x = (float)gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);
            float y = (float)gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);

            var xStep = gen[4].GetLatestFloatValue(controller, light, OutputSocketName2[4], debug);
            var yStep = gen[5].GetLatestFloatValue(controller, light, OutputSocketName2[5], debug);

            var itemsToGenerate = gen[6].GetLatestValue(controller, light, OutputSocketName2[6], debug);

            var monitor = gen[7].GetLatestValue(controller, light, OutputSocketName2[7], debug);

            var xAreaStep = gen[8].GetLatestValue(controller, light, OutputSocketName2[8], debug);
            var yAreaStep = gen[9].GetLatestValue(controller, light, OutputSocketName2[9], debug);

            hues = new List<ushort>();
            sats = new List<ushort>();
            bris = new List<ushort>();
            if (_screenCaptureEngine == null)
            {
                if (!controller.ScreenCaptureEngineDictionary.ContainsKey(monitor))
                    controller.ScreenCaptureEngineDictionary.Add(monitor, new ScreenCaptureEngine(monitor: monitor));

                _screenCaptureEngine = controller.ScreenCaptureEngineDictionary[monitor];
            }

            System.Drawing.Color? c;

            for (var ctr = 0; ctr < itemsToGenerate; ctr++)
            {
                c = _screenCaptureEngine.GetColour((ushort)x, (ushort)y, width, height, xAreaStep, yAreaStep);
                if (c == null)
                {
                    hues.Add((ushort)0);
                    sats.Add((ushort)0);
                    bris.Add((ushort)0);
                }
                else
                {
                    Utils.ColorToHSV(c.Value, out double hue, out double saturation, out double value);

                    hues.Add((ushort)(hue * 182.0444));
                    sats.Add((ushort)(saturation * 65535));
                    bris.Add((ushort)(value * 65535));
                }

                x += xStep;
                y += yStep;
            }
        }


        public void EndLoop()
        {
            if(_screenCaptureEngine != null)
                _screenCaptureEngine.ClearFrame();
        }
    }



}
