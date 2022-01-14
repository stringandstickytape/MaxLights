using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalProcessors;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MaxLifxCore
{
    public partial class AppController
    {

        private List<DiagramComponent> GetComponents()
        {
            //var numberSocket = DiagramSockets.First(x => x.Name == "Number");
            //var listSocket = DiagramSockets.First(x => x.Name == "List");
            //var booleanSocket = DiagramSockets.First(x => x.Name == "Boolean");
            //var stringSocket = DiagramSockets.First(x => x.Name == "String");
            //var hsbSocket = DiagramSockets.First(x => x.Name == "HSB");
            
            var diagramComponents = new List<DiagramComponent>();

            var processors = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace == "MaxLifxCore.SignalProcessors"
                    select t;

            foreach(var processor in processors)
            {
                var m = processor.GetMethod("GetDiagramComponent");
                if (m != null)
                {
                    var newComponent = (DiagramComponent)(m.Invoke(null, null));
                    newComponent.ImplementationClass = processor;
                    newComponent.Submenu = $"Output type: {newComponent.Outputs[0].Socket.Name}";
                    newComponent.ComponentJsName = $"{newComponent.ComponentName.Replace(" ","").Replace("(","").Replace(")","")}Component";
                    diagramComponents.Add(newComponent);
                    
                }
            }






            diagramComponents.AddRange(new List<DiagramComponent>
            {
                new DiagramComponent{
                    Inputs = new List<DiagramInput>() { new DiagramInput { JsToken = "inp1", InputName = "Rendered Light", Label = "Rendered Light", Socket = DiagramSockets.First(x => x.Name == "Rendered Light"), MultipleConnections = true } },
                    Outputs = new List<DiagramOutput>(),
                    ComponentJsName = "RendererComponent",
                    ComponentName = "Renderer",
                    HelpText = "Only lights connected to this component will be processed."
                },



                

                new DiagramComponent{
                    Inputs = new List<DiagramInput>()
                    {
                    },
                    Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Number", Socket = SignalProcessorBase.NumberSocket}
                    },
                    ComponentJsName = "NumberSineComponent",
                    ComponentName = "Sine",
                    HelpText = "Returns the Sine of a number, scaled to 0-65535.",
                    ImplementationClass = typeof(UshortSine)
                },

                //new DiagramComponent{
                //    Inputs = new List<DiagramInput>()
                //    {
                //    },
                //    Outputs = new List<DiagramOutput>()
                //    {
                //        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Number", Socket = SignalProcessorBase.NumberSocket}
                //    },
                //    ComponentJsName = "GpuTempComponent",
                //    ComponentName = "GPU Temp",
                //    HelpText = "Returns the current NVidia GPU temp, where 0 = 0C and 65535 = 100C",
                //    ImplementationClass = typeof(GpuTemp)
                //},

                

                
                // overloads - can't do this automatically yet

                new DiagramComponent{
                    Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List", Socket = SignalProcessorBase.ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Multiplier", Socket = SignalProcessorBase.NumberSocket}
                    },
                    Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "List", Socket = SignalProcessorBase.ListSocket}
                    },
                    ComponentJsName = "ListMultiplyComponent",
                    ComponentName = "Multiply List by Number",
                    HelpText = "Multiplies each element of a list by a number.",
                    ImplementationClass = typeof(Multiply)
                },

                new DiagramComponent{
                    Inputs = new List<DiagramInput>()
                    {
                        new DiagramInput { JsToken = "inp1", InputName = "num1", Label = "List", Socket = SignalProcessorBase.ListSocket},
                        new DiagramInput { JsToken = "inp2", InputName = "num2", Label = "Number", Socket = SignalProcessorBase.NumberSocket}
                    },
                    Outputs = new List<DiagramOutput>()
                    {
                        new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "List", Socket = SignalProcessorBase.ListSocket}
                    },
                    ComponentJsName = "ListDivideComponent",
                    ComponentName = "Divide List by Number",
                    HelpText = "Divides each element of a List by a single Number.",
                    ImplementationClass = typeof(Divide)
                },

});

            return diagramComponents;
        }
       }
}