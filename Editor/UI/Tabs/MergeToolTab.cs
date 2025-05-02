namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;

    internal abstract class MergeToolTab : Tab
    {
        protected readonly VisualElement root;
        private readonly VisualElement contentParent;

        public MergeToolTab(VisualElement contentParent)
        {
            this.contentParent = contentParent;
            root = new VisualElement();
            root.name = "Tab Root";
            root.style.flexGrow = 1;

            selected += _ => Select();

            CreateGUI();
            UpdateContent();
        }

        public void Select()
        {
            contentParent.Clear();
            contentParent.Add(root);
            UpdateContent();
        }

        public abstract void UpdateContent();

        protected abstract void CreateGUI();
    }
}
