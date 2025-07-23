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
        private Button startUsingButton;
        private readonly Action onStartUsingTool;

        public SetupTab(Action onStartUsingTool)
        {
            this.onStartUsingTool = onStartUsingTool;
        }

        protected override void CreateGUI()
        {
            AddVCSDropdownButton();
            AddStatusLabel();
            AddStartUsingButton();
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

            var showStartUsingButton = MergeTool.VcsStatus == VersionControlSystem.Status.Okay;
            startUsingButton.SetEnabled(showStartUsingButton);
        }

        private void AddVCSDropdownButton()
        {
            var line = new HorizontalLayout();
            Add(line);

            line.Add(new Label("Version control system:"));

            vcsDropdownButton = new Button();
            vcsDropdownButton.enabledSelf = MergeTool.CurrentMergeProcess == null;
            vcsDropdownButton.clicked += () =>
            {
                vcsDropdown.DropDown(vcsDropdownButton.worldBound, vcsDropdownButton, false);
            };
            line.Add(vcsDropdownButton);

            line.Add(new HorizontalSpacer());
        }

        private void AddStatusLabel()
        {
            Add(statusLabel = new Label());
        }

        private void AddStartUsingButton()
        {
            startUsingButton = new Button(OnStartUsingTool)
            {
                text = "Start using the tool..."
            };

            Add(startUsingButton);
        }

        private void OnStartUsingTool()
        {
            onStartUsingTool();
        }
    }
}
