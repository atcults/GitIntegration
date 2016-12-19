using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

            var output = GitParserHelper.ListShaWithFiles(ProjectDetail);

            foreach (var elm in output)
            {
                logger.Info(elm.ToString());
            }

            Console.WriteLine("Output generated");

        }

    }

    public static class ProcessRunner
    {
        public static string RunProcess(string command, string args)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = args;
            p.Start();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }
    }

    public static class GitParserHelper
    {
        public static List<GitCommit> ListShaWithFiles(ProjectDetail detail)
        {
            var path = detail.Location.Replace("\\", "/");

            var output = ProcessRunner.RunProcess("git", string.Format($" --git-dir={path}/.git --work-tree={path} log --name-status"));

            var commits = new List<GitCommit>();
            bool processingMessage = false;
            using (var strReader = new StringReader(output))
            {
                GitCommit commit = null;
                do
                {
                    var line = strReader.ReadLine();

                    if (line.StartsWith("commit "))
                    {
                        commit = new GitCommit();
                        commits.Add(commit);
                        commit.Sha = line.Split(' ')[1];
                    }

                    if (GitParserHelper.StartsWithHeader(line))
                    {
                        var header = line.Split(':')[0];
                        var val = string.Join(":", line.Split(':').Skip(1)).Trim();

                        // headers
                        commit.Headers.Add(header, val);
                    }

                    if (string.IsNullOrEmpty(line))
                    {
                        // commit message divider
                        processingMessage = !processingMessage;
                    }

                    if (line.Length > 0 && line[0] == '\t')
                    {
                        // commit message.
                        commit.Message += line;
                    }

                    if (line.Length > 1 && Char.IsLetter(line[0]) && line[1] == '\t')
                    {
                        var status = line.Split('\t')[0];
                        var file = line.Split('\t')[1];
                        commit.Files.Add(new GitFileStatus() { Status = status, File = file });
                    }
                }
                while (strReader.Peek() != -1);
            }

            return commits;
        }

        public static bool StartsWithHeader(string line)
        {
            if (line.Length > 0 && char.IsLetter(line[0]))
            {
                var seq = line.SkipWhile(ch => Char.IsLetter(ch) && ch != ':');
                return seq.FirstOrDefault() == ':';
            }
            return false;
        }
    }

    public class GitCommit
    {
        public GitCommit()
        {
            Headers = new Dictionary<string, string>();
            Files = new List<GitFileStatus>();
            Message = "";
        }

        public Dictionary<string, string> Headers { get; set; }
        public string Sha { get; set; }
        public string Message { get; set; }
        public List<GitFileStatus> Files { get; set; }

        public override string ToString()
        {
            var logString = new StringBuilder();

            logString.AppendLine("commit " + Sha);

            foreach (var key in Headers.Keys)
            {
                logString.AppendLine(key + ":" + Headers[key]);
            }

            logString.AppendLine(Message);

            foreach (var file in Files)
            {
                logString.AppendLine(file.Status + "\t" + file.File);
            }

            return logString.ToString();
        }


    }

    public class GitFileStatus
    {
        public string Status { get; set; }
        public string File { get; set; }
    }
}
