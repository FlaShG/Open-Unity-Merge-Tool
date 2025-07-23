
namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEditor.SceneManagement;
    using System.Collections.Generic;

    internal class SceneMergeProcess : MergeProcess
    {
        public SceneMergeProcess(VersionControlSystem.FilePath path) : base(path)
        {

        }

        protected override List<GameObjectMergeActionContainer> StartProcess()
        {
            void DisplayProgressBar(float step)
            {
                EditorUtility.DisplayProgressBar(DialogConstants.title, "Starting scene merge...", step / 5f);
            }

            DisplayProgressBar(0);
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Lightmapping.Cancel();
            MergeTool.Vcs.CheckoutTheirs(path.repositoryPath);
            var theirScenePath = FileUtility.CopyFile(path.projectPath, theirsSuffix);
            AssetDatabase.ImportAsset(theirScenePath);

            DisplayProgressBar(1);
            MergeTool.Vcs.CheckoutOurs(path.repositoryPath);

            DisplayProgressBar(2);
            var ourScene = EditorSceneManager.OpenScene(path.projectPath, OpenSceneMode.Single);
            Lightmapping.Cancel();

            DisplayProgressBar(2.5f);
            var ourObjects = GetAllSceneObjects(ourScene);
            gameObjectDictionary.AddOurObjects(ourObjects);

            DisplayProgressBar(3);
            var theirScene = EditorSceneManager.OpenScene(FileUtility.AttachSuffix(path.projectPath, theirsSuffix), OpenSceneMode.Additive);
            var theirObjects = GetAllSceneObjects(theirScene);
            gameObjectDictionary.AddTheirObjects(theirObjects);

            DisplayProgressBar(4);
            var mergeActions = gameObjectDictionary.GenerateMergeActions();

            DisplayProgressBar(5);
            EditorSceneManager.MergeScenes(theirScene, ourScene);
            Cleanup();

            return mergeActions;
        }

        protected override void CancelProcess()
        {
            Cleanup();
            EditorSceneManager.OpenScene(path.projectPath, OpenSceneMode.Single);
        }

        protected override void FinishProcess()
        {
            Cleanup();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            MergeTool.Vcs.MarkAsMerged(path.repositoryPath);
            EditorSceneManager.OpenScene(path.projectPath, OpenSceneMode.Single);
        }

        private void Cleanup()
        {
            AssetDatabase.DeleteAsset(FileUtility.AttachSuffix(path.projectPath, theirsSuffix));
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Returns all objects in the given <paramref name="scene"/> in the hierarchy order.
        /// Guarantees that child GameObjects come after their parents.
        /// </summary>
        private static IEnumerable<GameObject> GetAllSceneObjects(Scene scene)
        {
            var result = new List<GameObject>();
            scene.GetRootGameObjects(result);

            for (var index = 0; index < result.Count; index++)
            {
                AddAllChildren(result, ref index);
            }

            return result;
        }

        private static void AddAllChildren(List<GameObject> result, ref int index)
        {
            var root = result[index];
            foreach (Transform child in root.transform)
            {
                index++;
                result.Insert(index, child.gameObject);
                AddAllChildren(result, ref index);
            }
        }
    }
}
