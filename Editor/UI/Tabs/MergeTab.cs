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
        private VisualElement applyButtonsUI;
        private Label gameObjectLabel;
        private ScrollView scrollView;
        private ProgressBar progressBar;
        private Button nextButton;
        private GenericDropdownMenu pickGameObjectDropdown;
        private Button finishButton;
        private VisualElement noMergeProgressInfo;

        protected override void CreateGUI()
        {
            noMergeProgressInfo = new Label("No merge in progress. Start a merge from the conflicts tab.");
            Add(noMergeProgressInfo);

            mergeUI = new VisualElement();
            mergeUI.style.flexGrow = 1;
            Add(mergeUI);

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

                scrollView.Clear();
                currentContainerIndex = 0;
                currentContainer = null;
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
                nextButton.SetEnabled(completed < total);

                if (currentContainer != null)
                {
                    UpdateCards();
                }
                else
                {
                    currentContainerIndex = 0;
                    ShowCurrentContainer();
                }

                pickGameObjectDropdown = new GenericDropdownMenu();
                foreach (var container in MergeTool.CurrentMergeProcess.MergeActionContainers)
                {
                    pickGameObjectDropdown.AddItem(container.Name,
                        container.IsCompleted,
                        () =>
                        {
                            ShowContainer(container);
                        });
                }
            }
        }

        private void AddTopLine()
        {
            var line = new HorizontalLayout();
            line.style.flexShrink = 0;
            line.style.height = 30;
            line.style.backgroundColor = StyleConstants.BackgroundLineColor;
            mergeUI.Add(line);

            var gameObjectButton = new Button(() => currentContainer.TargetGameObject.Highlight());
            line.Add(gameObjectButton);

            var icon = new Image();
            icon.image = StyleConstants.Icons.GameObject;
            icon.style.SetSize(22, 22);
            gameObjectButton.Add(icon);

            gameObjectLabel = new Label();
            gameObjectLabel.style.fontSize = 18;
            gameObjectLabel.style.marginTop = 4;
            line.Add(gameObjectLabel);

            line.Add(new HorizontalSpacer());

            applyButtonsUI = new VisualElement();
            applyButtonsUI.style.flexDirection = FlexDirection.Row;
            line.Add(applyButtonsUI);

            var applyOursButton = new Button(UseOurs);
            applyOursButton.text = "Apply\nOurs";
            applyOursButton.style.fontSize = 11;
            applyButtonsUI.Add(applyOursButton);

            var applyTheirsButton = new Button(UseTheirs);
            applyTheirsButton.text = "Apply\nTheirs";
            applyTheirsButton.style.fontSize = 11;
            applyButtonsUI.Add(applyTheirsButton);
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
            mergeUI.Add(line);

            multipleObjectsUI = new HorizontalLayout();
            line.Add(multipleObjectsUI);

            var pickGameObjectButton = CreatePickGameObjectButton();
            multipleObjectsUI.Add(pickGameObjectButton);

            nextButton = new Button(ShowNextIncompleteContainer);
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
        }

        private Button CreatePickGameObjectButton()
        {
            var pickButton = new Button();
            pickButton.text = "Pick GameObject";
            pickButton.clicked += () =>
            {
                pickGameObjectDropdown.DropDown(pickButton.worldBound, pickButton, false);
            };
            return pickButton;
        }

        private void ShowContainer(GameObjectMergeActionContainer container)
        {
            if (container == currentContainer) return;

            currentContainerIndex = MergeTool.CurrentMergeProcess.MergeActionContainers.IndexOf(container);
            currentContainer = container;

            gameObjectLabel.text = currentContainer.TargetGameObject.name;
            scrollView.Clear();
            foreach (var mergeAction in currentContainer.MergeActions)
            {
                scrollView.Add(new MergeActionCard(mergeAction));
            }

            applyButtonsUI.style.SetVisible(currentContainer.MergeActions.Count > 1);
        }

        private void ShowCurrentContainer()
        {
            ShowContainer(MergeTool.CurrentMergeProcess.MergeActionContainers[currentContainerIndex]);
        }

        private void ShowNextIncompleteContainer()
        {
            var containers = MergeTool.CurrentMergeProcess.MergeActionContainers;
            do
            {
                currentContainerIndex++;
                if (currentContainerIndex >= containers.Count)
                {
                    currentContainerIndex = 0;
                }
            }
            while (containers[currentContainerIndex].IsCompleted);
            ShowContainer(containers[currentContainerIndex]);
        }

        private void UpdateCards()
        {
            scrollView.Query<MergeActionCard>().ForEach(card => card.UpdateContent());
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

        private void UseOurs()
        {
            currentContainer.UseOurs();
            MergeTool.TriggerStateChangeEvent();
        }

        private void UseTheirs()
        {
            currentContainer.UseTheirs();
            MergeTool.TriggerStateChangeEvent();
        }
    }
}
