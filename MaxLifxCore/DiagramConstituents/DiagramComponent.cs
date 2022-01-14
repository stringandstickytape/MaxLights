using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxLifxCore.DiagramConstituents
{


    public class DiagramComponent
    {
        public string ComponentJsName { get; set; }
        public string ComponentName { get; set; }

        public List<DiagramInput> Inputs { get; set; }

        public List<DiagramOutput> Outputs { get; set; }
        public Type ImplementationClass { get; set; }

        private string submenu = "";
        public string Submenu { get { return submenu; } set { submenu = value; } }

        public string PreCustomControlJS = "";
        public string CustomControlJS = "";
        public string CustomWorker = "";
        public string HelpText = "Hello World!!!!";

        public string GetJsDefinition()
        {
            var sb = new StringBuilder();

            sb.Append(
$@"class {ComponentJsName} extends Rete.Component {{
    constructor() {{
        super(""{ComponentName}"");
        this.data.component = CustomNode;
        this.data.submenu = '{Submenu}';
    }}



    builder(node) {{");


            if(ComponentJsName == "ScreenCaptureComponent")
            {
                sb.AppendLine($"var c1 = new ScreenSetupControl(this.editor, 'Presets');");
            }
            var inputCtr = 1;

            foreach(var input in Inputs)
            {
                sb.AppendLine($"var {input.JsToken} = new Rete.Input('{input.InputName}', '{input.Label}', {input.Socket.JsToken},  {(input.MultipleConnections ? "true" : "false")});");

                switch (input.Socket.Name)
                {
                    case "Number":
                        sb.AppendLine($"{input.JsToken}.addControl(new NumberControl(this.editor, '{input.Label}', false, {input.DefaultValue}));");
                        break;
                    case "Float":
                        sb.AppendLine($"{input.JsToken}.addControl(new FloatControl(this.editor, '{input.Label}', false, {input.DefaultValue}));");
                        break;
                    case "Boolean":
                        sb.AppendLine($"{input.JsToken}.addControl(new BooleanControl(this.editor, '{input.Label}'));");
                        break;
                    case "List":
                        break;
                    case "Rendered Light":
                        break;
                    case "String":
                        sb.AppendLine($"{input.JsToken}.addControl(new StringControl(this.editor, '{input.Label}'));");
                        break;
                    case "HSB":
                        break;
                    default: 
                            throw new NotImplementedException();
                }
                inputCtr++;
            }

            //sb.AppendLine($"var infoControl = new InfoControl(this.editor, 'Name');");

            foreach (var output in Outputs)
            {
                sb.AppendLine($"var {output.JsToken} = new Rete.Output('{output.OutputName}', '{output.Label}', {output.Socket.JsToken});");
                inputCtr++;
            }
            
            
            sb.AppendLine("return node");

            if (!string.IsNullOrWhiteSpace(PreCustomControlJS))
                sb.AppendLine(PreCustomControlJS);

            if (ComponentJsName == "ScreenCaptureComponent") 
                sb.AppendLine("    .addControl(c1)");

            foreach (var input in Inputs)
            sb.AppendLine($"    .addInput({input.JsToken})");

            //sb.AppendLine($"    .addControl(infoControl)");

            foreach (var output in Outputs)
                sb.AppendLine($"    .addOutput({output.JsToken})");

            if (!string.IsNullOrWhiteSpace(CustomControlJS))
                sb.AppendLine(CustomControlJS);

            sb.AppendLine(".addControl(new TooltipControl(this.editor, 'helpText', true))");

            sb.AppendLine(@";}

    worker(node, inputs, outputs) {");
            if (!string.IsNullOrWhiteSpace(CustomWorker))
                sb.AppendLine(CustomWorker);
            
            sb.AppendLine($"var node = this.editor.nodes.find(n => n.id == node.id);node.controls.get('helpText').setValue('{HelpText.Replace("'","\'")}');");
            sb.AppendLine(@"    }
}");

                return sb.ToString();
        }
    }

    public class DiagramInput
    {
        public string JsToken { get; set; }
        public string InputName { get; set; }
        public string Label { get; internal set; }
        public IDiagramSocket Socket { get; set; }

        public bool MultipleConnections { get; set; }
        public string DefaultValue { get; internal set; }
    }

    public class DiagramOutput
    {
        public string JsToken { get; set; }
        public string OutputName { get; set; }
        public string Label { get; internal set; }
        public IDiagramSocket Socket { get; set; }
    }
}
