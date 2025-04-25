namespace ThirteenPixels.OpenUnityMergeTool
{
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
        public class Property : IMergeable
        {
            public string Title => SerializedProperty.displayName;
            public string ApplyOursButtonLabel => "Apply ours";
            public string ApplyTheirsButtonLabel => "Apply theirs";

            public Resolution State { get; private set; } = Resolution.Open;

            public SerializedProperty SerializedProperty { get; }
            public object OurValue { get; }
            public object TheirValue { get; }
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
                State = Resolution.UsingOurs;
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
                State = Resolution.UsingTheirs;
                EditorRepainter.RepaintInspector();
            }

            public void AcceptNewValue()
            {
                State = Resolution.UsingNew;
            }

            ~Property()
            {
                SerializedProperty.Dispose();
            }
        }

        public override string Title => target.GetType().Name;
        public override string ApplyOursButtonLabel => "Apply all ours";
        public override string ApplyTheirsButtonLabel => "Apply all theirs";

        public override Resolution State
        {
            get
            {
                if (properties.Any(p => p.State == Resolution.Open))
                {
                    return Resolution.Open;
                }
                var firstState = properties[0].State;
                if (properties.All(p => p.State == firstState))
                {
                    return firstState;
                }
                return Resolution.UsingNew;
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
