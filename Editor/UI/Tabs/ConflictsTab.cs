namespace ThirteenPixels.OpenUnityMergeTool
{
    using System.IO;
    using UnityEditor;
    using UnityEngine.UIElements;

    internal class ConflictsTab : MergeToolTab
    {
        protected override void CreateGUI()
        {
        }

        public override void UpdateContent()
        {
            Clear();
            BuildConflictList();
        }

        private void BuildConflictList()
        {
            if (MergeTool.Vcs == null) return;

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
                    Add(new Label("Detected unresolved merge conflicts:"));
                    Add(scrollView);
                }

                scrollView.Add(CreateLine(path));
            }

            if (firstResult)
            {
                Add(new Label("No merge conflicts detected."));
            }
        }

        private VisualElement CreateLine(string path)
        {
            var line = new HorizontalLayout();
            line.style.backgroundColor = StyleConstants.BackgroundLineColor;
            line.style.marginTop = 4;

            var padding = new VisualElement();
            padding.style.flexDirection = FlexDirection.Row;
            padding.style.SetPadding(8);
            line.Add(padding);

            var icon = new Image();
            icon.image = AssetDatabase.GetCachedIcon(path);
            icon.style.SetSize(18, 18);
            padding.Add(icon);

            var directory = Path.GetDirectoryName(path);
            var filename = Path.GetFileNameWithoutExtension(path);
            var label = new Label($"{directory}{Path.DirectorySeparatorChar}<b>{filename}</b>");
            label.style.SetPadding(1, 1, 4, 4);
            padding.Add(label);

            line.Add(new HorizontalSpacer());

            var button = new Button(() => MergeTool.StartMergeProcess(path));
            button.text = "Start Merging";
            button.SetButtonColor(StyleConstants.MergedColor);
            line.Add(button);

            var separator = new Label("|");
            separator.style.SetPadding(9);
            line.Add(separator);

            var useOursButton = new Button(() => MergeTool.UseOurs(path));
            useOursButton.text = "Use\nOurs";
            useOursButton.style.fontSize = 11;
            useOursButton.style.width = 45;
            line.Add(useOursButton);

            var useTheirsButton = new Button(() => MergeTool.UseTheirs(path));
            useTheirsButton.text = "Use\nTheirs";
            useTheirsButton.style.fontSize = 11;
            useTheirsButton.style.width = 45;
            line.Add(useTheirsButton);

            return line;
        }
    }
}
