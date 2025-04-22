namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityObject = UnityEngine.Object;

    internal abstract class MergeActionComponent : MergeAction
    {
        protected override GameObject highlightTarget => target;
        private readonly GameObject target;
        private readonly Component componentCopy;
        private Component component;

        protected System.Type componentType => componentCopy.GetType();

        public MergeActionComponent(GameObject target, Component component)
        {
            this.target = target;
            this.component = component;

            // TODO Might not work well while prefab merging
            // TODO Possibly use a single, centralized object instead
            var copyGameObject = new GameObject("MergeTool GameObject");
            copyGameObject.SetActiveForMerging(false);
            componentCopy = copyGameObject.AddComponent(component);
        }

        protected void LetComponentExist()
        {
            if (component == null)
            {
                component = target.AddComponent(componentCopy);
            }
        }

        protected void RemoveComponent()
        {
            if (component != null)
            {
                UnityObject.DestroyImmediate(component, true);
            }
        }
    }
}
