namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;

    internal class ConflictsTab : MergeToolTab
    {
        public ConflictsTab(VisualElement content) : base(content)
        {
            label = "Conflicts";
        }

        protected override void CreateGUI()
        {
        }

        public override void UpdateContent()
        {
            root.Clear();
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
                    root.Add(new Label("Detected unresolved merge conflicts:"));
                }

                var button = new Button();
                button.text = path;
                button.clicked += () => MergeTool.StartMergeProcess(path);
                root.Add(button);
            }

            if (firstResult)
            {
                root.Add(new Label("No merge conflicts detected."));
            }
        }
    }
}
