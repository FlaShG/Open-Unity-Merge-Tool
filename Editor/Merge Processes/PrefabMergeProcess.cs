
namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using System.Collections.Generic;
    using System.Linq;

    internal class PrefabMergeProcess : MergeProcess
    {
        private PrefabStage prefabStage;

        public PrefabMergeProcess(VersionControlSystem.FilePath path) : base(path)
        {
            
        }

        protected override List<GameObjectMergeActionContainer> StartProcess()
        {
            void DisplayProgressBar(float step)
            {
                EditorUtility.DisplayProgressBar(DialogConstants.title, "Starting prefab merge...", step / 5f);
            }

            DisplayProgressBar(0);
            MergeTool.Vcs.CheckoutTheirs(path.repositoryPath);
            var theirPrefabPath = FileUtility.CopyFile(path.projectPath, theirsSuffix);
            AssetDatabase.ImportAsset(theirPrefabPath);

            DisplayProgressBar(1);
            MergeTool.Vcs.CheckoutOurs(path.repositoryPath);
            AssetDatabase.ImportAsset(path.projectPath);

            DisplayProgressBar(2);
            var ourPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path.projectPath);
            prefabStage = PrefabStageUtility.OpenPrefab(path.projectPath);
            EditorApplication.update += Update;

            DisplayProgressBar(2.5f);
            var ourObjects = FindAllObjects(ourPrefab);
            var ourInstantiatedObjects = FindAllObjects(prefabStage.prefabContentsRoot);
            gameObjectDictionary.AddOurObjects(ourInstantiatedObjects, ourObjects);

            DisplayProgressBar(3);
            var theirPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(theirPrefabPath);
            theirPrefab.name = theirPrefab.name[..^theirsSuffix.Length];

            DisplayProgressBar(3.5f);
            var theirObjects = FindAllObjects(theirPrefab);
            gameObjectDictionary.AddTheirObjects(theirObjects);

            DisplayProgressBar(4);
            // TODO The prefabs are merged at this point, which will break sibling indices.
            var mergeActions = gameObjectDictionary.GenerateMergeActions();

            DisplayProgressBar(5);
            Cleanup();

            return mergeActions;
        }

        protected override void CancelProcess()
        {
            ReturnFromStage();
            Cleanup();
        }

        protected override void FinishProcess()
        {
            PrefabUtility.SaveAsPrefabAsset(prefabStage.prefabContentsRoot, path.projectPath);
            ReturnFromStage();
            Cleanup();
            MergeTool.Vcs.MarkAsMerged(path.repositoryPath);
        }

        private void Cleanup()
        {
            AssetDatabase.DeleteAsset(FileUtility.AttachSuffix(path.projectPath, theirsSuffix));
            EditorUtility.ClearProgressBar();
        }

        private void ReturnFromStage()
        {
            EditorApplication.update -= Update;

            if (StageUtility.GetCurrentStage() == prefabStage)
            {
                StageUtility.GoBackToPreviousStage();
            }
        }

        private void Update()
        {
            if (StageUtility.GetCurrentStage() != prefabStage)
            {
                MergeTool.CancelCurrentMergeProgress();
            }
        }

        private static IEnumerable<GameObject> FindAllObjects(GameObject root)
        {
            return root.GetComponentsInChildren<Transform>().Select(transform => transform.gameObject);
        }
    }
}
