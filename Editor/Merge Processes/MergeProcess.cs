namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor.SceneManagement;
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class MergeProcess
    {
        protected const string oursSuffix = "--ours";
        protected const string theirsSuffix = "--theirs";

        protected readonly DualSourceGameObjectDictionary gameObjectDictionary = new();
        /// <summary>
        /// The path of the file this process is merging.
        /// </summary>
        protected readonly VersionControlSystem.FilePath path;

        public List<GameObjectMergeActionContainer> MergeActionContainers { get; private set; }
        public int CompletedMergeActionContainerCount => MergeActionContainers.Where(container => container.IsCompleted).Count();

        protected MergeProcess(VersionControlSystem.FilePath path)
        {
            this.path = path;
        }

        public void Start()
        {
            var sceneHierarchyIsClean = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (sceneHierarchyIsClean)
            {
                MergeActionContainers = StartProcess();
            }
            else
            {
                throw new MergeProcessException("Unsaved changes in an open scene.");
            }

            if (MergeActionContainers == null || MergeActionContainers.Count == 0)
            {
                throw new MergeProcessException("Merge Actions were not generated.");
            }
        }

        public void Finish()
        {
            FinishProcess();
            gameObjectDictionary.Dispose();
        }

        public void Cancel()
        {
            CancelProcess();
            gameObjectDictionary.Dispose();
        }

        public override string ToString()
        {
            return $"{GetType().Name} ({path})";
        }

        protected abstract List<GameObjectMergeActionContainer> StartProcess();

        protected abstract void FinishProcess();

        protected abstract void CancelProcess();

    }
}
