using System;
using GitIntegration;
using NLog;
using Xunit;

namespace IntegrationTest
{
    public class GitLogTest
    {
        [Fact]
        public void GitLogShouldGetFullLogs()
        {
            Console.WriteLine("Getting configuration");

            var configuration = ProjectConfigurationProvider.Instance;

            var ProjectDetail = configuration.Projects[0];

            LogHelper.AddLogger(ProjectDetail.Name);

            var logger = LogManager.GetLogger(ProjectDetail.Name);

            logger.Info($"Project Name {ProjectDetail.Name} and Location is {ProjectDetail.Location}");

            var output = GitParser.ListShaWithFiles(ProjectDetail);

            foreach (var elm in output)
            {
                logger.Info(elm.ToString());
            }

            Assert.True(output.Count > 0);

            LogManager.Flush();
            LogManager.Shutdown();
            
        }
    }
}