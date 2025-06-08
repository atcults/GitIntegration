using GitIntegration;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IntegrationTest
{
    public class GitLogTest : UnitTestBase
    {
        public GitLogTest(UnitTestCore fixture) : base(fixture)
        {
        }

        [Fact]
        public void GitLogShouldGetFullLogs()
        {
            var output = GitParser.ListShaWithFiles(RepositoryLocation);

            foreach (var elm in output)
            {
                Logger.LogInformation(elm.ToString());
            }

            Assert.True(output.Count > 0);
        }
    }
}