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
            tabContent.name = "Tab Content";
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
            MergeTool.OnMergeStateChanged += RefreshUI;
            EditorApplication.focusChanged += OnApplicationFocusChanged;
        }

        private void OnDestroy()
        {
            MergeTool.OnMergeProcessChanged -= RefreshUI;
            MergeTool.OnMergeStateChanged -= RefreshUI;
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
            setupTab.UpdateContent();
            mergeTab.UpdateContent();

            if (MergeTool.Vcs == null)
            {
                tabView.selectedTabIndex = 0;
                setupTab.Select();
            }
            else if (MergeTool.CurrentMergeProcess == null)
            {
                tabView.selectedTabIndex = 1;
                conflictsTab.Select();
            }
            else
            {
                tabView.selectedTabIndex = 2;
                mergeTab.Select();
            }
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
