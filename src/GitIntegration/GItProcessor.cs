using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GitIntegration.Model;

//Reference: http://blog.somewhatabstract.com/2015/06/22/getting-information-about-your-git-repository-with-c/

namespace GitIntegration
{
    public class GitProcessor : IDisposable
    {
        public static GitProcessor GetProcessorForPath(string path, string gitPath = null)
        {
            var gitProcessor = new GitProcessor(path.Replace("\\", "/"), gitPath);

            if (gitProcessor.IsGitRepository)
            {
                return gitProcessor;
            }
            return null;
        }

        public string CommitHash
        {
            get
            {
                return RunCommand("rev-parse HEAD");
            }
        }

        public string BranchName
        {
            get
            {
                return RunCommand("rev-parse --abbrev-ref HEAD");
            }
        }

        public string TrackedBranchName
        {
            get
            {
                return RunCommand("rev-parse --abbrev-ref --symbolic-full-name @{u}");
            }
        }

        public bool HasUnpushedCommits
        {
            get
            {
                return !String.IsNullOrWhiteSpace(RunCommand("log @{u}..HEAD"));
            }
        }

        public bool HasUncommittedChanges
        {
            get
            {
                return !String.IsNullOrWhiteSpace(RunCommand("status --porcelain"));
            }
        }

        public bool HasReceivedIncomingChanges
        {
            get
            {
                var result = RunCommand("pull");
                if (string.IsNullOrWhiteSpace(result))
                {
                    return false;
                }
                if (result.StartsWith("There is no tracking information") ||
                    result.StartsWith("fatal") ||
                    result.StartsWith("error"))
                {
                    return false;
                }
                return !result.StartsWith("Already up-to-date.");
            }
        }

        public IEnumerable<GitCommit> Logs
        {
            get
            {
                GitCommit commit = null;
                        
                int skip = 0;
                while (true)
                {
                    string entry = RunCommand(String.Format("log --skip={0} -n1 --name-status", skip++));
                    if (String.IsNullOrWhiteSpace(entry))
                    {
                        yield break;
                    }

                    bool processingMessage = false;
                    using (var strReader = new StringReader(entry))
                    {
                        do
                        {
                            var line = strReader.ReadLine();

                            if (line.StartsWith("commit "))
                            {
                                commit = new GitCommit();
                                commit.Sha = line.Split(' ')[1];

                            }

                            if (StartsWithHeader(line))
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

                            if (line.Length > 0 && (line[0] == '\t' || line.StartsWith("  ")))
                            {
                                // commit message.
                                commit.Message += line;
                            }

                            if (line.Length > 1 && char.IsLetter(line[0]) && line[1] == '\t')
                            {
                                var status = line.Split('\t')[0];
                                var file = line.Split('\t')[1];
                                commit.Files.Add(new GitFileStatus() { Status = status, File = file });
                            }
                        }
                        while (strReader.Peek() != -1);
                    }

                    yield return commit;
                }
            }
        }

        public void Dispose()
        {
            // Nothing to dispose for now. Method kept for backward
            // compatibility with the IDisposable interface.
        }

        private GitProcessor(string path, string gitPath)
        {
            _processInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = string.IsNullOrWhiteSpace(gitPath) ? "git" : gitPath,
                CreateNoWindow = true,
                WorkingDirectory = path
            };
        }

        private bool IsGitRepository
        {
            get
            {
                return !String.IsNullOrWhiteSpace(RunCommand("log -1"));
            }
        }

        private static bool StartsWithHeader(string line)
        {
            if (line.Length > 0 && char.IsLetter(line[0]))
            {
                var seq = line.SkipWhile(ch => char.IsLetter(ch) && ch != ':');
                return seq.FirstOrDefault() == ':';
            }
            return false;
        }

        private string RunCommand(string args)
        {
            var psi = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                FileName = _processInfo.FileName,
                CreateNoWindow = true,
                WorkingDirectory = _processInfo.WorkingDirectory,
                Arguments = args
            };

            using (var process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                output += process.StandardError.ReadToEnd();
                output = output.Trim();
                process.WaitForExit();
                return output;
            }
        }

        private readonly ProcessStartInfo _processInfo;
    }
}