namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor.SceneManagement;
    using System.Collections.Generic;

    internal abstract class MergeProcess
    {
        protected const string oursSuffix = "--ours";
        protected const string theirsSuffix = "--theirs";

        protected readonly DualSourceGameObjectDictionary gameObjectDictionary = new();
        /// <summary>
        /// The path of the file this process is merging.
        /// </summary>
        protected readonly string path;

        internal List<GameObjectMergeActionContainer> mergeActionContainers { get; private set; }

        protected MergeProcess(string path)
        {
            this.path = path;
        }

        public void Start()
        {
            var sceneHierarchyIsClean = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (sceneHierarchyIsClean)
            {
                mergeActionContainers = StartProcess();
            }
            else
            {
                throw new MergeProcessException("Unsaved changes in an open scene.");
            }

            if (mergeActionContainers == null)
            {
                throw new MergeProcessException("Merge Actions were not generated.");
            }
        }

        public abstract void Finish();

        public abstract void Cancel();

        protected abstract List<GameObjectMergeActionContainer> StartProcess();
    }
}
