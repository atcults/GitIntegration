using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitIntegration.Model;

namespace GitIntegration
{
    public static class GitParser
    {
        public static List<GitCommit> ListShaWithFiles(string path)
        {
            path = path.Replace("\\", "/");

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

            return commits;
        }

        public static bool StartsWithHeader(string line)
        {
            if (line.Length > 0 && char.IsLetter(line[0]))
            {
                var seq = line.SkipWhile(ch => char.IsLetter(ch) && ch != ':');
                return seq.FirstOrDefault() == ':';
            }
            return false;
        }
    }

}