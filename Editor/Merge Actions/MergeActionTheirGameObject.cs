namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    /// <summary>
    /// A <see cref="MergeAction"/> that decides between a <see cref="GameObject"/>
    /// that exists on <c>their</c> branch but not on <c>our</c>s.
    /// </summary>
    internal class MergeActionTheirGameObject : MergeAction
    {
        public override string Title => $"This GameObject has been <b>added</b> on their branch.";
        public override string ApplyOursButtonLabel => "Remove";
        public override string ApplyTheirsButtonLabel => "Add";

        private readonly GameObject gameObject;

        public MergeActionTheirGameObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        protected override void ApplyOurs()
        {
            gameObject.SetActiveForMerging(false);
        }

        protected override void ApplyTheirs()
        {
            gameObject.SetActiveForMerging(true);
        }
    }
}
