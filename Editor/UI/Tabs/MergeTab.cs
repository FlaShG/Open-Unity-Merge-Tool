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
        private Label gameObjectPathLabel;
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

                BuildPickObjectDropdown();
            }
        }

        private void AddTopLine()
        {
            var line = new HorizontalLayout();
            line.style.flexShrink = 0;
            line.style.height = 36;
            line.style.backgroundColor = StyleConstants.Colors.BackgroundLine;
            mergeUI.Add(line);

            var gameObjectButton = new Button(() => currentContainer.TargetGameObject.Highlight());
            gameObjectButton.style.SetBorder(1, StyleConstants.Colors.Highlight);
            gameObjectButton.tooltip = "Select and highlight the GameObject.";
            gameObjectButton.style.SetPadding(3, 4, 6, 6);
            line.Add(gameObjectButton);

            var icon = new Image();
            icon.image = StyleConstants.Icons.GameObject;
            icon.style.SetSize(26, 26);
            gameObjectButton.Add(icon);

            var gameObjectNamePanel = new VisualElement();
            gameObjectNamePanel.style.flexGrow = 1;
            line.Add(gameObjectNamePanel);

            gameObjectPathLabel = new Label();
            gameObjectPathLabel.style.fontSize = 11;
            gameObjectPathLabel.style.overflow = Overflow.Hidden;
            gameObjectPathLabel.style.SetMargin(1, -6, 0, 0);
            gameObjectPathLabel.style.color = StyleConstants.Colors.LightText;
            gameObjectNamePanel.Add(gameObjectPathLabel);

            gameObjectLabel = new Label();
            gameObjectLabel.style.fontSize = 18;
            gameObjectLabel.style.overflow = Overflow.Hidden;
            gameObjectLabel.style.marginTop = 6;
            gameObjectNamePanel.Add(gameObjectLabel);

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
            cancelButton.SetButtonColor(StyleConstants.Colors.Unmerged);
            line.Add(cancelButton);

            finishButton = new Button(MergeTool.FinishCurrentMergeProgress);
            finishButton.text = "Finish merge";
            finishButton.SetButtonColor(StyleConstants.Colors.Merged);
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

            currentContainer = container;
            currentContainerIndex = container == null ? 0 : MergeTool.CurrentMergeProcess.MergeActionContainers.IndexOf(container);

            scrollView.Clear();

            if (container != null)
            {
                var path = container.TargetGameObject.GetPathWithoutSelf();
                gameObjectPathLabel.text = path;
                gameObjectPathLabel.style.SetVisible(!string.IsNullOrEmpty(path));

                gameObjectLabel.text = container.TargetGameObject.name;

                foreach (var mergeAction in container.MergeActions)
                {
                    scrollView.Add(new MergeActionCard(mergeAction));
                }

                applyButtonsUI.style.SetVisible(container.MergeActions.Count > 1);
            }
            else
            {
                applyButtonsUI.style.SetVisible(false);
            }
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

        private void BuildPickObjectDropdown()
        {
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
