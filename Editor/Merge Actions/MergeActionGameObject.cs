namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;

    internal abstract class MergeActionGameObject : MergeAction
    {
        public override sealed bool IsUsingTheirs => !IsUsingOurs;
        protected bool gameObjectStays => gameObject.hideFlags == HideFlags.None;
        private readonly GameObject gameObject;
        /// <summary>
        /// Represents the <c>activeSelf</c> value that the GameObject should have after merging.
        /// </summary>
        /// <remarks>
        /// Needed because we manipulate the GameObject's state during the merge process.
        /// </remarks>
        private readonly bool gameObjectIsActuallyActiveInScene;

        public MergeActionGameObject(GameObject gameObject)
        {
            this.gameObject = gameObject;
            gameObjectIsActuallyActiveInScene = gameObject.activeSelf;

            LetGameObjectExist();
            SetAsAutoCompleted();
        }

        protected void LetGameObjectExist()
        {
            gameObject.SetActiveForMerging(true, gameObjectIsActuallyActiveInScene);
        }

        protected void RemoveGameObject()
        {
            gameObject.SetActiveForMerging(false, gameObjectIsActuallyActiveInScene);
        }
    }
}
