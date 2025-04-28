namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    /// <summary>
    /// A <see cref="MergeAction"/> that decides between a <see cref="GameObject"/>
    /// that exists on <c>their</c> branch but not on <c>our</c>s.
    /// </summary>
    internal class MergeActionTheirGameObject : MergeActionGameObject
    {
        public override GUIContent Title => new ($"This GameObject has been <b>added</b> on their branch.", StyleConstants.Icons.GameObject);
        public override string ApplyOursButtonLabel => "Remove";
        public override string ApplyTheirsButtonLabel => "Add";
        public override bool IsUsingOurs => !gameObjectStays;


        public MergeActionTheirGameObject(GameObject gameObject) : base(gameObject)
        {
        }

        protected override void ApplyOurs()
        {
            RemoveGameObject();
        }

        protected override void ApplyTheirs()
        {
            LetGameObjectExist();
        }
    }
}
