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
                if (vcs != null)
                {
                    EditorPrefs.SetString(vcsKey, value.GetType().Name);
                }
                else
                {
                    EditorPrefs.DeleteKey(vcsKey);
                }
            }
        }
        public static VersionControlSystem.Status VcsStatus => Vcs?.GetStatus() ?? VersionControlSystem.Status.VCSNotFound;

        public static MergeProcess CurrentMergeProcess { get; private set; }

        public static event Action OnStateChanged;

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
                TriggerStateChangeEvent();
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
                TriggerStateChangeEvent();
            }
        }

        public static void FinishCurrentMergeProgress()
        {
            EditorSceneManager.sceneUnloaded -= OnSceneUnloaded;
            if (CurrentMergeProcess != null)
            {
                CurrentMergeProcess.Finish();
                CurrentMergeProcess = null;
                TriggerStateChangeEvent();
            }
        }

        public static void UseOurs(string path)
        {
            EditorUtility.DisplayProgressBar(DialogConstants.title, $"Using our version of\n{path}...", 0f);
            vcs.CheckoutOurs(path);
            EditorUtility.DisplayProgressBar(DialogConstants.title, $"Using our version of\n{path}...", 0.5f);
            vcs.MarkAsMerged(path);
            OnStateChanged?.Invoke();
            EditorUtility.ClearProgressBar();
        }

        public static void UseTheirs(string path)
        {
            EditorUtility.DisplayProgressBar(DialogConstants.title, $"Using their version of\n{path}...", 0f);
            vcs.CheckoutTheirs(path);
            EditorUtility.DisplayProgressBar(DialogConstants.title, $"Using their version of\n{path}...", 0.5f);
            vcs.MarkAsMerged(path);
            OnStateChanged?.Invoke();
            EditorUtility.ClearProgressBar();
        }

        public static void TriggerStateChangeEvent()
        {
            OnStateChanged?.Invoke();
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
