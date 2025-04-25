namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    /// <summary>
    /// A <see cref="MergeAction"/> that decides between a <see cref="Component"/>
    /// that exists on <c>their</c> branch but not on <c>our</c>s.
    /// </summary>
    internal class MergeActionTheirComponent : MergeActionComponent
    {
        public override string Title => $"A <b>{componentType.Name}</b> component has been added on their branch.";
        public override string ApplyOursButtonLabel => "Ignore";
        public override string ApplyTheirsButtonLabel => "Add";
        public override bool IsUsingOurs => !componentStays;

        public MergeActionTheirComponent(GameObject target, Component component)
            : base(target, component)
        {
        }

        protected override void ApplyOurs()
        {
            RemoveComponent();
        }

        protected override void ApplyTheirs()
        {
            LetComponentExist();
        }
    }
}
