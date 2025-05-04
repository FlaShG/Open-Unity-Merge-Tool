namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine.UIElements;
    using System;

    internal class SetupTab : MergeToolTab
    {
        protected override void CreateGUI()
        {
            AddVCSDropdown();
            AddStatusLabel();
        }

        public override void UpdateContent()
        {
            
        }

        private void AddVCSDropdown()
        {
            var line = new HorizontalLayout();
            line.Add(new Label("Version control system:"));

            var button = new Button();
            button.style.flexGrow = 1;
            button.text = VersionControlSystem.GetTitle(MergeTool.Vcs);
            button.enabledSelf = MergeTool.CurrentMergeProcess == null;

            var dropdown = new GenericDropdownMenu();
            var availableSystems = TypeCache.GetTypesDerivedFrom<VersionControlSystem>();
            foreach (var vcsType in availableSystems)
            {
                dropdown.AddItem(VersionControlSystem.GetTitle(vcsType),
                    MergeTool.Vcs?.GetType() == vcsType,
                    () =>
                    {
                        MergeTool.Vcs = (VersionControlSystem)Activator.CreateInstance(vcsType);
                        UpdateContent();
                    });
            }
            button.clicked += () =>
            {
                dropdown.DropDown(button.worldBound, button, false);
            };
            line.Add(button);

            Add(line);
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
            Add(new Label($"Status: {status}"));
        }
    }
}
