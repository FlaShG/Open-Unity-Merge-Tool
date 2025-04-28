namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;

    internal class MergeTab : MergeToolTab
    {
        private int currentContainerIndex;
        public int CurrentContainerIndex
        {
            get => currentContainerIndex;
            set
            {
                currentContainerIndex = value;
                ShowCurrentContainer();
            }
        }
        public GameObjectMergeActionContainer CurrentContainer
        {
            get
            {
                try
                {
                    return MergeTool.CurrentMergeProcess.mergeActionContainers[CurrentContainerIndex];
                }
                catch
                {
                    return null;
                }
            }
        }

        private Button gameObjectButton;
        private ScrollView scrollView;

        public MergeTab(VisualElement content) : base(content)
        {
            label = "Merge";
            currentContainerIndex = 0;
        }

        protected override void BuildUI()
        {
            if (MergeTool.CurrentMergeProcess == null)
            {
                root.Add(new Label("No merge in progress. Start a merge from the conflicts tab."));
                return;
            }

            AddTopLine();
            AddScrollView();
            AddBottomLine();

            ShowCurrentContainer();
        }

        private void AddTopLine()
        {
            var line = new HorizontalLayout();
            line.style.flexShrink = 0;
            line.style.height = 30;

            var icon = new Image();
            icon.image = StyleConstants.Icons.GameObject;
            icon.style.width = 22;
            icon.style.height = 22;
            icon.style.marginTop = 4;
            line.Add(icon);

            gameObjectButton = new Button(() => CurrentContainer.TargetGameObject.Highlight());
            line.Add(gameObjectButton);

            line.Add(new HorizontalSpacer());

            root.Add(line);
        }

        private void AddScrollView()
        {
            scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.style.flexGrow = 1;
            scrollView.style.marginTop = 5;
            scrollView.style.marginBottom = 5;
            root.Add(scrollView);
        }

        private void AddBottomLine()
        {
            var line = new HorizontalLayout();
            line.style.alignSelf = Align.FlexEnd;
            line.style.flexShrink = 0;
            line.style.height = 30;

            if (MergeTool.CurrentMergeProcess?.mergeActionContainers.Count > 1)
            {
                var pickButton = new Button();
                pickButton.text = "Pick GameObject";
                line.Add(pickButton);

                var nextButton = new Button(ShowNextContainer);
                nextButton.text = "Next →";
                line.Add(nextButton);
            }

            line.Add(new HorizontalSpacer());

            var cancelButton = new Button(MergeTool.CancelCurrentMergeProgress);
            cancelButton.text = "Cancel merge";
            cancelButton.SetButtonColor(StyleConstants.UnmergedColor);
            line.Add(cancelButton);

            var finishButton = new Button(MergeTool.FinishCurrentMergeProgress);
            finishButton.text = "Finish merge";
            finishButton.SetButtonColor(StyleConstants.MergedColor);
            line.Add(finishButton);

            root.Add(line);
        }

        private void ShowCurrentContainer()
        {
            gameObjectButton.text = CurrentContainer.TargetGameObject.name;
            scrollView.Clear();
            foreach (var mergeAction in CurrentContainer.MergeActions)
            {
                scrollView.Add(new MergeActionCard(mergeAction));
            }
        }

        private void ShowNextContainer()
        {
            var index = currentContainerIndex;
            index++;
            if (index >= MergeTool.CurrentMergeProcess.mergeActionContainers.Count)
            {
                index = 0;
            }
            CurrentContainerIndex = index;
        }
    }
}
