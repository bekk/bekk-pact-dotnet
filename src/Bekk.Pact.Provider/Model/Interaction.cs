using System;
using Bekk.Pact.Common.Contracts;
using Newtonsoft.Json;

namespace Bekk.Pact.Provider.Model
{
    class Interaction : IPactInformation
    {
        public string Description { get; set; }
        [JsonPropertyAttribute("provider_state")]
        public string ProviderState { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }
        public string Consumer { get; set; }
        public DateTime Created { get; set; }
    }
}