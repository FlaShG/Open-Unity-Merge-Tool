namespace ThirteenPixels.OpenUnityMergeTool
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Reflection;

    public abstract class VersionControlSystem
    {
        public enum Status
        {
            VCSNotFound, NoRepo, Okay
        }

        public readonly struct FilePath
        {
            public readonly string repositoryPath;
            public readonly string projectPath;

            public FilePath(string repositoryPath, string projectPath)
            {
                this.repositoryPath = repositoryPath;
                this.projectPath = projectPath;
            }
        }

        protected internal abstract Status GetStatus();
        protected internal abstract string GetRepositoryRoot();
        protected internal abstract FilePath[] GetAllUnmergedPaths();
        protected internal abstract void CheckoutOurs(string path);
        protected internal abstract void CheckoutTheirs(string path);
        protected internal abstract void MarkAsMerged(string path);

        protected string RunCommand(string command, params string[] parameters)
        {
            return RunCommand(command, true, parameters);
        }

        protected string RunCommandInAnyWorkingDirectory(string command, params string[] parameters)
        {
            return RunCommand(command, false, parameters);
        }

        private string RunCommand(string command, bool inRepositoryRoot, params string[] parameters)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = command;
            startInfo.Arguments = string.Join(' ', parameters);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            if (inRepositoryRoot)
            {
                startInfo.WorkingDirectory = GetRepositoryRoot();
            }
            process.StartInfo = startInfo;

            try
            {
                process.Start();
            }
            catch (Win32Exception)
            {
                throw new VCSException();
            }

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }

        internal static string GetTitle(VersionControlSystem vcs)
        {
            return vcs == null ? "None" : GetTitle(vcs.GetType());
        }

        internal static string GetTitle(Type type)
        {
            return type.GetCustomAttribute<VCSInfoAttribute>()?.title ?? type.Name;
        }

        protected static string InQuotes(string s)
        {
            return $"\"{s}\"";
        }
    }
}
