using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class UshortMax : SignalProcessorBase, ISignalGenerator
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
                ComponentJsName = "NumberMaxComponent",
                ComponentName = "Maximum (65535)",
                HelpText = "Returns the number 0.",
            };
        }

        public new ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            debug?.AppendLine($"Maximum (65535) => 65535");
            return 65535;
        }
    }

}