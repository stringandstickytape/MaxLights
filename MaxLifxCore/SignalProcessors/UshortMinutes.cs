using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class UshortMinutes : SignalProcessorBase, ISignalGenerator
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
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Number", Socket = NumberSocket}
                    },
                ComponentJsName = "UshortMinutesComponent",
                ComponentName = "Time (minutes)",
                HelpText = "Returns the local time minutes value",
            };
        }

        public new ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            debug?.AppendLine($"UshortHours (0) => {(ushort)DateTime.Now.Minute}");
            return (ushort)DateTime.Now.Minute;
        }
    }

}