
using ColorThiefDotNet;

using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.SignalGenerators;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.Webserver;
using NAudio.CoreAudioApi;
using RGB.NET.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

using System.Text;
using System.Threading.Tasks;

namespace MaxLifxCore.SignalProcessors
{
    abstract class SignalProcessorBase : ISignalGenerator
    {
        public static IDiagramSocket NumberSocket = new NumberSocket();
        public static IDiagramSocket ListSocket = new ListSocket();
        public static IDiagramSocket BooleanSocket = new BooleanSocket();
        public static IDiagramSocket StringSocket = new StringSocket();
        public static IDiagramSocket HsbSocket = new HsbSocket();
        public static IDiagramSocket FloatSocket = new FloatSocket();

        internal ISignalGenerator[] gen;

        public ISignalGenerator Initialise(Random r, DateTime d, double interval, int nodeId)
        {
            Rnd = r;
            NodeId = nodeId;
            return this;
        }

        public Random Rnd;

        public string[] OutputSocketName2 { get; set; }

        public int NodeId { get; set; }

        public void Process() { }

        public void EndLoop()
        {

        }

        public float GetLatestFloatValue(AppController controller, Light light, string socketName, StringBuilder debug = null) { throw new NotImplementedException(); }

        public bool GetLatestBoolValue(AppController controller, Light light, StringBuilder debug = null) { throw new NotImplementedException(); }
        public string GetLatestStringValue(AppController controller, Light light, StringBuilder debug = null) { throw new NotImplementedException(); }
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null) { throw new NotImplementedException(); }

        public List<HsbUshort> GetLatestHsbListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null) { throw new NotImplementedException(); }

        public List<ushort> GetLatestListValues(AppController controller, Light light, string outputSocketName, StringBuilder debug = null) { throw new NotImplementedException(); }

        public void AttachParents(DiagramNode node, Diagram diagram, AppController controller, int parents)
        {
            FindAndAttachParents(node, diagram, controller, parents);
        }

        public void SetupGenerator(DiagramNode node, Diagram diagram, AppController controller)
        {
        }

        private void FindAndAttachParents(DiagramNode node, Diagram diagram, AppController controller, int ct)
        {
            gen = new ISignalGenerator[ct];
            OutputSocketName2 = new string[ct];

            for (var i = 0; i < ct; i++)
            {
                FindAndAttachParent(node, diagram, controller, i);
            }
        }

        private void FindAndAttachParent(DiagramNode node, Diagram diagram, AppController controller, int genNumber)
        {
            var connection1 = diagram.connections.Where(x => x.inid == node.id);
            var connection = connection1.FirstOrDefault(x => x.insocket == $"num{genNumber + 1}");

            if (connection != null)
            {
                var parentNode = diagram.nodes.Single(x => x.id == connection.outid);
                var nodeId = parentNode.id;

                var genx = controller.Generators.FirstOrDefault(x => x.NodeId == nodeId);

                if (genx == null)
                {
                    genx = controller.CreateGeneratorFromNode(parentNode, connection.outsocket);
                    //OutputSocketName2[genNumber] = connection.outsocket;
                    controller.Generators.Add(genx);
                }
                OutputSocketName2[genNumber] = connection.outsocket;
                gen[genNumber] = genx;

            }
            else //if (node.data.ContainsKey(key))
            {
                var comp = controller.DiagramComponents.First(x => x.ComponentName == node.type);
                var inputLabel = comp.Inputs[genNumber].Label;
                var data = node.data.ContainsKey(inputLabel.Replace("\\", "")) ? node.data[inputLabel.Replace("\\", "")] : "0";

                switch (comp.Inputs[genNumber].Socket.Name)
                {
                    case "Number":
                        gen[genNumber] = new UshortLiteral((ushort?)(int.Parse(data))).Initialise(new Random(), DateTime.Now, 1000, 0);
                        break;
                    case "Boolean":
                        gen[genNumber] = new BooleanLiteral(data == "true").Initialise(new Random(), DateTime.Now, 1000, 0);
                        break;
                    case "String":
                        gen[genNumber] = new StringLiteral(data).Initialise(new Random(), DateTime.Now, 1000, 0);
                        break;
                    case "Float":
                        gen[genNumber] = new FloatLiteral(float.Parse(data)).Initialise(new Random(), DateTime.Now, 1000, 0);
                        break;
                    case "HSB":
                        gen[genNumber] = null;
                        break;
                    default:

                        throw new ComponentNullInputException($"{comp.ComponentName} component {comp.Inputs[genNumber].Socket.Name}");
                }
                OutputSocketName2[genNumber] = "";
            }
        }
    }
}