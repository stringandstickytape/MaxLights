using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalProcessors;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;
using MaxLifxCoreBulbController.Controllers;
using MaxLifxCore.Controls;
using MaxLifxCoreBulbController.Payload;

namespace MaxLifxCore
{
    public partial class AppController
    {
        public Object LockObj;

        public AppSettings AppSettings = new AppSettings() { StaticLifxDevices = new List<LifxDevice>(), StaticWledDevices = new List<WledUdpDevice>() } ;

        public Dictionary<int, ScreenCaptureEngine> ScreenCaptureEngineDictionary = new Dictionary<int, ScreenCaptureEngine>();
        public List<IDiagramSocket> DiagramSockets { get; internal set; }
        public List<DiagramComponent> DiagramComponents { get; internal set; }

        public List<DiagramControl> DiagramControls { get; internal set; }
        private Func<int> callback { get; set; }

        public RGBSurface surface;

        public AppController(Func<int> formCallback)
        {
            LockObj = new object();

            callback = formCallback;
            Luminaires = new ObservableCollection<ILuminaireDevice>();

            DiagramSockets = new List<IDiagramSocket>
            {
                new StringSocket(),
                new NumberSocket(),
                new RenderedLightSocket(),
                new ListSocket(),
                new BooleanSocket(),
                new HsbSocket(),
                new FloatSocket()
            };

            List<DiagramComponent> diagramComponents = GetComponents();

            DiagramComponents = diagramComponents;

            DiagramControls = new List<DiagramControl>
            {
                new DiagramControl{ JsToken = "ListControl", VueControlJsToken = "VueListControl" },
                new DiagramControl{ JsToken = "NumberControl", VueControlJsToken = "VueNumberControl" },
                new DiagramControl{ JsToken = "FloatControl", VueControlJsToken = "VueFloatControl" },
                new DiagramControl{ JsToken = "StringControl", VueControlJsToken = "VueStringControl" },
                new DiagramControl{ JsToken = "InfoControl", VueControlJsToken = "VueInfoControl" },
                new DiagramControl{ JsToken = "ScreenSetupControl", VueControlJsToken = "VueScreenSetupControl" },
                new DiagramControl{ JsToken = "String2Control", VueControlJsToken = "VueString2Control" },
                new DiagramControl{ JsToken = "BooleanControl", VueControlJsToken = "VueBooleanControl" },
                new DiagramControl{ JsToken = "DefaultLayoutControl", VueControlJsToken = "VueDefaultLayoutControl" },
                new DiagramControl{ JsToken = "TooltipControl", VueControlJsToken = "VueTooltipControl" },
                new DiagramControl{ JsToken = "HsbControl", VueControlJsToken = "VueHsbControl" },
            };

        }


        public void LoadSettings()
        {
            if (File.Exists("settings.xml"))
            {
                var t = File.ReadAllText("settings.xml");

                using (var reader = new StringReader(t))
                    AppSettings = (AppSettings)(new System.Xml.Serialization.XmlSerializer(typeof(AppSettings)).Deserialize(reader));

                callback.Invoke();
            }
        }

        public void SaveSettings()
        {
            var x = MaxLifxCore.Utils.ToXmlString(AppSettings);
            File.WriteAllText("settings.xml", x);
        }

        public ObservableCollection<ILuminaireDevice> Luminaires = new ObservableCollection<ILuminaireDevice>();

        public List<System.Drawing.Point> LatestPoints { get; internal set; }

        public Diagram LatestDiagram = null;
        internal void SetFromDiagram(Diagram diagram, bool remap)
        {
            LatestDiagram = diagram;
            if (Luminaires == null) return;

            
            var diagramLights = diagram.nodes.Where(x => x.type == "Light").ToList();

            lock (ProcessableLights)
                ProcessableLights = new List<Light>();

            lock(Generators)
                Generators = new List<ISignalGenerator>();

            foreach (var diagramLight in diagramLights)
            {
                var luminaire = Luminaires.FirstOrDefault(x => x.IpAddress == diagramLight.data["IP Address"] && x.GetType().Name == diagramLight.lighttype);

                if (luminaire == null) luminaire = Luminaires.FirstOrDefault(x => x.IpAddress == diagramLight.data["IP Address"] && x.GetType().Name == "LifxDevice");

                if (luminaire == null) luminaire = Luminaires.FirstOrDefault(x => x.IpAddress == diagramLight.name);

                if (luminaire == null) luminaire = Luminaires.FirstOrDefault(x => x.Label == diagramLight.name.Split(':')[0].Trim(' '));

                if(luminaire == null || remap)
                {
                    var f = new SelectLuminaire(Luminaires.ToList());
                    f.ShowDialog();

                    if (f.Tag != null)
                    {
                        luminaire = (ILuminaireDevice)f.Tag;
                    }
                    else continue;
                }

                var newLight = new Light(luminaire, _controller, this, DateTime.Now);


                lock (ProcessableLights)
                    ProcessableLights.Add(newLight);

                ConstructFromDiagram(diagramLight, newLight);
            }
        }



