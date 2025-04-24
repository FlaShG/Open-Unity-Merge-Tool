namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine.UIElements;
    using System;

    internal class SetupTab : MergeToolTab
    {
        public SetupTab(VisualElement content) : base(content)
        {
            label = "Setup";
        }

        protected override void BuildUI()
        {
            AddVCSDropdown();
            AddStatusLabel();
        }

        private void AddVCSDropdown()
        {
            var line = new HorizontalLayout();
            line.Add(new Label("Version control system:"));

            var button = new Button();
            button.style.flexGrow = 1;
            button.text = VersionControlSystem.GetTitle(MergeTool.Vcs);

            var dropdown = new GenericDropdownMenu();
            var availableSystems = TypeCache.GetTypesDerivedFrom<VersionControlSystem>();
            foreach (var vcsType in availableSystems)
            {
                dropdown.AddItem(VersionControlSystem.GetTitle(vcsType),
                    MergeTool.Vcs?.GetType() == vcsType,
                    () =>
                    {
                        MergeTool.Vcs = (VersionControlSystem)Activator.CreateInstance(vcsType);
                        // button.text = VersionControlSystem.GetTitle(MergeTool.vcs);
                        Refresh();
                    });
            }
            button.clicked += () =>
            {
                dropdown.DropDown(button.worldBound, button, false);
            };
            line.Add(button);

            root.Add(line);
        }

        private void AddStatusLabel()
        {
            string status;
            switch (MergeTool.VcsStatus)
            {
                case VersionControlSystem.Status.Okay:
                    status = "Ready.";
                    break;
                case VersionControlSystem.Status.NoRepo:
                    status = "Project is not a repository.";
                    break;
                default:
                    status = "VCS not found";
                    break;
            }
            root.Add(new Label($"Status: {status}"));
        }
    }
}
