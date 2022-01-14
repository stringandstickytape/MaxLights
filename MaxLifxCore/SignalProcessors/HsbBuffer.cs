using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class HsbBuffer : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "HSB", Socket = HsbSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "BufferLength", Socket = NumberSocket}
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "HSB", Socket = HsbSocket}
                    },
                ComponentJsName = "HSBBufferComponent",
                ComponentName = "HsbBuffer",
                HelpText = "First-In-First-Out buffer that can store and send out HSB lists.",
            };
        }
        private Queue<List<HsbUshort>> _buffer;
        private ushort _bufferCapacity;
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var newVal = gen[0].GetLatestHsbListValues(controller, light, OutputSocketName2[0], debug);

            List<HsbUshort> retVal;
            debug?.Append($"ListGenerator => ");
            if (_buffer.Count == _bufferCapacity)
            {
                debug?.Append($"Dequeue => ");
                retVal = _buffer.Dequeue();
            }
            else if (_buffer.Count > 0)
            {
                debug?.Append($"Peek => ");
                retVal = _buffer.Peek();
            }
            else
            {
                debug?.Append($"Passthrough => ");
                retVal = newVal;
            }

            _buffer.Enqueue(newVal);
            debug?.AppendLine($"{newVal.Count}");

            return retVal;
        }

        public void SetupGenerator(DiagramNode node, Diagram diagram, AppController controller)
        {
            _bufferCapacity = gen[1].GetLatestValue(controller, null, "");
            _buffer = new Queue<List<HsbUshort>>(_bufferCapacity);
            
        }
    }



}
