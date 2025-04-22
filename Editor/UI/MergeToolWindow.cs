namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using UnityEditor;

    internal class MergeToolWindow : EditorWindow
    {
        [MenuItem("Tools/Open Unity Merge Tool")]
        private static void OpenWindow()
        {
            var window = GetWindow<MergeToolWindow>();
            // TODO Add icon
            window.titleContent = new GUIContent("Merge Tool");
        }

        private TabView tabView;

        private void CreateGUI()
        {
            var root = new Margin(10);
            rootVisualElement.Add(root);

            tabView = new TabView();
            tabView.Add(new SetupTab());
            tabView.Add(new ConflictsTab());
            tabView.Add(new MergeTab());
            root.Add(tabView);

            RefreshUI();
            MergeTool.OnMergeProcessChanged += RefreshUI;
        }

        private void RefreshUI()
        {
            foreach (MergeToolTab tab in tabView.Children())
            {
                tab.Refresh();
            }

            int tabIndex;
            if (MergeTool.Vcs == null)
            {
                tabIndex = 0;
            }
            else if (MergeTool.CurrentMergeProcess == null)
            {
                tabIndex = 1;
            }
            else
            {
                tabIndex = 2;
            }
            tabView.selectedTabIndex = tabIndex;
        }
    }
}
