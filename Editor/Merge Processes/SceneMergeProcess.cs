
namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEditor.SceneManagement;
    using System.Collections.Generic;
    using System.Linq;

    internal class SceneMergeProcess : MergeProcess
    {
        public SceneMergeProcess(string path) : base(path)
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
            MergeTool.Vcs.CheckoutTheirs(path);
            var theirScenePath = FileUtility.CopyFile(path, theirsSuffix);
            AssetDatabase.ImportAsset(theirScenePath);

            DisplayProgressBar(1);
            MergeTool.Vcs.CheckoutOurs(path);

            DisplayProgressBar(2);
            var ourScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            Lightmapping.Cancel();

            DisplayProgressBar(2.5f);
            var ourObjects = GetAllSceneObjects(ourScene);
            gameObjectDictionary.AddOurObjects(ourObjects);

            DisplayProgressBar(3);
            var theirScene = EditorSceneManager.OpenScene(FileUtility.AttachSuffix(path, theirsSuffix), OpenSceneMode.Additive);
            var theirObjects = GetAllSceneObjects(theirScene);
            gameObjectDictionary.AddTheirObjects(theirObjects);
            EditorSceneManager.MergeScenes(theirScene, ourScene);

            DisplayProgressBar(4);
            var mergeActions = gameObjectDictionary.GenerateMergeActions();

            DisplayProgressBar(5);
            Cleanup();

            return mergeActions;
        }

        public override void Cancel()
        {
            Cleanup();
        }

        public override void Finish()
        {
            Cleanup();
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            MergeTool.Vcs.MarkAsMerged(path);
        }

        private void Cleanup()
        {
            AssetDatabase.DeleteAsset(FileUtility.AttachSuffix(path, theirsSuffix));
            EditorUtility.ClearProgressBar();
        }

        private static HashSet<GameObject> GetAllSceneObjects(Scene scene)
        {
            var objects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            return new HashSet<GameObject>(objects.Where(gameObject => gameObject.scene == scene));
        }
    }
}
