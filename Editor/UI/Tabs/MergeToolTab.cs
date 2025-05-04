namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;

    internal abstract class MergeToolTab : VisualElement
    {

        public MergeToolTab()
        {
            name = "Tab Root";
            style.flexGrow = 1;

            CreateGUI();
            UpdateContent();
        }

        public abstract void UpdateContent();

        protected abstract void CreateGUI();
    }
}
