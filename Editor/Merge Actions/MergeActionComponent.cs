namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    internal abstract class MergeActionComponent : MergeAction
    {
        protected override GameObject highlightTarget => target;
        private readonly GameObject target;
        private readonly Component component;

        protected System.Type componentType => component.GetType();

        public MergeActionComponent(GameObject target, Component component)
        {
            this.target = target;

            if (component.gameObject != target)
            {
                component = target.AddComponent(component);
            }
            this.component = component;
        }

        protected void LetComponentExist()
        {
            component.hideFlags = HideFlags.None;
        }

        protected void RemoveComponent()
        {
            component.hideFlags = HideFlags.HideAndDontSave;
        }
    }
}
