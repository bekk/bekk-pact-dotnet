using System.Threading.Tasks;
using Bekk.Pact.Consumer.Builders;
using Bekk.Pact.Consumer.Config;
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
                .WithBody(new {
                    Test="ABC"
                })
                .InPact();
            var result = pact.ToString();

            Assert.NotNull(result);
            output.WriteLine(result);
        }
    }
}