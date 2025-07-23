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

            setupTab = new SetupTab(OpenConflictsTab);
            conflictsTab = new ConflictsTab();
            mergeTab = new MergeTab();

            RefreshUI();
            MergeTool.StateChanged += RefreshUI;
            EditorApplication.focusChanged += OnApplicationFocusChanged;
        }

        private void OnDestroy()
        {
            MergeTool.StateChanged -= RefreshUI;
            EditorApplication.focusChanged -= OnApplicationFocusChanged;
        }

        private void OnApplicationFocusChanged(bool focus)
        {
            if (focus && !IsSelected(mergeTab))
            {
                RefreshUI();
            }
        }

        private void RefreshUI()
        {
            if (IsSelected(setupTab) || MergeTool.VcsStatus != VersionControlSystem.Status.Okay)
            {
                SelectTab(setupTab);
            }
            else if (MergeTool.CurrentMergeProcess == null)
            {
                OpenConflictsTab();
            }
            else
            {
                SelectTab(mergeTab);
            }
        }

        private void SelectTab(MergeToolTab tab)
        {
            tab.UpdateContent();
            if (!tabContent.Contains(tab))
            {
                tabContent.Clear();
                tabContent.Add(tab);
            }
        }

        private bool IsSelected(MergeToolTab tab)
        {
            return tabContent.Contains(tab);
        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Open Settings"), false, OpenSettings);
        }

        private void OpenSettings()
        {
            SelectTab(setupTab);
        }

        private void OpenConflictsTab()
        {
            mergeTab?.UpdateContent();
            SelectTab(conflictsTab);
        }
    }
}
