using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class ImageParameters : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Image Filename", Socket = StringSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {

                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Width", Socket = NumberSocket},
                        new DiagramOutput { JsToken = "out2", OutputName = "num1", Label = "Height", Socket = NumberSocket},
                    },
                ComponentJsName = "ImageParams",
                ComponentName = "Get Image Parameters",
                HelpText = "Gets the parameters of an image, such as width and height.",
            };
        }
        private Queue<ushort> _buffer;
        private ushort _bufferCapacity;
        private Bitmap frame = null;
        private int frameCtr = 0;


        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            if (frame == null)
            {
                var filename = gen[0].GetLatestStringValue(controller, light, debug);
                frame = new Bitmap(filename);
            }


            switch (socketName)
            {
                case "num1": // height
                    return (ushort)frame.Height;
                case "num": // width
                    return (ushort)frame.Width;
                default:
                    throw new NotImplementedException();
            }
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
                    throw new NotImplementedException();
        }


        public void EndLoop()
        {
            base.EndLoop();
        }
    }



}
