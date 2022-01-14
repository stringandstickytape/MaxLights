using Ceen;
using Ceen.Httpd;
using Ceen.Httpd.Handler;
using Ceen.Httpd.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MaxLifxCore.Webserver
{
    class WebserverCore
    {
        public WebserverCore(ref AppController appController)
        {
            _appController = appController;
        }

        private AppController _appController;

        public void InitWebserver()
        {
            var tcs = new CancellationTokenSource();
            var config = new ServerConfig()
                .AddLogger(new CLFStdOut())
                .AddRoute("/timeofday", new TimeOfDayHandler(ref _appController))
                .AddRoute("/toggle", new ToggleHandler(ref _appController))
                .AddRoute("/listfunctions", new ListFunctionsHandler(ref _appController))
                .AddRoute("/json", new UploadDiagramHandler(ref _appController))
                .AddRoute("/load", new LoadHandler(ref _appController))
                .AddRoute("/spectrum", new SpectrumHandler(ref _appController))
                .AddRoute(new FileHandler("."));

            var task = HttpServer.ListenAsync(
                new IPEndPoint(IPAddress.Any, _appController.AppSettings.Port == 0 ? 8080 : _appController.AppSettings.Port),
                false,
                config,
                tcs.Token
            );

            //tcs.Cancel(); // Request stop
            //task.Wait();  // Wait for shutdown
        }
    }

    internal class ListFunctionsHandler : Ceen.IHttpModule
    {
        private AppController _appController;

        public ListFunctionsHandler(ref AppController appController)
        {
            _appController = appController;
        }

        public async Task<bool> HandleAsync(IHttpContext context)
        {
            context.Response.SetNonCacheable();

            var output = $"<html><body><table style='border-collapse: collapse;'><tr>{string.Join("</tr><tr>", (_appController.DiagramComponents.Select(x => $"<td  style='border:1px solid black;'>{x.ComponentName}</td><td style='border:1px solid black;'>{x.HelpText}</td>")))}</tr></table></html></body>";

            await context.Response.WriteAllAsync(System.Text.Encoding.ASCII.GetBytes(output), "text/html");
            return true;
        }
    }

    public class TimeOfDayHandler : Ceen.IHttpModule
    {
        private AppController _appController;
        public TimeOfDayHandler(ref AppController AppController)
        {
            _appController = AppController;
        }
        public async Task<bool> HandleAsync(IHttpContext context)
        {
            context.Response.SetNonCacheable();
            await context.Response.WriteAllJsonAsync(JsonConvert.SerializeObject(new { app = _appController }));
            return true;
        }
    }

    public class Diagram
    {
        public List<DiagramNode> nodes { get; set; }
        public List<DiagramConnection> connections { get; set; }
    }
    public class DiagramNode
    {
        public int id { get; set; }
        public string name { get; set; }

        public string type { get; set; }

        public string lighttype { get; set; }

        public ushort? numberval { get; set; }

        public string stringval { get; set; }

        public Dictionary<string, string> data {get;set;}

    }

    public class DiagramConnection
    {
        //outid: connection.output.node.id, outsocket: connection.output.socket.name, inid: connection.input.node.id, insocket: connection.input.socket.name });
        public int outid { get; set; }

        public int inid { get; set; }
        public string outsocket { get; set; }
        public string insocket { get; set; }

    }

    public class ToggleHandler : Ceen.IHttpModule
    {
        private AppController _appController;
        public ToggleHandler(ref AppController AppController)
        {
            _appController = AppController;
        }
        public async Task<bool> HandleAsync(IHttpContext context)
        {
            var t = File.ReadAllText("graph.html");

            var s = "";
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                s += $"Monitor #{System.Windows.Forms.Screen.AllScreens.ToList().IndexOf(screen)}: {screen.Bounds.Width} x {screen.Bounds.Height}<br/>";

            s = "<br/><div>" + s + "</div>";

            t = t.Replace("###MonitorInfo###", s);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("var mzlightcomponent = components.filter(function (i) { return i.name == 'Light'; });");
            sb.AppendLine("var renderercomponent = components.filter(function (i) { return i.name == 'Renderer'; });");

            if (_appController.LatestDiagram == null)
            {

                for (var i = 0; i < _appController.Luminaires.Count; i++) 
                {
                    
                    sb.AppendLine($@"
var l{i} = await mzlightcomponent[0].createNode({{name:'{_appController.Luminaires[i].IpAddress}'}}); 
l{i}.data['Name'] = '{_appController.Luminaires[i].Label?.Replace("'","")}'; 
l{i}.data['Type'] = '{_appController.Luminaires[i].GetType().Name}'; 
l{i}.data['IP Address'] = '{_appController.Luminaires[i].IpAddress}'; 
l{i}.data['Frequency (ms)'] = 100; 
l{i}.data['helpText'] = '';
editor.addNode(l{i});

");
                }

                sb.AppendLine($"renderer = await renderercomponent[0].createNode({{}});  editor.addNode(renderer);");
            }
            else { 
                _appController.LatestDiagram.connections = _appController.LatestDiagram.connections.DistinctBy(x => $"{x.inid}/{x.insocket}/{x.outid}/{x.outsocket}").ToList();

                for (var i = 0; i < _appController.Luminaires.Count; i++)
                {

                    sb.AppendLine($@"var l{i} = await mzlightcomponent[0].createNode({{name:'{_appController.Luminaires[i].IpAddress}'}});  
l{i}.data['Name'] = '{_appController.Luminaires[i].Label?.Replace("'", "")}'; 
l{i}.data['Type'] = '{_appController.Luminaires[i].GetType().Name}'; 
l{i}.data['IP Address'] = '{_appController.Luminaires[i].IpAddress}';
l{i}.data['Frequency (ms)'] = 100; 
l{i}.data['helpText'] = '';
editor.addNode(l{i});");
                }
            }

            sb.AppendLine($"latestDiagram = {JsonConvert.SerializeObject(_appController.LatestDiagram)};");
            sb.AppendLine();

            var reteJs = new StringBuilder();
            reteJs.Append(string.Join(Environment.NewLine, _appController.DiagramSockets.Select(x => $"var {x.JsToken} = new Rete.Socket('{x.Name}'); ")));

            var controlJsList = _appController.DiagramControls.Select(x => x.GetJsDefinition());

            foreach (var controlJs in controlJsList)
            {
                reteJs.Append(controlJs);
                reteJs.AppendLine();
            }


            var componentJsList = _appController.DiagramComponents.OrderBy(x => x.ComponentName).Select(x => x.GetJsDefinition());

            foreach (var componentJs in componentJsList)
            {
                reteJs.Append(componentJs);
                reteJs.AppendLine();
            }

            //sb.AppendLine("editor.connect(h.outputs.get('hue'), l0.inputs.get('Hue'));");
            var lightsJs = string.Join(Environment.NewLine, _appController.Luminaires.Select(x => $""));

            t = t.Replace("MAGICSOCKETTOKEN", reteJs.ToString())
                .Replace("MAGICTOKEN", sb.ToString());

            var componentNames = _appController.DiagramComponents.OrderBy(x => x.ComponentName).Select(x => $"new {x.ComponentJsName}()");

            var componentListJs = $"var components = [ new NumberComponent(),  new ListComponent(), new StringComponent(), new LightComponent(), {string.Join(",", componentNames)} ];";

            t = t.Replace("MAGICCOMPONENTTOKEN", componentListJs);

            context.Response.SetNonCacheable();
            await context.Response.WriteAllAsync(System.Text.Encoding.ASCII.GetBytes(t),"text/html");//WriteAllJsonAsync(JsonConvert.SerializeObject(new { app = _appController }));
            return true;
        }
    }



    public class LoadHandler : Ceen.IHttpModule
    {
        private AppController _appController;
        public LoadHandler(ref AppController AppController)
        {
            _appController = AppController;
        }
        public async Task<bool> HandleAsync(IHttpContext context)
        {
            var t = File.ReadAllText("load.html");

            if(context.Request.QueryString.ContainsKey("filename"))
            {
                _appController.LoadFromJson(context.Request.QueryString["filename"], false);
            }
            StringBuilder sb = new StringBuilder();

            var files = Directory.GetFiles(Directory.GetCurrentDirectory()).Where(f => f.EndsWith(".json"));

            foreach (var file in files)
                sb.AppendLine($"<button onclick='clickBut(\"{file.Replace(Directory.GetCurrentDirectory() + "\\", "")}\")'>{file.Replace(Directory.GetCurrentDirectory()+"\\","")}</button>");

            t = t.Replace("MAGICJSONTOKEN", sb.ToString());
            
            context.Response.SetNonCacheable();
            await context.Response.WriteAllAsync(System.Text.Encoding.ASCII.GetBytes(t), "text/html");//WriteAllJsonAsync(JsonConvert.SerializeObject(new { app = _appController }));
            return true;
        }
    }


    public class SpectrumHandler : Ceen.IHttpModule
    {
        private AppController _appController;
        public SpectrumHandler(ref AppController AppController)
        {
            _appController = AppController;
        }
        public async Task<bool> HandleAsync(IHttpContext context)
        {
            var t = File.ReadAllText("spectrum.html");

            var points = _appController.LatestPoints;

            var bitmap = new System.Drawing.Bitmap(points.Count, 256);
            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                gr.Clear(Color.White);

                var pen = new Pen(Brushes.Green);

                foreach (var point in points)
                    gr.DrawLine(pen, new Point(point.X, 0), new Point(point.X, point.Y));
                
            }

            // if (context.Request.QueryString.ContainsKey("filename"))
            // {
            //     _appController.LoadFromJson(context.Request.QueryString["filename"]);
            // }
            // StringBuilder sb = new StringBuilder();
            //
            // var files = Directory.GetFiles(Directory.GetCurrentDirectory()).Where(f => f.EndsWith(".json"));
            //
            // foreach (var file in files)
            //     sb.AppendLine($"<button onclick='clickBut(\"{file.Replace(Directory.GetCurrentDirectory() + "\\", "")}\")'>{file.Replace(Directory.GetCurrentDirectory() + "\\", "")}</button>");
            //
            // t = t.Replace("MAGICJSONTOKEN", sb.ToString());
            byte[] image;
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                image = stream.ToArray();
            }



            context.Response.SetNonCacheable();
            await context.Response.WriteAllAsync(image, "image/png");//WriteAllJsonAsync(JsonConvert.SerializeObject(new { app = _appController }));
            return true;
        }
    }


    public class UploadDiagramHandler : Ceen.IHttpModule
    {
        private AppController _appController;
        public UploadDiagramHandler(ref AppController AppController)
        {
            _appController = AppController;
        }
        public async Task<bool> HandleAsync(IHttpContext context)
        {
            context.Response.SetNonCacheable();

            if (context.Request.Body.Length > -1)
            {
                var buffer = new Byte[context.Request.Body.Length];

                while (context.Request.Body.Position < context.Request.Body.Length)
                {
                    context.Request.Body.Read(buffer, (int)context.Request.Body.Position, (int)context.Request.Body.Length);
                }
                var json = System.Text.Encoding.UTF8.GetString(buffer);

                var diagram = JsonConvert.DeserializeObject<Diagram>(json);

                try
                {
                    _appController.SetFromDiagram(diagram, false);
                }
                catch(Exception e)
                {
                    await context.Response.WriteAllAsync(System.Text.Encoding.ASCII.GetBytes(e.ToString()), "text/html");//WriteAllJsonAsync(JsonConvert.SerializeObject(new { app = _appController }));
                    return false;
                }
            }

            await context.Response.WriteAllAsync(System.Text.Encoding.ASCII.GetBytes("Success"), "text/html");//WriteAllJsonAsync(JsonConvert.SerializeObject(new { app = _appController }));
            return true;
        }
    }
    public class LuminaireNotFoundException : Exception { 

        string ip { get; set; }
        public LuminaireNotFoundException(string message) : base(message) { ip = message; } 
        
        public override string ToString() { return $"{ip} is not a known IP address for any light."; } 
    
    };

    public class LuminaireNullInputException : Exception
    {

        string ip { get; set; }
        public LuminaireNullInputException(string message) : base(message) { ip = message; }

        public override string ToString() {
            return $"{ip} input is not attached to anything.";
        }

    };

    public class ComponentNullInputException : Exception
    {

        string ip { get; set; }
        public ComponentNullInputException(string message) : base(message) { ip = message; }

        public override string ToString() { 
            return $"{ip} input is not attached to anything.";
        }

    };
}
