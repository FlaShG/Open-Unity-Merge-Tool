namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using System.Collections.Generic;

    /// <summary>
    /// UI represantation of a single component or GameObject.<br/>
    /// Contains one or more <see cref="MergeActionLine"/>s that represent the conflicts with that object.
    /// </summary>
    internal class MergeActionCard : VisualElement
    {
        private readonly MergeAction mergeAction;
        private readonly VisualElement background;
        private readonly MergeActionLine topLine;
        private readonly List<MergeActionLine> childrenLines;

        public MergeActionCard(MergeAction mergeAction)
        {
            this.mergeAction = mergeAction;
            style.marginTop = 2;
            style.marginBottom = 2;
            style.SetBorder(1f, new Color(0.2f, 0f, 0f));

            background = CreateBackground();

            var topLineIsHeader = mergeAction.Children != null && mergeAction.Children.Count > 0;
            var topLineHasButtons = mergeAction.Children == null || mergeAction.Children.Count > 1;
            topLine = new MergeActionLine(mergeAction,
                topLineIsHeader ? MergeActionLine.Type.Header : MergeActionLine.Type.SingleLine,
                showButtons: topLineHasButtons);
            background.Add(topLine);

            if (mergeAction.Children != null)
            {
                var innerBox = new Margin(4, 4, 8, 0);
                background.Add(innerBox);

                childrenLines = new();
                foreach (var child in mergeAction.Children)
                {
                    var line = new MergeActionLine(child, MergeActionLine.Type.Child);
                    childrenLines.Add(line);
                    innerBox.Add(line);
                }
            }

            UpdateContent();
        }

        private Box CreateBackground()
        {
            var background = new Box();
            background.style.SetPadding(2, 4, 2, 2);
            background.style.EnableBackgroundTransitions();
            Add(background);
            return background;
        }

        public void UpdateContent()
        {
            background.style.backgroundColor = StyleConstants.GetColorFor(mergeAction.DecisionState);
            topLine.Refresh();

            if (childrenLines != null)
            {
                foreach (var line in childrenLines)
                {
                    line.Refresh();
                }
            }
        }
    }
}
