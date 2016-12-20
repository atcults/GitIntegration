using System;
using NLog;

namespace GitIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting program");

            var configuration = ProjectConfigurationProvider.Instance;

            var ProjectDetail = configuration.Projects[0];

            LogHelper.AddLogger(ProjectDetail.Name);

            var logger = LogManager.GetLogger(ProjectDetail.Name);

            logger.Info("Starting Process");

            var output = GitParser.ListShaWithFiles(ProjectDetail);

            foreach (var elm in output)
            {
                logger.Info(elm.ToString());
                Console.WriteLine(elm.ToString());
            }

            LogManager.Flush();
            LogManager.Shutdown();

            Console.WriteLine("Output generated");

        }
    }
}
