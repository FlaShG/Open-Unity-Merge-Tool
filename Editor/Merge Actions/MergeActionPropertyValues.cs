namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using UnityObject = UnityEngine.Object;

    /// <summary>
    /// A <see cref="MergeAction"/> that represents all differences in the properties of a single <see cref="UnityObject"/>.
    /// </summary>
    internal class MergeActionPropertyValues : MergeAction
    {
        /// <summary>
        /// Represents a SerializedProperty for which the two versions of an object have different values.
        /// </summary>
        public class Property : IMergeable
        {
            public GUIContent Title => new(SerializedProperty.displayName);
            public string ApplyOursButtonLabel => "Apply ours";
            public string ApplyTheirsButtonLabel => "Apply theirs";

            public DecisionState DecisionState { get; private set; } = DecisionState.Incomplete;

            public SerializedProperty SerializedProperty { get; }
            public object OurValue { get; }
            public object TheirValue { get; }
            public bool IsUsingOurs => SerializedProperty.HasValue(OurValue);
            public bool IsUsingTheirs => SerializedProperty.HasValue(TheirValue);
            public bool OurValueIsPrefabDefault { get; }
            public bool TheirValueIsPrefabDefault { get; }
            private bool ourValueIsPrefabOverride { get; }
            private bool theirValueIsPrefabOverride { get; }

            public Property(SerializedProperty ourProperty, SerializedProperty theirProperty)
            {
                SerializedProperty = ourProperty;

                OurValue = ourProperty.GetValue();
                TheirValue = theirProperty.GetValue();

                OurValueIsPrefabDefault = ourProperty.IsPrefabDefault();
                TheirValueIsPrefabDefault = theirProperty.IsPrefabDefault();
                ourValueIsPrefabOverride = ourProperty.IsPrefabOverride();
                theirValueIsPrefabOverride = theirProperty.IsPrefabOverride();
            }

            public void UseOurs()
            {
                if (OurValueIsPrefabDefault)
                {
                    SerializedProperty.SetPrefabOverride(false);
                }
                else if (ourValueIsPrefabOverride && !SerializedProperty.IsPrefabOverride() && Equals(OurValue, TheirValue))
                {
                    SerializedProperty.SetPrefabOverride(true);
                }
                SerializedProperty.SetValue(OurValue);
                DecisionState = DecisionState.Complete;
                EditorRepainter.RepaintInspector();
            }

            public void UseTheirs()
            {
                if (TheirValueIsPrefabDefault)
                {
                    SerializedProperty.SetPrefabOverride(false);
                }
                else if (theirValueIsPrefabOverride && !SerializedProperty.IsPrefabOverride() && Equals(OurValue, TheirValue))
                {
                    SerializedProperty.SetPrefabOverride(true);
                }
                SerializedProperty.SetValue(TheirValue);
                DecisionState = DecisionState.Complete;
                EditorRepainter.RepaintInspector();
            }

            public void AcceptNewValue()
            {
                DecisionState = DecisionState.Complete;
            }

            ~Property()
            {
                SerializedProperty.Dispose();
            }
        }

        /// <summary>
        /// Represents a difference in the enabled/active state between the two versions of the object.
        /// </summary>
        public class EnabledDifference : IMergeable
        {
            public GUIContent Title { get; }

            public string ApplyOursButtonLabel { get; }

            public string ApplyTheirsButtonLabel { get; }

            public DecisionState DecisionState { get; private set; } = DecisionState.Incomplete;

            public SerializedProperty SerializedProperty => null;

            public object OurValue => ourValue;

            public object TheirValue => theirValue;

            public bool IsUsingOurs => target.GetEnabled() == ourValue;

            public bool IsUsingTheirs => target.GetEnabled() == theirValue;

            // TODO Prefab defaults should be supported here as well
            public bool OurValueIsPrefabDefault => false;
            public bool TheirValueIsPrefabDefault => false;

            private readonly UnityObject target;
            private readonly bool ourValue;
            private readonly bool theirValue;

            public EnabledDifference(UnityObject target)
            {
                this.target = target;
                var oursIsEnabled = target.GetEnabled();

                Title = new GUIContent(target is GameObject ? "Active" : "Enabled");
                ApplyOursButtonLabel = oursIsEnabled ? "On" : "Off";
                ApplyTheirsButtonLabel = oursIsEnabled ? "Off" : "On";

                ourValue = oursIsEnabled ? true : false;
                theirValue = oursIsEnabled ? false : true;
            }

            public void AcceptNewValue()
            {
                DecisionState = DecisionState.Complete;
            }

            public void UseOurs()
            {
                target.SetEnabled(ourValue);
                DecisionState = DecisionState.Complete;
            }

            public void UseTheirs()
            {
                target.SetEnabled(theirValue);
                DecisionState = DecisionState.Complete;
            }
        }

        public override GUIContent Title => new($"<b>{target.GetType().Name}</b>", AssetPreview.GetMiniTypeThumbnail(target.GetType()));
        public override string ApplyOursButtonLabel => "Apply all ours";
        public override string ApplyTheirsButtonLabel => "Apply all theirs";
        public override bool IsUsingOurs => properties.All(p => p.IsUsingOurs);
        public override bool IsUsingTheirs => properties.All(p => p.IsUsingTheirs);

        public override DecisionState DecisionState
        {
            get
            {
                var hasAutocomplete = false;
                foreach (var property in properties)
                {
                    switch (property.DecisionState)
                    {
                        case DecisionState.Incomplete:
                            return DecisionState.Incomplete;
                        case DecisionState.AutoCompleted:
                            hasAutocomplete = true;
                            break;
                    }
                }
                return hasAutocomplete ? DecisionState.AutoCompleted : DecisionState.Complete;
            }
        }

        public override ReadOnlyCollection<IMergeable> Children { get; }

        private readonly UnityObject target;
        private readonly List<IMergeable> properties = new();

        public MergeActionPropertyValues(UnityObject target)
        {
            this.target = target;
            Children = properties.AsReadOnly();
        }

        public void AddProperty(SerializedProperty ourProperty, SerializedProperty theirProperty)
        {
            properties.Add(new Property(ourProperty, theirProperty));
            theirProperty.Dispose();
        }

        public void SetEnabledDifference()
        {
            properties.Insert(0, new EnabledDifference(target));
        }

        public void SortPropertiesAlphabetically()
        {
            properties.Sort((a, b) => a.SerializedProperty.name.CompareTo(b.SerializedProperty.name));
        }

        protected override void ApplyOurs()
        {
            foreach (var property in properties)
            {
                property.UseOurs();
            }
        }

        protected override void ApplyTheirs()
        {
            foreach (var property in properties)
            {
                property.UseTheirs();
            }
        }
    }
}
