namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    /// <summary>
    /// A <see cref="MergeAction"/> that decides between a <see cref="GameObject"/>
    /// that exists on <c>our</c> branch but not on <c>their</c>s.
    /// </summary>
    internal class MergeActionOurGameObject : MergeActionGameObject
    {
        public override string Title => $"This GameObject has been <b>deleted</b> on \"their\" branch.";
        public override string ApplyOursButtonLabel => "Keep";
        public override string ApplyTheirsButtonLabel => "Remove";
        public override bool IsUsingOurs => gameObjectStays;

        public MergeActionOurGameObject(GameObject gameObject) : base(gameObject)
        {
        }

        protected override void ApplyOurs()
        {
            LetGameObjectExist();
        }

        protected override void ApplyTheirs()
        {
            RemoveGameObject();
        }
    }
}
