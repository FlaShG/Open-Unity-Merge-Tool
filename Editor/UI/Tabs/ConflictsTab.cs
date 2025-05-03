namespace ThirteenPixels.OpenUnityMergeTool
{
    using System.IO;
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

            var scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.style.flexGrow = 1;

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
                    root.Add(scrollView);
                }

                scrollView.Add(CreateLine(path));
            }

            if (firstResult)
            {
                root.Add(new Label("No merge conflicts detected."));
            }
        }

        private VisualElement CreateLine(string path)
        {
            var line = new HorizontalLayout();
            line.style.backgroundColor = StyleConstants.BackgroundLineColor;
            line.style.marginTop = 4;

            var padding = new VisualElement();
            padding.style.flexDirection = FlexDirection.Row;
            padding.style.SetPadding(6);
            line.Add(padding);

            var icon = new Image();
            if (FileUtility.IsScene(path))
            {
                icon.image = StyleConstants.Icons.Scene;
            }
            else if (FileUtility.IsPrefab(path))
            {
                icon.image = StyleConstants.Icons.Prefab;
            }
            icon.style.SetSize(18, 18);
            padding.Add(icon);

            var directory = Path.GetDirectoryName(path);
            var filename = Path.GetFileNameWithoutExtension(path);
            var label = new Label($"{directory}{Path.DirectorySeparatorChar}<b>{filename}</b>");
            label.style.SetPadding(1, 1, 4, 4);
            padding.Add(label);

            line.Add(new HorizontalSpacer());

            var button = new Button();
            button.text = "Merge";
            button.clicked += () => MergeTool.StartMergeProcess(path);
            line.Add(button);

            return line;
        }
    }
}
