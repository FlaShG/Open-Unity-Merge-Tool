namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.ObjectModel;

    internal abstract class MergeAction : IMergeable
    {
        public abstract GUIContent Title { get; }
        public virtual DecisionState DecisionState { get; private set; }

        public virtual ReadOnlyCollection<IMergeable> Children => null;
        public SerializedProperty SerializedProperty => null;
        public virtual string ApplyOursButtonLabel => ">>>";
        public virtual string ApplyTheirsButtonLabel => "<<<";
        public object OurValue => null;
        public object TheirValue => null;
        public abstract bool IsUsingOurs { get; }
        public abstract bool IsUsingTheirs { get; }
        public virtual bool OurValueIsPrefabDefault => false;
        public virtual bool TheirValueIsPrefabDefault => false;

        public void UseOurs()
        {
            try
            {
                ApplyOurs();
                DecisionState = DecisionState.Complete;
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
                DecisionState = DecisionState.Complete;
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
