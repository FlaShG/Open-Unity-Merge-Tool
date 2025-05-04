namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// A <see cref="MergeAction"/> that decides between a <see cref="Component"/>
    /// that exists on <c>our</c> branch but not on <c>their</c>s.
    /// </summary>
    internal class MergeActionOurComponent : MergeActionComponent
    {
        public override GUIContent Title => new ($"A <b>{componentType.Name}</b> only exists on <b>\"our\" branch</b>.", AssetPreview.GetMiniTypeThumbnail(componentType));
        public override string ApplyOursButtonLabel => "Keep";
        public override string ApplyTheirsButtonLabel => "Remove";
        public override bool IsUsingOurs => componentStays;

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
