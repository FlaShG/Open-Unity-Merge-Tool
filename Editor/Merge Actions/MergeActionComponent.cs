namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    internal abstract class MergeActionComponent : MergeAction
    {
        private readonly Component component;

        protected System.Type componentType => component.GetType();

        public MergeActionComponent(GameObject target, Component component)
        {
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
