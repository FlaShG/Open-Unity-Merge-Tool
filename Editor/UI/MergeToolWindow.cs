namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using UnityEditor;

    internal class MergeToolWindow : EditorWindow, IHasCustomMenu
    {
        [MenuItem("Tools/Open Unity Merge Tool")]
        private static void OpenWindow()
        {
            var window = GetWindow<MergeToolWindow>();
            // TODO Add icon
            window.titleContent = new GUIContent("Merge Tool");
            window.minSize = new Vector2(600, 200);
        }

        private VisualElement tabContent;
        private SetupTab setupTab;
        private ConflictsTab conflictsTab;
        private MergeTab mergeTab;

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;
            root.style.SetPadding(5);

            tabContent = new VisualElement();
            tabContent.name = "Tab Content";
            tabContent.style.flexGrow = 1;
            tabContent.style.SetPadding(6, 4, 4, 2);
            root.Add(tabContent);

            setupTab = new SetupTab();
            conflictsTab = new ConflictsTab();
            mergeTab = new MergeTab();

            RefreshUI();
            MergeTool.OnStateChanged += RefreshUI;
            EditorApplication.focusChanged += OnApplicationFocusChanged;
        }

        private void OnDestroy()
        {
            MergeTool.OnStateChanged -= RefreshUI;
            EditorApplication.focusChanged -= OnApplicationFocusChanged;
        }

        private void OnApplicationFocusChanged(bool focus)
        {
            if (focus)
            {
                RefreshUI();
            }
        }

        private void RefreshUI()
        {
            if (MergeTool.Vcs == null)
            {
                SelectTab(setupTab);
            }
            else if (MergeTool.CurrentMergeProcess == null)
            {
                mergeTab.UpdateContent();
                SelectTab(conflictsTab);
            }
            else
            {
                SelectTab(mergeTab);
            }
        }

        private void SelectTab(MergeToolTab tab)
        {
            tab.UpdateContent();
            tabContent.Clear();
            tabContent.Add(tab);
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Debug Status"), false, ShowStatus);
        }

        private void ShowStatus()
        {
            EditorUtility.DisplayDialog("Merge Tool Debug Status",
                $"Merge Process: {MergeTool.CurrentMergeProcess}",
                "OK");
        }
    }
}
