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
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Multiply by", Socket = FloatSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Cap at 65535 instead of wrapping to 0", Socket = BooleanSocket},
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
            var cap = gen[2].GetLatestBoolValue(controller, light, debug);
            var s = gen[1].GetLatestFloatValue(controller,light,OutputSocketName2[1],debug);

            if(!cap)
                return gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug).Select(
                    x => (ushort)(x*s)
                    ).ToList();
            else
                return gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug).Select(
                    x => (ushort)(x * s > 65535 ? 65535 : x*s)
                    ).ToList();
        }
    }
}
