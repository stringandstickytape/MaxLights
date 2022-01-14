using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class UshortMin : SignalProcessorBase, ISignalGenerator
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
                ComponentJsName = "NumberMinComponent",
                ComponentName = "Minimum (0)",
                HelpText = "Returns the number 0.",
            };
        }

        public new ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            debug?.AppendLine($"Minimum (0) => 0");
            return 0;
        }
    }

}