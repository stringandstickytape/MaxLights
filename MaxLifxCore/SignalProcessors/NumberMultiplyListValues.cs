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
    class NumberMultiplyListValues : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Multiply by", Socket = NumberSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "List", Socket = ListSocket}
                    },
                ComponentJsName = "NumberMultiplyListValuesComponent",
                ComponentName = "Multiply List Values by Number",
                HelpText = "Multiplies the numbers in a list by a single input number.",
            };
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var s = gen[1].GetLatestValue(controller,light,OutputSocketName2[1],debug);
            return gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug).Select(
                x => (ushort)((x * s) - (x * s) / 65536 * 65536)
                ).ToList();
        }
    }
}
