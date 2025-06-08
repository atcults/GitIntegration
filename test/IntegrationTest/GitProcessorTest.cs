using GitIntegration;
using Xunit;

namespace IntegrationTest
{
    public class GitProcessorTest : UnitTestBase
    {
        public GitProcessorTest(UnitTestCore fixture) : base(fixture)
        {
            
        }

        [Fact]
        public void GitLogShouldGetFullLogs()
        {
            var gitProcessor = GitProcessor.GetProcessorForPath(RepositoryLocation);

            foreach (var elm in gitProcessor.Logs)
            {
                Logger.LogInformation(elm.ToString());
            }
            
        }

         [Fact]
        public void GitCheckIncomingChanges()
        {
            var gitProcessor = GitProcessor.GetProcessorForPath(RepositoryLocation);

            Assert.False(gitProcessor.HasReceivedIncomingChanges);
        }
    }
}