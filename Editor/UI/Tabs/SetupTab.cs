namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine.UIElements;
    using System;

    internal class SetupTab : MergeToolTab
    {
        private GenericDropdownMenu vcsDropdown;
        private Button vcsDropdownButton;
        private Label statusLabel;

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

            vcsDropdownButton.text = VersionControlSystem.GetTitle(MergeTool.Vcs);

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
            statusLabel.text = $"Status: {status}";
        }

        private void AddVCSDropdownButton()
        {
            var line = new HorizontalLayout();
            line.Add(new Label("Version control system:"));

            vcsDropdownButton = new Button();
            vcsDropdownButton.style.flexGrow = 1;
            vcsDropdownButton.enabledSelf = MergeTool.CurrentMergeProcess == null;
            vcsDropdownButton.clicked += () =>
            {
                this.vcsDropdown.DropDown(vcsDropdownButton.worldBound, vcsDropdownButton, false);
            };
            line.Add(vcsDropdownButton);

            Add(line);
        }

        private void AddStatusLabel()
        {
            Add(statusLabel = new Label());
        }
    }
}
