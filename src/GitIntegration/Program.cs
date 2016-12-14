using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GitIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting console");
            var output = GitCommit.ListShaWithFiles("/code/jayhawk");

            foreach(var elm in output)
            {
                elm.Print();
            }

            Console.ReadLine();

        }

    }

    public static class ProcessRunner
    {
        public static string RunProcess(string command)
        {
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "git";
            p.StartInfo.Arguments = command;
            p.Start();
            // Read the output stream first and then wait.
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;

            /*
            
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "cmd",
                Arguments = "/C git status > /code/output/log.txt",
                WorkingDirectory = "/code/jayhawk"
            };

            Process proc = new Process() { StartInfo = startInfo, };
            proc.Start();

            proc.WaitForExit();
            
            */
        }
    }

    public static class GitParserHelper
    {
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

        public void Print()
        {
            Console.WriteLine("commit " + Sha);
            foreach (var key in Headers.Keys)
            {
                Console.WriteLine(key + ":" + Headers[key]);
            }
            Console.WriteLine();
            Console.WriteLine(Message);
            Console.WriteLine();
            foreach (var file in Files)
            {
                Console.WriteLine(file.Status + "\t" + file.File);
            }
        }

        public static List<GitCommit> ListShaWithFiles(string path)
        {
            var output = ProcessRunner.RunProcess(string.Format(" --git-dir={0}/.git --work-tree={1} log --name-status", path.Replace("\\", "/"), path.Replace("\\", "/")));

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
    }

    public class GitFileStatus
    {
        public string Status { get; set; }
        public string File { get; set; }
    }
}
