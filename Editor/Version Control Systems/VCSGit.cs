namespace ThirteenPixels.OpenUnityMergeTool
{
    [VCSInfo("Git")]
    public class VCSGit : VersionControlSystem
    {
        private const string command = "git";

        protected internal override Status GetStatus()
        {
            try
            {
                var result = RunCommand(command, "status");
                if (string.IsNullOrEmpty(result) || result.StartsWith("fatal: not a git repository"))
                {
                    return Status.NoRepo;
                }
                return Status.Okay;
            }
            catch (VCSException)
            {
                return Status.VCSNotFound;
            }
        }

        protected internal override string GetRepositoryRoot()
        {
            return RunCommandInAnyWorkingDirectory(command, "rev-parse --show-toplevel").Trim();
        }

        protected internal override FilePath[] GetAllUnmergedPaths()
        {
            var repositoryPaths = RunCommand(command, "diff --name-only --diff-filter=U").Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
            var repositoryRootPath = GetRepositoryRoot();

            var result = new FilePath[repositoryPaths.Length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = new FilePath(repositoryPaths[i], FileUtility.GetProjectLocal(repositoryPaths[i], repositoryRootPath));
            }

            return result;
        }

        protected internal override void CheckoutOurs(string path)
        {
            RunCommand(command, "checkout --ours", InQuotes(path));
        }

        protected internal override void CheckoutTheirs(string path)
        {
            RunCommand(command, "checkout --theirs", InQuotes(path));
        }

        protected internal override void MarkAsMerged(string path)
        {
            RunCommand(command, "add", InQuotes(path));
        }
    }
}
