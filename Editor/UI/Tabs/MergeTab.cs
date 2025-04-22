namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;

    internal class MergeTab : MergeToolTab
    {
        private Button cancelButton;
        private Button finishButton;

        public MergeTab()
        {
            label = "Merge";
            EnableAutoRefresh();
        }

        protected override void BuildUI()
        {
            if (MergeTool.CurrentMergeProcess == null)
            {
                Add(new Label("No merge in progress. Start a merge from the conflicts tab."));
                return;
            }

            foreach (var container in MergeTool.CurrentMergeProcess.mergeActionContainers)
            {
                Add(new Label(container.Name));
                foreach (var mergeAction in container.MergeActions)
                {
                    Add(new MergeActionCard(mergeAction));
                }
            }

            CreateFinishCancelLine();
        }

        private void CreateFinishCancelLine()
        {
            var line = new HorizontalLayout();
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

            Add(line);
        }
    }
}
