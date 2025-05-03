namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// UI representation of a single <see cref="IMergeable"/>.
    /// </summary>
    internal class MergeActionLine : VisualElement
    {
        public enum Type
        {
            Header, SingleLine, Child
        }

        private const int applyButtonWidth = 100;

        private readonly IMergeable mergeable;

        private readonly VisualElement line;
        private readonly Button applyOursButton;
        private readonly Button applyTheirsButton;

        private readonly Type type;

        public MergeActionLine(IMergeable mergeable, Type type, bool showButtons = true)
        {
            this.mergeable = mergeable;
            this.type = type;

            style.height = 26;

            line = new HorizontalLayout();
            if (type == Type.Header)
            {
                line.style.backgroundColor = new Color(0f, 0f, 0f, 0.3f);
                line.style.height = 22;
                line.style.SetPadding(3, 3, 4, 4);
            }
            else
            {
                if (type == Type.SingleLine)
                {
                    line.style.SetPadding(3, 3, 4, 4);
                }
                line.style.EnableBackgroundTransitions();
            }
            Add(line);

            if (mergeable.SerializedProperty != null)
            {
                line.Add(new PropertyField(mergeable, SendUpdateEvent));
            }
            else
            {
                if (mergeable.Title.image != null)
                {
                    var titleIcon = new Image();
                    titleIcon.image = mergeable.Title.image;
                    titleIcon.style.flexGrow = 0;
                    titleIcon.style.flexShrink = 0;
                    titleIcon.style.width = 16;
                    titleIcon.style.height = 16;
                    titleIcon.style.alignSelf = Align.Center;
                    line.Add(titleIcon);
                }

                var titleLabel = new Label(mergeable.Title.text);
                titleLabel.style.alignSelf = Align.Center;
                line.Add(titleLabel);

                line.Add(new HorizontalSpacer());
            }

            if (showButtons)
            {
                applyOursButton = new Button(UseOurs);
                if (mergeable.OurValueIsPrefabDefault)
                {
                    applyOursButton.style.SetBorder(1.5f, StyleConstants.PrefabColor);
                }
                if (mergeable.OurValue != null)
                {
                    var ourValuePreview = PropertyValuePreviewFactory.GetPreview(mergeable.OurValue);
                    ourValuePreview.style.flexGrow = 1;
                    applyOursButton.Add(ourValuePreview);
                }
                else
                {
                    applyOursButton.text = mergeable.ApplyOursButtonLabel;
                }
                applyOursButton.style.width = applyButtonWidth;
                line.Add(applyOursButton);

                applyTheirsButton = new Button(UseTheirs);
                if (mergeable.TheirValueIsPrefabDefault)
                {
                    applyTheirsButton.style.SetBorder(1.5f, StyleConstants.PrefabColor);
                }
                if (mergeable.TheirValue != null)
                {
                    var theirValuePreview = PropertyValuePreviewFactory.GetPreview(mergeable.TheirValue);
                    theirValuePreview.style.flexGrow = 1;
                    applyTheirsButton.Add(theirValuePreview);
                }
                else
                {
                    applyTheirsButton.text = mergeable.ApplyTheirsButtonLabel;
                }
                applyTheirsButton.style.width = applyButtonWidth;
                line.Add(applyTheirsButton);

                if (type == Type.Header)
                {
                    applyOursButton.style.fontSize = 11;
                    applyOursButton.style.SetMargin(0, 0, 3, 3);

                    applyTheirsButton.style.fontSize = 11;
                    applyTheirsButton.style.SetMargin(0, 0, 3, 3);
                }
            }

            Refresh();
        }

        public void Refresh()
        {
            if (type != Type.Header)
            {
                line.style.backgroundColor = StyleConstants.GetColorFor(mergeable.DecisionState);
            }

            if (mergeable.DecisionState != DecisionState.Incomplete)
            {
                if (applyOursButton != null && applyTheirsButton != null)
                {
                    if (mergeable.IsUsingOurs)
                    {
                        applyOursButton.SetButtonColor(StyleConstants.MergedColor);
                    }
                    else
                    {
                        applyOursButton.ResetButtonColor();
                    }
                    if (mergeable.IsUsingTheirs)
                    {
                        applyTheirsButton.SetButtonColor(StyleConstants.MergedColor);
                    }
                    else
                    {
                        applyTheirsButton.ResetButtonColor();
                    }
                }
            }
        }

        private void UseOurs()
        {
            mergeable.UseOurs();
            SendUpdateEvent();
        }

        private void UseTheirs()
        {
            mergeable.UseTheirs();
            SendUpdateEvent();
        }

        private void SendUpdateEvent()
        {
            MergeTool.UpdateAfterMergeStateChange();
        }
    }
}
