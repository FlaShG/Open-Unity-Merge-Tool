namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine.UIElements;
    using System;

    internal class SetupTab : MergeToolTab
    {
        private GenericDropdownMenu vcsDropdown;

        protected override void CreateGUI()
        {
            AddVCSDropdownButton();
            AddStatusLabel();
        }

        public override void UpdateContent()
        {
            vcsDropdown = new GenericDropdownMenu();
            var availableSystems = TypeCache.GetTypesDerivedFrom<VersionControlSystem>();
            foreach (var vcsType in availableSystems)
            {
                vcsDropdown.AddItem(VersionControlSystem.GetTitle(vcsType),
                    MergeTool.Vcs?.GetType() == vcsType,
                    () =>
                    {
                        MergeTool.Vcs = (VersionControlSystem)Activator.CreateInstance(vcsType);
                        MergeTool.TriggerStateChangeEvent();
                    });
            }
        }

        private void AddVCSDropdownButton()
        {
            var line = new HorizontalLayout();
            line.Add(new Label("Version control system:"));

            var button = new Button();
            button.style.flexGrow = 1;
            button.text = VersionControlSystem.GetTitle(MergeTool.Vcs);
            button.enabledSelf = MergeTool.CurrentMergeProcess == null;
            button.clicked += () =>
            {
                this.vcsDropdown.DropDown(button.worldBound, button, true, true);
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
