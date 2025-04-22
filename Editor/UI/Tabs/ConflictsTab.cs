namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;

    internal class ConflictsTab : MergeToolTab
    {
        public ConflictsTab()
        {
            label = "Conflicts";
            EnableAutoRefresh();
        }

        protected override void BuildUI()
        {
            BuildConflictList();
        }

        private void BuildConflictList()
        {
            var unmergedPaths = MergeTool.Vcs.GetAllUnmergedPaths();

            var firstResult = true;
            foreach (var path in unmergedPaths)
            {
                if (!FileUtility.IsSupportedByMergeTool(path))
                {
                    continue;
                }

                if (firstResult)
                {
                    firstResult = false;
                    Add(new Label("Detected unresolved merge conflicts:"));
                }

                var button = new Button();
                button.text = path;
                button.clicked += () => MergeTool.StartMergeProcess(path);
                Add(button);
            }

            if (firstResult)
            {
                Add(new Label("No merge conflicts detected."));
            }
        }
    }
}
