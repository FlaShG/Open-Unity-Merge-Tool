namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;

    internal class MergeTab : MergeToolTab
    {
        private Button cancelButton;
        private Button finishButton;

        public MergeTab(VisualElement content) : base(content)
        {
            label = "Merge";
        }

        protected override void BuildUI()
        {
            if (MergeTool.CurrentMergeProcess == null)
            {
                root.Add(new Label("No merge in progress. Start a merge from the conflicts tab."));
                return;
            }

            var scrollView = new ScrollView(ScrollViewMode.Vertical);
            scrollView.style.flexGrow = 1;
            scrollView.style.marginTop = 5;
            scrollView.style.marginBottom = 5;
            root.Add(scrollView);

            foreach (var container in MergeTool.CurrentMergeProcess.mergeActionContainers)
            {
                scrollView.Add(new Label(container.Name));
                foreach (var mergeAction in container.MergeActions)
                {
                    scrollView.Add(new MergeActionCard(mergeAction));
                }
            }

            CreateFinishCancelLine();
        }

        private void CreateFinishCancelLine()
        {
            var line = new HorizontalLayout();
            line.style.alignSelf = Align.FlexEnd;
            line.style.flexShrink = 0;
            line.style.height = 30;
            line.Add(new HorizontalSpacer());

            cancelButton = new Button();
            cancelButton.text = "Cancel merge";
            //cancelButton.style.backgroundColor = StyleConstants.UnmergedColor;
            cancelButton.clicked += MergeTool.CancelCurrentMergeProgress;
            line.Add(cancelButton);

            finishButton = new Button();
            finishButton.text = "Finish merge";
            //finishButton.style.backgroundColor = StyleConstants.MergedColor;
            finishButton.clicked += MergeTool.FinishCurrentMergeProgress;
            line.Add(finishButton);

            root.Add(line);
        }
    }
}
