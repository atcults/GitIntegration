using System;
using NLog;
using System.Text.Json;

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
                var json = JsonSerializer.Serialize(elm, new JsonSerializerOptions { WriteIndented = true });
                logger.Info(json);
                Console.WriteLine(json);
            }

            LogManager.Flush();
            LogManager.Shutdown();

            Console.WriteLine("Output generated");

        }
    }
}
