namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    /// <summary>
    /// A <see cref="MergeAction"/> that decides between a <see cref="GameObject"/>
    /// that exists on <c>their</c> branch but not on <c>our</c>s.
    /// </summary>
    internal class MergeActionTheirGameObject : MergeAction
    {
        public override string Title => $"Their new GameObject";
        public override string ApplyOursButtonLabel => "Remove";
        public override string ApplyTheirsButtonLabel => "Add";

        protected override GameObject highlightTarget => gameObject;
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
