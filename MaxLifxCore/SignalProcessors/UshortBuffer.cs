using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class UshortBuffer : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List", Socket = ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "BufferLength", Socket = NumberSocket}
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "List", Socket = ListSocket}
                    },
                ComponentJsName = "NumberBufferComponent",
                ComponentName = "Buffer",
                HelpText = "First-In-First-Out buffer that can store and send out numbers.",
            };
        }
        private Queue<List<ushort>> _buffer;
        private ushort _bufferCapacity;
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var newVal = gen[0].GetLatestListValues(controller, light, OutputSocketName2[0], debug);

            List<ushort> retVal;
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
            _buffer = new Queue<List<ushort>>(_bufferCapacity);
            
        }
    }



}
