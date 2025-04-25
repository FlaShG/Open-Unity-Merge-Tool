namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    internal abstract class MergeActionGameObject : MergeAction
    {
        public override sealed bool IsUsingTheirs => !IsUsingOurs;
        protected bool gameObjectStays => gameObject.hideFlags == HideFlags.None;
        private readonly GameObject gameObject;

        public MergeActionGameObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        protected void LetGameObjectExist()
        {
            gameObject.SetActiveForMerging(true);
        }

        protected void RemoveGameObject()
        {
            gameObject.SetActiveForMerging(false);
        }
    }
}
