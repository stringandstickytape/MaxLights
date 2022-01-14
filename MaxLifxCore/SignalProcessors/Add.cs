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
    // Class will be discovered via reflection at runtime
    class Add : SignalProcessorBase, ISignalGenerator
    {
        // Description of the function and its inputs and outputs
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                // This function will have two Number inputs and one Number output.

                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Number 1", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Number 2", Socket = NumberSocket}
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Number", Socket = NumberSocket}
                    },

                // ComponentJSName will be used in the auto-generated JS code, and must be a valid JS variable name (no spaces, etc.)
                ComponentJsName = "NumberAddComponent",

                // ComponentName should be user-readable.  You should probably avoid quotes and double-quotes here :D
                ComponentName = "Add Numbers",

                // Help text for when the user hovers over the function name
                HelpText = "Adds two numbers.",
            };
        }

        // Actual C# implementation of function
        //
        // This method will be called when an output is asked for its latest Ushort (Number) value.  Note that for different kinds of output socket,
        // different methods are called (GetLatestHsbListValues, for instance, if the socket is HSB).
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            // Get the latest value for input 0, which we expect to be a Ushort
            ushort i0 = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);

            // Get the latest value for input 1, which we also expect to be a Ushort
            ushort i1 = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);

            // Work out the actual return value
            ushort retVal = (ushort)(i0 + i1);

            // Log it in debug if enabled
            debug?.AppendLine($"Add => {retVal}");

            // return the actual return value
            return retVal;
        }
    }
}
