namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using System;
    using UnityEditor.SceneManagement;
    using UnityEngine.SceneManagement;

    internal static class MergeTool
    {
        private const string vcsKey = "omt/vcs";

        private static VersionControlSystem vcs;
        public static VersionControlSystem Vcs
        {
            get => vcs;
            set 
            {
                vcs = value;
                EditorPrefs.SetString(vcsKey, value.GetType().Name);
            }
        }
        public static VersionControlSystem.Status VcsStatus => Vcs?.GetStatus() ?? VersionControlSystem.Status.VCSNotFound;

        public static MergeProcess CurrentMergeProcess { get; private set; }

        public static event Action OnMergeProcessChanged;
        public static event Action OnMergeStateChanged;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            LoadVCSFromPrefs();
        }

        public static void StartMergeProcess(string path)
        {
            CancelCurrentMergeProgress();

            if (FileUtility.IsScene(path))
            {
                CurrentMergeProcess = new SceneMergeProcess(path);
            }
            else if (FileUtility.IsPrefab(path))
            {
                CurrentMergeProcess = new PrefabMergeProcess(path);
            }

            try
            {
                CurrentMergeProcess.Start();
                OnMergeProcessChanged?.Invoke();
                EditorSceneManager.sceneUnloaded += OnSceneUnloaded;
            }
            catch
            {
                CancelCurrentMergeProgress();
                throw;
            }
        }

        public static void CancelCurrentMergeProgress()
        {
            EditorSceneManager.sceneUnloaded -= OnSceneUnloaded;
            if (CurrentMergeProcess != null)
            {
                CurrentMergeProcess.Cancel();
                CurrentMergeProcess = null;
                OnMergeProcessChanged?.Invoke();
            }
        }

        public static void FinishCurrentMergeProgress()
        {
            EditorSceneManager.sceneUnloaded -= OnSceneUnloaded;
            if (CurrentMergeProcess != null)
            {
                CurrentMergeProcess.Finish();
                CurrentMergeProcess = null;
                OnMergeProcessChanged?.Invoke();
            }
        }

        public static void UpdateAfterMergeStateChange()
        {
            OnMergeStateChanged?.Invoke();
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            CancelCurrentMergeProgress();
        }

        private static void LoadVCSFromPrefs()
        {
            var vcsName = EditorPrefs.GetString(vcsKey, string.Empty);
            if (!string.IsNullOrEmpty(vcsName))
            {
                var availableSystems = TypeCache.GetTypesDerivedFrom<VersionControlSystem>();
                foreach (var vcsType in availableSystems)
                {
                    if (vcsType.Name == vcsName)
                    {
                        vcs = (VersionControlSystem)Activator.CreateInstance(vcsType);
                        break;
                    }
                }
            }
        }
    }
}
