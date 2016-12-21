using System;
using NLog;

namespace GitIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting program");

            LogHelper.AddLogger("Program");

            var logger = LogManager.GetLogger("Program");

            logger.Info("Starting Process");

            var output = GitParser.ListShaWithFiles("c:\\code\\GitIntegration");

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
