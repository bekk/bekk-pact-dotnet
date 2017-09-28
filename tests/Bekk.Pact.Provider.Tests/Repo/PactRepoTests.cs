using Bekk.Pact.Common.Contracts;
using Bekk.Pact.Provider.Config;
using Bekk.Pact.Provider.Repo;
using Xunit;
using Xunit.Abstractions;

namespace Bekk.Pact.Provider.Tests.Repo
{
    public class PactRepoTests
    {
        private readonly ITestOutputHelper output;

        public PactRepoTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("../../../Repo/TestFiles/", "timekeeper-svc", 6)]
        public void FetchAll_FromFile_ParsesJsonFilesAndReturnsOnlyPactsSpecificForProvider(string path, string provider, int expectedCount)
        {
            var config = Configuration.With.Log(System.Console.WriteLine).PublishPath(path).LogLevel(LogLevel.Verbose);
            var target = new PactRepo(config);
            var count = 0;
            foreach(var pact in target.FetchAll(provider))
            {
                Assert.NotNull(pact.ProviderState);
                count ++;
            }
            Assert.Equal(expectedCount, count);
        }
    }
}