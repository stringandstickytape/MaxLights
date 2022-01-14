using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class KeydownTest : SignalProcessorBase, ISignalGenerator
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
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Number (0-256)", Socket = NumberSocket}
                    },
                ComponentJsName = "KeydownTestComponent",
                ComponentName = "Keydown",
                HelpText = "Returns the lowest VK_ code of all held-down keys. See https://docs.microsoft.com/en-gb/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN for keycodes.",
            };
        }
        private List<short> prevKeyStates, currKeyStates;


        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        // vks: https://docs.microsoft.com/en-gb/windows/win32/inputdev/virtual-key-codes?redirectedfrom=MSDN

        public ushort GetLatestValue(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            if(currKeyStates == null)
                currKeyStates = Enumerable.Range(0, 256).Select(x => GetAsyncKeyState(x)).ToList();

            if (prevKeyStates == null)
                prevKeyStates = currKeyStates;

            var ctr = 0;

            while (ctr < 255 && currKeyStates[ctr] == prevKeyStates[ctr])
                ctr++;

            if (ctr == 255 && currKeyStates[ctr] == prevKeyStates[ctr]) 
                return 0;

            return ((ushort)ctr);
        }

        public new void EndLoop()
        {
            prevKeyStates = currKeyStates;
            currKeyStates = null;
            
            base.EndLoop();
        }
    }



}
