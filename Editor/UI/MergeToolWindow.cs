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

        private TabView tabView;
        private SetupTab setupTab;
        private ConflictsTab conflictsTab;
        private MergeTab mergeTab;

        private void CreateGUI()
        {
            var root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;
            root.style.SetPadding(5);

            var tabContent = new VisualElement();
            tabContent.style.flexGrow = 1;
            tabContent.style.SetPadding(6, 4, 4, 2);

            tabView = new TabView();
            tabView.Add(setupTab = new SetupTab(tabContent));
            tabView.Add(conflictsTab = new ConflictsTab(tabContent));
            tabView.Add(mergeTab = new MergeTab(tabContent));
            root.Add(tabView);

            root.Add(tabContent);

            RefreshUI();
            MergeTool.OnMergeProcessChanged += RefreshUI;
            EditorApplication.focusChanged += OnApplicationFocusChanged;
        }

        private void OnDestroy()
        {
            MergeTool.OnMergeProcessChanged -= RefreshUI;
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
            int tabIndex;
            if (MergeTool.Vcs == null)
            {
                tabIndex = 0;
                setupTab.Refresh();
            }
            else if (MergeTool.CurrentMergeProcess == null)
            {
                tabIndex = 1;
                conflictsTab.Refresh();
            }
            else
            {
                tabIndex = 2;
                mergeTab.Refresh();
            }
            tabView.selectedTabIndex = tabIndex;
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
