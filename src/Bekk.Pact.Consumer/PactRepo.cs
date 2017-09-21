using System.Threading.Tasks;
using Bekk.Pact.Common;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Consumer.Contracts;
using Bekk.Pact.Consumer.Rendering;

namespace Bekk.Pact.Consumer
{
    class PactRepo : Common.Utils.PactRepoBase
    {
        public PactRepo(IConfiguration configuration): base(configuration)
        {
        }

        public async Task Put(IPactDefinition pact)
        {
            await PutPacts(pact ,new PactJsonRenderer(pact).ToString());
        }
    }
}