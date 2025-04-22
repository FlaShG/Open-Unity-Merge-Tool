namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    /// <summary>
    /// A <see cref="MergeAction"/> that decides between a <see cref="GameObject"/>
    /// that exists on <c>theirour</c> branch but not on <c>their</c>s.
    /// </summary>
    internal class MergeActionOurGameObject : MergeAction
    {
        public override string Title => $"Our GameObject";
        public override string ApplyOursButtonLabel => "Keep";
        public override string ApplyTheirsButtonLabel => "Remove";

        protected override GameObject highlightTarget => gameObject;
        private readonly GameObject gameObject;

        public MergeActionOurGameObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        protected override void ApplyOurs()
        {
            gameObject.SetActiveForMerging(true);
        }

        protected override void ApplyTheirs()
        {
            gameObject.SetActiveForMerging(false);
        }
    }
}
