namespace ThirteenPixels.OpenUnityMergeTool
{
    using System.Collections.ObjectModel;
    using UnityEditor;

    internal abstract class MergeAction : IMergeable
    {
        internal enum Resolution
        {
            Incomplete, AutoCompleted, Complete
        }

        public abstract string Title { get; }
        public virtual Resolution State { get; private set; }

        public virtual ReadOnlyCollection<IMergeable> Children => null;
        public SerializedProperty SerializedProperty => null;
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
                State = Resolution.Complete;
                EditorRepainter.RepaintInspector();
            }
            catch
            {
                return;
            }
        }

        public void UseTheirs()
        {
            try
            {
                ApplyTheirs();
                State = Resolution.Complete;
                EditorRepainter.RepaintInspector();
            }
            catch
            {
                return;
            }
        }

        public void AcceptNewValue()
        {
            EditorRepainter.RepaintInspector();
        }

        protected abstract void ApplyOurs();

        protected abstract void ApplyTheirs();
    }
}
