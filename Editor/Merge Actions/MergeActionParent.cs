namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine;

    internal class MergeActionParent : MergeAction
    {
        public override GUIContent Title => new("GameObject Parent", StyleConstants.Icons.Hierarchy);

        public override string ApplyOursButtonLabel => ourParent != null ? ourParent.name : "None";
        public override string ApplyTheirsButtonLabel => theirParent != null ? theirParent.name : "None";

        public override bool IsUsingOurs => target.transform.parent == ourParent;
        public override bool IsUsingTheirs => target.transform.parent == theirParent;

        private GameObject target;
        private readonly Transform ourParent;
        private readonly int ourSiblingIndex;
        private readonly Transform theirParent;
        private readonly int theirSiblingIndex;


        public MergeActionParent(GameObject target,
            Transform ourParent,
            int ourSiblingIndex,
            Transform theirParent,
            int theirSiblingIndex)
        {
            this.target = target;
            this.ourParent = ourParent;
            this.ourSiblingIndex = ourSiblingIndex;
            this.theirParent = theirParent;
            this.theirSiblingIndex = theirSiblingIndex;

            SetAsAutoCompleted();
        }

        protected override void ApplyOurs()
        {
            target.transform.SetParent(ourParent, true);
            target.transform.SetSiblingIndex(ourSiblingIndex);
            WarnAboutSiblingIndex();
        }

        protected override void ApplyTheirs()
        {
            target.transform.SetParent(theirParent, true);
            target.transform.SetSiblingIndex(theirSiblingIndex);
            WarnAboutSiblingIndex();
        }

        private static void WarnAboutSiblingIndex()
        {
            var content = new GUIContent("Please check the sibling order of the object.", StyleConstants.Icons.Warning);
            EditorWindow.GetWindow<MergeToolWindow>().ShowNotification(content);
        }
    }
}
