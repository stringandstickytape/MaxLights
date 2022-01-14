using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class Clock : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Length of Cycle (ms)", Socket = NumberSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Current time (ms)", Socket = NumberSocket}
                    },
                ComponentJsName = "ClockComponent",
                ComponentName = "Clock",
                HelpText = "Repeatedly times a period of up to 65535ms.",
            };
        }



        public  ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            var cycleLength = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);

            var msSofar = (DateTime.Now - controller.StartTime).TotalMilliseconds;

            var retVal = (ushort)(msSofar % cycleLength);
            return retVal;
        }
    }



}
