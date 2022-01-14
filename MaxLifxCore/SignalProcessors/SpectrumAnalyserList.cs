using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaxLifxCore.SignalProcessors
{
    class SpectrumAnalyserList : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Lower bin # (0-512)", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Upper bin # (0-512)", Socket = NumberSocket},
                        new DiagramInput { JsToken = "inp3", InputName = "num3", Label = "Values to generate", Socket = NumberSocket},
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "List", Socket = ListSocket}
                    },
                ComponentJsName = "NumberSpectrumAnalyserListComponent",
                ComponentName = "Spectrum Analyser List",
                HelpText = "Gets an audio frequency sample from the relevant bin (0-512), scaled to 0-65535.",
            };
        }



        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var lowerBin = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);
            var upperBin = gen[1].GetLatestValue(controller, light, OutputSocketName2[1], debug);
            var valuesToGenerate = gen[2].GetLatestValue(controller, light, OutputSocketName2[2], debug);
            var binRange = upperBin - lowerBin;
            var binsPerValue = (float)binRange / valuesToGenerate;

            var outList = new List<ushort>();

            float binCtr = lowerBin;

            while (outList.Count < valuesToGenerate)
            {
                if (controller.LatestPoints == null) outList.Add(0);
                else outList.Add((ushort)(controller.LatestPoints[(int)Math.Floor(binCtr)].Y * 256));
                binCtr += binsPerValue;
            }

            return outList;
        }
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
        }

    }



}
