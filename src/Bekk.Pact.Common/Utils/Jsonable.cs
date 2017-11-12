using System;
using Bekk.Pact.Common.Contracts;
using Newtonsoft.Json.Linq;

namespace Bekk.Pact.Common.Utils
{
    public class Jsonable : IJsonable
    {
        private readonly Func<JContainer> render;
        public Jsonable(Func<JContainer> render)
        {
            this.render = render;
        }
        public Jsonable(JContainer json) : this(()=>json){}
        public Jsonable(string json)
        {
            render = () => (JContainer) JToken.Parse(json);
        }
        public JContainer Render() => render();
    }
}