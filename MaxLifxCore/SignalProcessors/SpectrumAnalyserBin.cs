using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class SpectrumAnalyserBin : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Number", Socket = NumberSocket}
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Number", Socket = NumberSocket}
                    },
                ComponentJsName = "NumberSpectrumAnalyserBinComponent",
                ComponentName = "Spectrum Analyser",
                HelpText = "Gets an audio frequency sample from the relevant bin (0-512), scaled to 0-65535.",
            };
        }



        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            var bin = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);
            var retVal = (ushort)(controller.LatestPoints[bin].Y * 256);
            debug?.AppendLine($"Spectrum Analyser => {retVal}");

            return retVal;
        }

    }



}
