using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class HsbAdd : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Hsb 1", Socket = HsbSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Hsb 2", Socket = HsbSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "out1", Label = "HSB", Socket = HsbSocket},
                    },
                ComponentJsName = "HsbAddComponent",
                ComponentName = "HSB Add",
                HelpText = "Adds each pair of values from two input HSB lists.",
            };
        }


        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var hsb1 = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug);
            var hsb2 = gen[1].GetLatestHsbListValues(controller, light, OutputSocketName2[1], debug);
            
            var retVal = new List<HsbUshort>();

            var ct = Math.Min(hsb1.Count, hsb2.Count);

            for (var i = 0; i < ct; i++)
            {
                //if (hsb2[i].B > 0 || hsb1[i].B > 0)
                //{ }

               //var h1 = hsb1[i].H;
               //var h2 = hsb2[i].H;
               //var s1 = hsb1[i].S * (hsb1[i].B/65535f);
               //var s2 = hsb2[i].S * (hsb2[i].B / 65535f);
               //var ratio = (s1+s2)==0?0:s2/((float)s1+s2);
               //
               //var midHue = MaxLifx.Utils.HueBetween(h1, h2, ratio);
               //var midSat = (hsb2[i].S - hsb1[i].S) * ratio + hsb1[i].S;
               //var midBri = (hsb2[i].B - hsb1[i].B) / 2f + hsb1[i].B;
               //
               //retVal.Add(new HsbUshort { H = (ushort)(midHue), S = (ushort)(midSat), B = (ushort)(midBri) });
                
                var hsv1 = HSVColor.Create(((float)hsb1[i].H) / 182.044f, ((float)hsb1[i].S) / 65535, ((float)hsb1[i].B) / 65535);
                var hsv2 = HSVColor.Create(((float)hsb2[i].H) / 182.044f, ((float)hsb2[i].S) / 65535, ((float)hsb2[i].B) / 65535);

                var output = new RGB.NET.Core.Color(
                    (hsv1.GetR() + hsv2.GetR()) / 2,
                    (hsv1.GetG() + hsv2.GetG()) / 2,
                    (hsv1.GetB() + hsv2.GetB()) / 2);
                var output2 = output.GetHSV();
                retVal.Add(new HsbUshort { H = (ushort)(output2.hue * 182.044), S = (ushort)(output2.saturation * 65535), B = (ushort)(output2.value * 65535) });

                
            }

            return retVal;
        }
    }



}
