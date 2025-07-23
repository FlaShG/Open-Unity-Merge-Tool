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
        /// The path of the file this process is merging, relative to the repository.
        /// </summary>
        protected readonly string path;
        /// <summary>
        /// The path of the file this process is merging, relative to the Unity project root.
        /// </summary>
        protected readonly string projectLocalPath;

        public List<GameObjectMergeActionContainer> MergeActionContainers { get; private set; }
        public int CompletedMergeActionContainerCount => MergeActionContainers.Where(container => container.IsCompleted).Count();

        protected MergeProcess(string path)
        {
            this.path = path;
            projectLocalPath = FileUtility.GetProjectLocal(path);
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
