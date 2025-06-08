using System;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GitIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting program");

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "HH:mm:ss ";
                });
            });
            var logger = loggerFactory.CreateLogger("Program");

            logger.LogInformation("Starting Process");

            var output = GitParser.ListShaWithFiles("c:\\code\\GitIntegration");

            foreach (var elm in output)
            {
                var json = JsonSerializer.Serialize(elm, new JsonSerializerOptions { WriteIndented = true });
                logger.LogInformation(json);
                Console.WriteLine(json);
            }

            Console.WriteLine("Output generated");
        }
    }
}
