namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;

    internal class HorizontalLayout : VisualElement
    {
        public HorizontalLayout()
        {
            style.flexDirection = FlexDirection.Row;
            style.width = Length.Percent(100);
            style.justifyContent = Justify.Center;
        }
    }
}
