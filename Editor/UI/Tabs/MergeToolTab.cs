namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;

    internal abstract class MergeToolTab : Tab
    {
        protected VisualElement root;

        public MergeToolTab(VisualElement content)
        {
            root = content;
            closing += OnClose;
            selected += OnSelect;
        }

        protected abstract void BuildUI();

        public void Refresh()
        {
            //Clear();
            root.Clear();
            BuildUI();
        }

        private void OnSelect(Tab tab)
        {
            Refresh();
        }

        private bool OnClose()
        {
            root.Clear();
            return true;
        }
    }
}
