namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;
    using UnityEditor;

    internal abstract class MergeToolTab : Tab
    {
        private readonly VisualElement root;

        public MergeToolTab()
        {
            root = new Margin(10, 5, 5);
            base.Add(root);
            BuildUI();
        }

        protected abstract void BuildUI();

        public new void Add(VisualElement element)
        {
            root.Add(element);
        }

        public new void Remove(VisualElement element)
        {
            root.Remove(element);
        }

        public new void Clear()
        {
            root.Clear();
        }

        public void Refresh()
        {
            Clear();
            BuildUI();
        }

        /// <summary>
        /// Enables automatic <see cref="Refresh"/> invocation when the tab is selected or the editor is focused while it is open.
        /// </summary>
        protected void EnableAutoRefresh()
        {
            selected += _ => Refresh();
            EditorApplication.focusChanged += focus =>
            {
                if (focus && visible)
                {
                    Refresh();
                }
            };
        }
    }
}
