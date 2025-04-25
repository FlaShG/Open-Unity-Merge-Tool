namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    internal abstract class MergeActionComponent : MergeAction
    {
        public override sealed bool IsUsingTheirs => !IsUsingOurs;
        protected System.Type componentType => component.GetType();
        protected bool componentStays => component.hideFlags == HideFlags.None;

        private readonly Component component;


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
