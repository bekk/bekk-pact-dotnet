using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Consumer.Builders;
using Bekk.Pact.Consumer.Config;
using Bekk.Pact.Consumer.Extensions;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Bekk.Pact.Consumer.Tests.Builders
{
    public class PactBuilderTests
    {
        private readonly ITestOutputHelper output;
        public PactBuilderTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public async Task BuildPActAndRenderToJson_ReturnsString()
        {
            var pact = await PactBuilder.Build("A test pact")
                .With(Configuration.With.Log(output.WriteLine))
                .Between("Test provider").And("Test consumer")
                .WithProviderState("Some test assumptions")
                .WhenRequesting("/serviceurl/something/1")
                .WithHeader("Header", "headerValue", "Another value")
                .WithQuery("a","b")
                .WithVerb("PUT")
                .ThenRespondsWith(200)
                .WithHeader("Some reply header", "Some result")
                .WithJsonBody(new {
                    Test="ABC"
                })
                .InPact();
            var result = pact.ToString();

            Assert.NotNull(result);
            output.WriteLine(result);
        }

        [Fact]
        public async Task BuildPactWithJsonBody_AddsHeader()
        {
            var body = new { A = "B", C = new [] {"D"} };
            var baseAddress = new Uri("http://localhost:8978");
            using(var pact = await PactBuilder.Build("A test pact")
                .With(Configuration.With
                    .Log(output.WriteLine)
                    .LogLevel(LogLevel.Verbose)
                    .Comparison(StringComparison.InvariantCultureIgnoreCase)
                    .MockServiceBaseUri(baseAddress))
                .Between("Test provider").And("Test consumer")
                .WithProviderState("Some test assumptions")
                .WhenRequesting("/serviceurl/something/else/1")
                .WithVerb("POST")
                .WithJsonBody(body)
                .ThenRespondsWith(200)
                .InPact())
            {
                using(var client = new HttpClient())
                {
                    client.BaseAddress = baseAddress;
                    var response = await client.PostAsync("/serviceurl/something/else/1", new StringContent(JObject.FromObject(body).ToString() ,Encoding.UTF8, "application/json")); 
                    output.WriteLine(await response.Content.ReadAsStringAsync());
                    Assert.Equal(200, (int)response.StatusCode);
                }

                pact.Verify();
            }  
        }
    }
}