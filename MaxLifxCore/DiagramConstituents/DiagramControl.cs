using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxLifxCore.DiagramConstituents
{
    public class DiagramControl
    {
        public string JsToken { get; internal set; }
        public string VueControlJsToken { get; internal set; }

        internal object GetJsDefinition()
        {
return $@"class {JsToken} extends Rete.Control {{

    constructor(emitter, key, readonly, defaultVal) {{

        super(key);
        this.component = {VueControlJsToken};
        this.props = {{ emitter, ikey: key, readonly, defaultVal }};
    }}

    setValue(val) {{
        this.vueContext.value = val;
    }}
}}
";
        }
    }
}
