namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;
    using System.Collections.Generic;

    internal class MergeActionCard : VisualElement
    {
        private readonly MergeAction mergeAction;
        private readonly VisualElement background;
        private readonly MergeableActionLine topLine;
        private readonly List<MergeableActionLine> childrenLines;

        public MergeActionCard(MergeAction mergeAction)
        {
            this.mergeAction = mergeAction;
            style.marginTop = 4;
            style.marginBottom = 4;

            background = CreateBackground();

            var topLineHasButtons = mergeAction.Children == null || mergeAction.Children.Count > 1;
            topLine = new MergeableActionLine(this, mergeAction, showButtons: topLineHasButtons);
            background.Add(topLine);

            if (mergeAction.Children != null)
            {
                var innerBox = new Margin(10, 10, 10, 0);
                background.Add(innerBox);

                childrenLines = new();
                foreach (var child in mergeAction.Children)
                {
                    var line = new MergeableActionLine(this, child);
                    childrenLines.Add(line);
                    innerBox.Add(line);
                }
            }

            Refresh();
        }

        private Box CreateBackground()
        {
            var background = new Box();
            background.style.SetPadding(2, 4, 2, 2);
            background.style.transitionProperty = new List<StylePropertyName> { "background-color" };
            background.style.transitionDuration = new List<TimeValue> { StyleConstants.TransitionDuration };
            background.style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(EasingMode.EaseOut) };
            Add(background);
            return background;
        }

        public void Refresh()
        {
            background.style.backgroundColor = mergeAction.IsMerged ? StyleConstants.MergedColor : StyleConstants.UnmergedColor;
            topLine.Update();

            if (childrenLines != null)
            {
                foreach (var line in childrenLines)
                {
                    line.Update();
                }
            }
        }
    }
}