        private void ConstructFromDiagram(DiagramNode diagramLight, Light processableLight)
        {
            lock (processableLight)
            {
                var endNode = diagramLight;

                var connectionsFromNode = LatestDiagram.connections.Where(x => x.inid == endNode.id);
                var distinctConnections = connectionsFromNode.DistinctBy(x => $"{x.inid}/{x.insocket}/{x.outid}/{x.outsocket}");

                lock (Generators)
                {
                    AddInputForConnectionOrControlValue(distinctConnections, processableLight, endNode, "Hue", 1);
                    AddInputForConnectionOrControlValue(distinctConnections, processableLight, endNode, "Brightness", 3);
                    AddInputForConnectionOrControlValue(distinctConnections, processableLight, endNode, "Saturation", 2);
                    AddInputForConnectionOrControlValue(distinctConnections, processableLight, endNode, "Frequency (ms)", 5);
                    AddInputForConnectionOrControlValue(distinctConnections, processableLight, endNode, "Fade", 6);
                    AddInputForConnectionOrControlValue(distinctConnections, processableLight, endNode, "Reverse", 7);
                    AddInputForConnectionOrControlValue(distinctConnections, processableLight, endNode, "HSB", 8);
                }
                if ((processableLight.gen1 == null || processableLight.gen2 == null || processableLight.gen3 == null) && processableLight.gen8 == null)  throw new LuminaireNullInputException($"{processableLight.Luminaire.IpAddress}: Hue/Sat/Bri/HSB"           );
                if (processableLight.gen5 == null) throw new LuminaireNullInputException($"{processableLight.Luminaire.IpAddress}: Frequency (ms)");
                if (processableLight.gen6 == null) throw new LuminaireNullInputException($"{processableLight.Luminaire.IpAddress}: Fade"          );
                if (processableLight.gen7 == null) throw new LuminaireNullInputException($"{processableLight.Luminaire.IpAddress}: Reverse"       );
            }
        }

        private void AddInputForConnectionOrControlValue(IEnumerable<DiagramConnection> connections, Light processableLight, DiagramNode endNode, string key, int input)
        {
            var connection = connections.SingleOrDefault(x => x.insocket == key);
            if (connection != null)
            {
                var node = LatestDiagram.nodes.Single(x => x.id == connection.outid);
                var nodeId = node.id;

                var gen = Generators.FirstOrDefault(x => x.NodeId == nodeId);

                if (gen == null)
                {
                    gen = CreateGeneratorFromNode(node, connection.outsocket);

                    lock (Generators)
                    {
                        Generators.Add(gen);
                    }
                }
                
                processableLight.SetInput(input, gen, connection.outsocket);
            }
            else
            {
                ISignalGenerator gen;
                var data = endNode.data.ContainsKey(key) ? endNode.data[key] : "0";

                if (input == 6 || input == 7)
                {
                    gen = new BooleanLiteral(data == "true").Initialise(new Random(), DateTime.Now, 1000, 0);
                }
                else if (input == 4 || input == 5) gen = new UshortLiteral((ushort?)(int.Parse(data))).Initialise(new Random(), DateTime.Now, 1000, 0);
                else gen = null;
                processableLight.SetInput(input, gen, "");
            }
        }

        public ISignalGenerator CreateGeneratorFromNode(DiagramNode node, string outputSocketName)
        {
            ISignalGenerator generator = null;

            var component = DiagramComponents.First(x => x.ComponentName == node.type);
            generator = (ISignalGenerator)Activator.CreateInstance(component.ImplementationClass);

            generator.Initialise(r, DateTime.Now, 1000, node.id);
            ((SignalProcessorBase)generator).AttachParents(node, LatestDiagram, this, ((DiagramComponent)(generator.GetType().GetMethod("GetDiagramComponent").Invoke(null,null))).Inputs.Count);
            generator.SetupGenerator(node, LatestDiagram, this);

            return generator;
            
        }

        public void LoadFromJson(string filename, bool remap)
        {
            var t = File.ReadAllText(filename);
            var diagram = JsonConvert.DeserializeObject<MaxLifxCore.Webserver.Diagram>(t);
            SetFromDiagram(diagram, remap);

        }


        public DateTime StartTime { get; private set; }
        public bool Debug { get { return false; } }

        public object JsToken { get; private set; }
        public List<ISignalGenerator> Generators { get; internal set; }
        public SpectrumAnalyserEngine SpectrumAnalyserEngine { get; internal set; }
        public Form1 Form { get; internal set; }

        private MaxLifxCoreBulbController.Controllers.MaxLifxCoreBulbController _controller;

        private List<Light> ProcessableLights;
        private Random r;

        public void Stop()
        {
            if (_discoveryThread != null && _discoveryThread.IsAlive)
                _discoveryThread.Abort();

            if (_controller != null)
                _controller.Stop();
        }

        private Thread _discoveryThread;
        internal void Run(ref MaxLifxCoreBulbController.Controllers.MaxLifxCoreBulbController controller)
        {
            Generators = new List<ISignalGenerator>();
            StartTime = DateTime.Now;
            _controller = controller;
            r = new Random();

            ProcessableLights = new List<Light>();

            _discoveryThread = new Thread(() =>
            {
                while (true)
                {
                    bool anyExpired = false;

                    lock (ProcessableLights)
                        anyExpired = ProcessableLights.Any(x => x.IsExpired());

                    while (!anyExpired)
                    {
                        Thread.Sleep(1);
                        lock (ProcessableLights)
                            anyExpired = ProcessableLights.Any(x => x.IsExpired());
                    }

                    lock (ProcessableLights)
                        foreach (var processableLight in ProcessableLights.Where(x => x.IsExpired()))
                            if (processableLight.InputsAreSet())
                            {
                                if (processableLight.Luminaire is LifxDevice && !processableLight.IsOn)
                                {
                                    var p = new SetPowerPayload(true);

                                    _controller.SendPayloadToMacAddress(p, processableLight.Luminaire.MacAddress, processableLight.Luminaire.IpAddress);
                                    Thread.Sleep(1);
                                    _controller.SendPayloadToMacAddress(p, processableLight.Luminaire.MacAddress, processableLight.Luminaire.IpAddress);
                                    processableLight.IsOn = true;
                                }
                                processableLight.Process();
                            }

                    lock(Generators)
                        Generators.ForEach(x => x.EndLoop());
                }
            });
            _discoveryThread.Start();
        }
    }

    static class Extensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}