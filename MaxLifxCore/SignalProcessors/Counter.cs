using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class Counter : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Counter Limit", Socket = NumberSocket},

                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Current counter value", Socket = NumberSocket}
                    },
                ComponentJsName = "CounterComponent",
                ComponentName = "Counter",
                HelpText = "Counts from zero to a given number, then loops.",
            };
        }

        private ushort _ctr = 0;
        private ushort _limit = 0;

        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            return _ctr;
        }
        public void SetupGenerator(DiagramNode node, Diagram diagram, AppController controller)
        {
            _limit = gen[0].GetLatestValue(controller, null, OutputSocketName2[0]);
        }

        public new void EndLoop()
        {
            _ctr++;

            if (_ctr >= _limit)
                _ctr = 0;

            base.EndLoop();
        }
    }



}
