namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor.UIElements;
    using UnityEngine;
    using UnityEngine.UIElements;

    /// <summary>
    /// UI representation of a single <see cref="IMergeable"/>.
    /// </summary>
    internal class MergeActionLine : VisualElement
    {
        private const int applyButtonWidth = 100;

        private readonly MergeActionCard parentCard;
        private readonly IMergeable mergeable;

        private readonly VisualElement line;
        private readonly Button applyOursButton;
        private readonly Button applyTheirsButton;

        private readonly bool isHeader;

        public MergeActionLine(MergeActionCard parent, IMergeable mergeable, bool isHeader = false, bool showButtons = true)
        {
            parentCard = parent;
            this.mergeable = mergeable;

            this.isHeader = isHeader;

            line = new HorizontalLayout();
            if (!isHeader)
            {
                line.style.EnableBackgroundTransitions();
            }
            else
            {
                line.style.backgroundColor = new Color(0f, 0f, 0f, 0.3f);
            }
            Add(line);

            if (mergeable.SerializedProperty != null)
            {
                var propertyField = new PropertyField(mergeable.SerializedProperty);
                propertyField.Bind(mergeable.SerializedProperty.serializedObject);
                propertyField.style.marginTop = 5;
                // TODO Apparently not working for all types of values?
                /*
                propertyField.RegisterValueChangeCallback(evt =>
                {
                    mergeable.AcceptNewValue();
                    parentCard.Refresh();
                });
                */
                propertyField.style.flexGrow = 1;
                line.Add(propertyField);
            }
            else
            {
                var titleLabel = new Label(mergeable.Title);
                titleLabel.style.marginTop = 3;
                titleLabel.style.marginLeft = 3;
                line.Add(titleLabel);
                line.Add(new HorizontalSpacer());
            }

            if (showButtons)
            {
                applyOursButton = new Button();
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
                applyOursButton.clicked += UseOurs;
                line.Add(applyOursButton);

                applyTheirsButton = new Button();
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
                applyTheirsButton.clicked += UseTheirs;
                line.Add(applyTheirsButton);
            }

            Refresh();
        }

        private void UseOurs()
        {
            mergeable.UseOurs();
            parentCard.Refresh();
        }

        private void UseTheirs()
        {
            mergeable.UseTheirs();
            parentCard.Refresh();
        }

        public void Refresh()
        {
            if (!isHeader)
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
    }
}
