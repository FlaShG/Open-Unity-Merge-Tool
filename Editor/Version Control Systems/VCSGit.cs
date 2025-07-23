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

        protected internal override string[] GetAllUnmergedPaths()
        {
            var repositoryPaths = RunCommand(command, "diff --name-only --diff-filter=U").Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
            var repositoryRootPath = RunCommand(command, "rev-parse --show-toplevel").Trim();

            for (var i = 0; i < repositoryPaths.Length; i++)
            {
                repositoryPaths[i] = FileUtility.GetProjectLocal(repositoryPaths[i], repositoryRootPath);
            }

            return repositoryPaths;
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
