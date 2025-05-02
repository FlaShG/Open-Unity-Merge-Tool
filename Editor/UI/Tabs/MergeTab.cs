namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine.UIElements;

    internal class MergeTab : MergeToolTab
    {
        private int currentContainerIndex;
        private GameObjectMergeActionContainer currentContainer;

        private VisualElement mergeUI;
        private VisualElement multipleObjectsUI;
        private Button gameObjectButton;
        private ScrollView scrollView;
        private ProgressBar progressBar;
        private Button finishButton;
        private VisualElement noMergeProgressInfo;

        public MergeTab(VisualElement content) : base(content)
        {
            label = "Merge";
            currentContainerIndex = 0;
        }

        protected override void CreateGUI()
        {
            noMergeProgressInfo = new Label("No merge in progress. Start a merge from the conflicts tab.");
            root.Add(noMergeProgressInfo);

            mergeUI = new VisualElement();
            mergeUI.style.flexGrow = 1;
            root.Add(mergeUI);

            AddTopLine();
            AddScrollView();
            AddBottomLine();
        }

        public override void UpdateContent()
        {
            if (MergeTool.CurrentMergeProcess == null)
            {
                noMergeProgressInfo.style.Show();
                mergeUI.style.Hide();
            }
            else
            {
                noMergeProgressInfo.style.Hide();
                mergeUI.style.Show();

                var total = MergeTool.CurrentMergeProcess.MergeActionContainers.Count;
                var completed = MergeTool.CurrentMergeProcess.CompletedMergeActionContainerCount;

                multipleObjectsUI.visible = total > 1;

                progressBar.highValue = total;
                progressBar.lowValue = completed;
                progressBar.Q<Label>().text = $"{completed} / {total}";

                finishButton.SetEnabled(completed == total);

                if (currentContainer == null)
                {
                    currentContainerIndex = 0;
                    ShowCurrentContainer();
                }
            }
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

            gameObjectButton = new Button(() => currentContainer.TargetGameObject.Highlight());
            line.Add(gameObjectButton);

            line.Add(new HorizontalSpacer());

            mergeUI.Add(line);
        }

        private void AddScrollView()
        {
            scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.style.flexGrow = 1;
            scrollView.style.marginTop = 5;
            scrollView.style.marginBottom = 5;
            mergeUI.Add(scrollView);
        }

        private void AddBottomLine()
        {
            var line = new HorizontalLayout();
            line.style.alignSelf = Align.FlexEnd;
            line.style.flexShrink = 0;
            line.style.height = 30;

            multipleObjectsUI = new HorizontalLayout();
            line.Add(multipleObjectsUI);

            var pickButton = new Button();
            pickButton.text = "Pick GameObject";
            multipleObjectsUI.Add(pickButton);

            var nextButton = new Button(ShowNextContainer);
            nextButton.text = "Next →";
            multipleObjectsUI.Add(nextButton);

            progressBar = new ProgressBar();
            progressBar.style.flexGrow = 1;
            progressBar.style.marginTop = 5;
            progressBar.EnableProgressBarAnimation();
            multipleObjectsUI.Add(progressBar);

            var cancelButton = new Button(CancelCurrentMergeProgress);
            cancelButton.text = "Cancel merge";
            cancelButton.SetButtonColor(StyleConstants.UnmergedColor);
            line.Add(cancelButton);

            finishButton = new Button(MergeTool.FinishCurrentMergeProgress);
            finishButton.text = "Finish merge";
            finishButton.SetButtonColor(StyleConstants.MergedColor);
            line.Add(finishButton);

            mergeUI.Add(line);
        }

        private void ShowContainer(GameObjectMergeActionContainer container)
        {
            if (container == currentContainer) return;

            currentContainer = container;
            gameObjectButton.text = currentContainer.TargetGameObject.name;
            scrollView.Clear();
            foreach (var mergeAction in currentContainer.MergeActions)
            {
                scrollView.Add(new MergeActionCard(mergeAction));
            }
        }

        private void ShowCurrentContainer()
        {
            ShowContainer(MergeTool.CurrentMergeProcess.MergeActionContainers[currentContainerIndex]);
        }

        private void ShowNextContainer()
        {
            currentContainerIndex++;
            if (currentContainerIndex >= MergeTool.CurrentMergeProcess.MergeActionContainers.Count)
            {
                currentContainerIndex = 0;
            }
            ShowContainer(MergeTool.CurrentMergeProcess.MergeActionContainers[currentContainerIndex]);
        }

        private void CancelCurrentMergeProgress()
        {
            if (MergeTool.CurrentMergeProcess.CompletedMergeActionContainerCount > 0)
            {
                var result = EditorUtility.DisplayDialog(DialogConstants.title, "Do you really want to cancel the current merge process?", "Yes", "No, continue");

                if (result == false)
                {
                    return;
                }
            }

            MergeTool.CancelCurrentMergeProgress();
        }
    }
}
