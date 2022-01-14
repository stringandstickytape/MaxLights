using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using OpenHardwareMonitor.Hardware;
using MaxLifxCore.SignalReceivers;
using MaxLifxCore.SignalProcessors;
using MaxLifxCore.DiagramConstituents;
using MaxLifxCore.Webserver;

namespace MaxLifxCore.SignalGenerators
{
    class UshortLiteral : SignalGeneratorBase, ISignalGenerator
    {
        public UshortLiteral(ushort? ushortval)
        {
            _currentValue = ushortval.HasValue ? ushortval.Value : (ushort)0;
        }

        public new ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            debug?.AppendLine($"Number => {_currentValue}");
            return _currentValue;
        }
    }

    class FloatLiteral : SignalGeneratorBase, ISignalGenerator
    {
        float _val;
        public FloatLiteral(float f)
        {
            _val = f;
        }

        public new float GetLatestFloatValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            debug?.AppendLine($"Float literal => {_val}");
            return _val;
        }
    }

    class BooleanLiteral : SignalGeneratorBase, ISignalGenerator
    {
        Boolean _val;
        public BooleanLiteral(bool b)
        {
            _val = b;
        }

        public new bool GetLatestBoolValue(AppController controller, Light light, StringBuilder debug = null)
        {
            debug?.AppendLine($"Boolean literal => {_val}");
            return _val;
        }
    }

    class StringLiteral : SignalGeneratorBase, ISignalGenerator
    {
        string _val;
        public StringLiteral(string  str)
        {
            _val = str;
        }

        public new string GetLatestStringValue(AppController controller, Light light, StringBuilder debug = null)
        {
            debug?.AppendLine($"Boolean literal => {_val}");
            return _val;
        }
    }


    class UshortSine : SignalGeneratorBase, ISignalGenerator
    {
        public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
        {
            var msSinceStart = DateTime.Now.Subtract(controller.StartTime).TotalMilliseconds;

            var window = 5000;

            var sValue = msSinceStart % window;

            sValue = sValue * Math.PI * 2 / window;

            var sineValue = Math.Sin(sValue);
            
            var ushortSineValue = (ushort)(sineValue * 32768 + 32767);

            debug?.AppendLine($"Sine => {ushortSineValue}");

            return ushortSineValue;
        }
    }

    //class GpuTemp : SignalProcessorBase, ISignalGenerator
    //{
    //    UpdateVisitor updateVisitor = new UpdateVisitor();
    //    Computer computer = new Computer();
    //    public static DiagramComponent GetDiagramComponent()
    //    {
    //        return new DiagramComponent
    //        {
    //            Inputs = new List<DiagramInput>()
    //            {
    //            },
    //            Outputs = new List<DiagramOutput>()
    //                {
    //                    new DiagramOutput { JsToken = "out1", OutputName = "num", Label = "Number", Socket = NumberSocket}
    //                },
    //            ComponentJsName = "GpuTempComponent",
    //            ComponentName = "NVidia GPU Temp",
    //            HelpText = "Returns the GPU temp (NVidia only)",
    //        };
    //    }
    //    public void SetupGenerator(DiagramNode node, Diagram diagram, AppController controller)
    //    {
    //        computer.Open();
    //        computer.GPUEnabled = true;
    //        computer.Accept(updateVisitor);
    //    }
    //    public ushort GetLatestValue(AppController controller, Light light, string socketName, StringBuilder debug = null)
    //    {
    //        //Double temperature = 0;
    //        //String instanceName = "";
    //        //
    //        //
    //        //// Query the MSAcpi_ThermalZoneTemperature API
    //        //// Note: run your app or Visual Studio (while programming) or you will get "Access Denied"
    //        //ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
    //        //
    //        //foreach (ManagementObject obj in searcher.Get())
    //        //{
    //        //    temperature = Convert.ToDouble(obj["CurrentTemperature"].ToString());
    //        //    // Convert the value to celsius degrees
    //        //    temperature = (temperature - 2732) / 10.0;
    //        //    instanceName = obj["InstanceName"].ToString();
    //        //    Console.WriteLine(temperature);
    //        //
    //        //    // ACPI\ThermalZone\TZ01_0
    //        //    Console.WriteLine(instanceName);
    //        //}
    //
    //
    //
    //        for (int i = 0; i < computer.Hardware.Length; i++)
    //        {
    //            if (computer.Hardware[i].HardwareType == HardwareType.GpuNvidia)
    //            {
    //                var gpuCoreSensor = computer.Hardware[i].Sensors.First(x => x.Name == "GPU Core").Value;
    //                //computer.Close();
    //                return (ushort)(gpuCoreSensor * 655.35f);
    //
    //            }
    //        }
    //        //computer.Close();
    //
    //
    //        return 0;
    //    }
    //}

    // from https://stackoverflow.com/questions/29607595/c-sharp-cpu-and-gpu-temp
    //class UpdateVisitor : IVisitor
    //{
    //    public void VisitComputer(IComputer computer)
    //    {
    //        computer.Traverse(this);
    //    }
    //    public void VisitHardware(IHardware hardware)
    //    {
    //        hardware.Update();
    //        foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
    //    }
    //    public void VisitSensor(ISensor sensor) { }
    //    public void VisitParameter(IParameter parameter) { }
    //}
}
