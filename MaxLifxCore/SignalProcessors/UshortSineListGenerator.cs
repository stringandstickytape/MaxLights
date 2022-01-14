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
    class UshortSineListGenerator : SignalProcessorBase, ISignalGenerator
    {
        public static DiagramComponent GetDiagramComponent()
        {
            return new DiagramComponent
            {
                Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "Items to Generate", Socket = NumberSocket },
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Cosine", Socket = BooleanSocket },
                    },
                Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "list", Label = "List of Numbers", Socket = ListSocket}
                    },
                ComponentJsName = "SineListGeneratorComponent",
                ComponentName = "Sine List Generator",
                HelpText = "Generates a sine wave that varies by time and list position",
            };
        }
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            throw new NotImplementedException();
            /*             var msSinceStart = DateTime.Now.Subtract(controller.StartTime).TotalMilliseconds;

            var window = 5000;

            var sValue = msSinceStart % window;

            sValue = sValue * Math.PI * 2 / window;

            var sineValue = Math.Sin(sValue);
            Debug.WriteLine(sineValue);
            var ushortSineValue = (ushort)(sineValue * 32768 + 32767);

            debug?.AppendLine($"Sine => {ushortSineValue}");

            return ushortSineValue; */
        }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null)
        {
            var itemsToGenerate = gen[0].GetLatestValue(controller, light, OutputSocketName2[0], debug);

            var cosine = gen[1].GetLatestBoolValue(controller, light, debug);

            var msSinceStart = DateTime.Now.Subtract(controller.StartTime).TotalMilliseconds;

            var window = 5000;

            var sValue = msSinceStart % window;

            sValue = sValue * Math.PI * 2 / window;

            var step = (Math.PI * 2) / itemsToGenerate;

            var outList = new List<ushort>();
            for (var i = 0; i < itemsToGenerate; i++)
            {
                
                var sineValue = cosine ? Math.Cos(sValue) : Math.Sin(sValue);
                var ushortSineValue = (ushort)(sineValue * 32768 + 32767);

                outList.Add(ushortSineValue);

                sValue += step;

            }
            debug?.AppendLine($"ListGenerator => {string.Join(",", outList.Select(x => x.ToString()))}");
            return outList;
        }

    }



}
