namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using System.Collections.ObjectModel;
    using UnityEditor;

    // TODO should this just produce a VisualElement instead?
    internal abstract class MergeAction : IMergeable
    {
        internal enum Resolution
        {
            Open, UsingOurs, UsingTheirs, UsingNew
        }

        public abstract string Title { get; }
        public virtual Resolution State { get; private set; }
        public bool IsMerged => State != Resolution.Open;

        public virtual ReadOnlyCollection<IMergeable> Children => null;
        public SerializedProperty SerializedProperty => null;

        protected abstract GameObject highlightTarget { get; }
        public virtual string ApplyOursButtonLabel => ">>>";
        public virtual string ApplyTheirsButtonLabel => "<<<";
        public object OurValue => null;
        public object TheirValue => null;
        public virtual bool OurValueIsPrefabDefault => false;
        public virtual bool TheirValueIsPrefabDefault => false;

        public void UseOurs()
        {
            try
            {
                ApplyOurs();
                HighlightTarget();
            }
            catch
            {
                return;
            }

            State = Resolution.UsingOurs;
        }

        public void UseTheirs()
        {
            try
            {
                ApplyTheirs();
                HighlightTarget();
            }
            catch
            {
                return;
            }

            State = Resolution.UsingTheirs;
        }

        public void AcceptNewValue()
        {
            State = Resolution.UsingNew;
        }

        public void HighlightTarget()
        {
            highlightTarget.Highlight();
        }

        protected abstract void ApplyOurs();

        protected abstract void ApplyTheirs();
    }
}
