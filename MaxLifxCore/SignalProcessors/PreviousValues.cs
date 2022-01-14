
using ColorThiefDotNet;

using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using NAudio.CoreAudioApi;
using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

using System.Text;
using System.Threading.Tasks;

namespace MaxLifxCore.SignalProcessors
{


    class PreviousValues : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                {
                },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Hue", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out2", OutputName = "num1", Label = "Saturation", Socket = ListSocket},
                        new DiagramOutput { JsToken = "out3", OutputName = "num2", Label = "Brightness", Socket = ListSocket},
                    },
                ComponentJsName = "PreviousValuesComponent",
                ComponentName = "Previous Values",
                HelpText = "Returns the previous numbers used to set the same light.",
            };
        }

        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }
        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            List<ushort> retVal;

            switch (outputSocketName)
            {
                case "num":
                    if (light.prevHsbMode) retVal = light.prevHSBs.Select(x => x.H).ToList();
                    else retVal = light.prevHues;
                    break;
                case "num1":
                    if (light.prevHsbMode) retVal = light.prevHSBs.Select(x => x.S).ToList();
                    else retVal = light.prevSats;
                    break;
                case "num2":
                    if (light.prevHsbMode) retVal = light.prevHSBs.Select(x => x.B).ToList();
                    else retVal = light.prevBris;
                    break;
                default: throw new NotImplementedException();
            }

            if (retVal == null) retVal = new ushort[light.Luminaire.Zones].ToList();

            return retVal;
        }

    }
}