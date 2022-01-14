using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxLifxCore.DiagramConstituents
{
    public interface IDiagramSocket
    {
        string Name { get; }
        string JsToken { get; }
    }

    public class ListSocket: IDiagramSocket
    {
        public string Name => "List";
        public string JsToken => "listSocket";
    }

    public class BooleanSocket : IDiagramSocket
    {
        public string Name => "Boolean";
        public string JsToken => "booleanSocket";
    }

    public class FloatSocket : IDiagramSocket
    {
        public string Name => "Float";
        public string JsToken => "floatSocket";
    }

    public class NumberSocket : IDiagramSocket
    {
        public string Name => "Number";
        public string JsToken => "numberSocket";
    }

    public class HsbSocket : IDiagramSocket
    {
        public string Name => "HSB";
        public string JsToken => "hsbSocket";
    }


    public class RenderedLightSocket : IDiagramSocket
    {
        public string Name => "Rendered Light";
        public string JsToken => "renderedLightSocket";
    }

    public class StringSocket : IDiagramSocket
    {
        public string Name => "String";
        public string JsToken => "stringSocket";
    }
}
