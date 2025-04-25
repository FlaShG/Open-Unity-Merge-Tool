namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor.UIElements;
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

        public MergeActionLine(MergeActionCard parent, IMergeable mergeable, bool showButtons = true)
        {
            parentCard = parent;
            this.mergeable = mergeable;

            line = new HorizontalLayout();
            line.style.EnableBackgroundTransitions();
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

            Update();
        }

        private void UseOurs()
        {
            mergeable.UseOurs();
            Refresh();
        }

        private void UseTheirs()
        {
            mergeable.UseTheirs();
            Refresh();
        }

        private void Refresh()
        {
            parentCard.Refresh();
        }

        public void Update()
        {
            line.style.backgroundColor = StyleConstants.GetColorFor(mergeable.DecisionState);
        }
    }
}
