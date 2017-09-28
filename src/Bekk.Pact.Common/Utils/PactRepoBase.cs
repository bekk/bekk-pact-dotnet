using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Bekk.Pact.Common.Contracts;
using System.Linq;
using Bekk.Pact.Common.Extensions;
using Bekk.Pact.Common.Exceptions;
using System.Text;
using System.IO;

namespace Bekk.Pact.Common.Utils
{
    public abstract class PactRepoBase : IDisposable
    {
        protected PactRepoBase(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        private HttpClient _client;
        protected HttpClient Client => _client ?? (_client = new HttpClient());
        protected IConfiguration Configuration { get; }
        
        public void Dispose()
        {
            _client?.Dispose();
            _client = null;
        }
    }
}