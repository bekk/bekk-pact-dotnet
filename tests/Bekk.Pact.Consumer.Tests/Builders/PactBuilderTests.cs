using System;
using System.Collections.Generic;
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
        public async Task BuildPactAndRenderToJson_ReturnsString()
        {
            using(var pact = await PactBuilder.Build("A test pact")
                .With(Configuration.With.Log(output.WriteLine))
                .Between("Test provider").And("Test consumer")
                .WithProviderState("Some test assumptions")
                .WhenRequesting("/serviceurl/something/1")
                .WithHeader("Header", "headerValue", "Another value")
                .WithQuery("a","b")
                .WithVerb("PUT")
                .ThenRespondsWith(200)
                .WithHeader("Some_reply-header", "Some result")
                .WithJsonBody(new {
                    Test="ABC"
                })
                .InPact())
            {
                var result = pact.ToString();

                Assert.NotNull(result);
                output.WriteLine(result);
            }
        }

        [Fact(Skip = "Fails in build. Do not why")]
        public async Task BuildPactWithUrlEncodedFormDataAndRenderToJson_ServerReplies()
        {
            var data = new Dictionary<string,string>{{"A","Some text, & possibly escaped."},{"C","3"},{"d","3.567"}};
            var baseAddress = new Uri("http://localhost:8978");
            var url = "/serviceurl/whatever";
            using(var pact = await PactBuilder.Build("A test pact fith form data")
                .With(Configuration.With.Log(output.WriteLine).MockServiceBaseUri(baseAddress))
                .Between("Test provider").And("Test consumer")
                .WithProviderState("Some other test assumptions")
                .WhenRequesting(url)
                .WithVerb("POST")
                //.WithUrlEncodedFormData(data)
                .WithUrlEncodedFormData(FormData.With("A", "Some text, & possibly escaped.").And("C", 3).And("d", 3.567))
                .ThenRespondsWith(200)
                .WithHeader("Some-reply-header", "Some result")
                .InPact())
            {
                using(var client = new HttpClient())
                {
                    client.BaseAddress = baseAddress;
                    var content = new FormUrlEncodedContent(data);
                    var response = await client.PostAsync(url, content); 
                    output.WriteLine(await response.Content.ReadAsStringAsync());
                    Assert.Equal(200, (int)response.StatusCode);
                }

                pact.Verify();
            }
        }

        [Fact]
        public async Task BuildPactWithJsonBody_AddsHeader()
        {
            var body = new { A = "B", C = new [] {"D"} };
            var baseAddress = new Uri("http://localhost:8958");
            const string url = "/serviceurl/something/else/1";
            using (var pact = await PactBuilder.Build("A test pact")
                .With(Configuration.With
                    .Log(output.WriteLine)
                    .LogLevel(LogLevel.Verbose)
                    .Comparison(StringComparison.InvariantCultureIgnoreCase)
                    .MockServiceBaseUri(baseAddress))
                .Between("Test provider").And("Test consumer")
                .WithProviderState("Yet some test assumptions")
                .WhenRequesting(url)
                .WithVerb("POST")
                .WithJsonBody(body)
                .ThenRespondsWith(200)
                .InPact())
            {
                using(var client = new HttpClient{BaseAddress=baseAddress})
                {
                    client.DefaultRequestHeaders.Add("Connection", "close");
                    var jsonBody = JObject.FromObject(body).ToString();
                    var content = new StringContent(jsonBody , Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(url, content); 
                    output.WriteLine(await response.Content.ReadAsStringAsync());
                    Assert.Equal(200, (int)response.StatusCode);
                }

                pact.Verify();
            }  
        }
    }
}