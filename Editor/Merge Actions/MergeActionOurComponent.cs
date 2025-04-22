namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    /// <summary>
    /// A <see cref="MergeAction"/> that decides between a <see cref="Component"/>
    /// that exists on <c>our</c> branch but not on <c>their</c>s.
    /// </summary>
    internal class MergeActionOurComponent : MergeActionComponent
    {
        public override string Title => $"Our {componentType.Name} component";
        public override string ApplyOursButtonLabel => "Keep";
        public override string ApplyTheirsButtonLabel => "Remove";

        public MergeActionOurComponent(GameObject target, Component component)
            : base(target, component)
        {
        }

        protected override void ApplyOurs()
        {
            LetComponentExist();
        }

        protected override void ApplyTheirs()
        {
            RemoveComponent();
        }
    }
}
