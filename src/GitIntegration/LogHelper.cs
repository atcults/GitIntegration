using System;
using System.Linq;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace GitIntegration
{
    public class LogHelper
    {

        public static void AddLogger(string projectName)
        {
            var configuration = ProjectConfigurationProvider.Instance;

            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new LoggingConfiguration();
            }

            var loggingConfiguration = LogManager.Configuration;

            var loggingRule = loggingConfiguration.LoggingRules.FirstOrDefault(x => x.LoggerNamePattern.Equals(projectName));

            if (loggingRule != null)
            {
                loggingConfiguration.LoggingRules.Remove(loggingRule);
            }

            var fileTarget = new FileTarget
            {
                Encoding = Encoding.UTF8,
                FileName = $"{configuration.TempPath}/Logs/{projectName}-{DateTime.Now.ToString("dd-MM-yyyy_hhmmss")}.txt",
                Layout = "${date:format=HH\\:mm\\:ss} ${logger} ${event-context:item=Dev} [${level:uppercase=true}]\t${message}. ${exception:format=ToString,StackTrace}"
            };

            loggingConfiguration.AddTarget("file", fileTarget);

            var loggingTarget = new AsyncTargetWrapper(fileTarget, 5000, AsyncTargetWrapperOverflowAction.Grow);

            loggingRule = new LoggingRule(projectName, LogLevel.Trace, loggingTarget);

            loggingConfiguration.LoggingRules.Add(loggingRule);

            LogManager.Configuration = loggingConfiguration;

            LogManager.Configuration.Reload();
        }
    }
}