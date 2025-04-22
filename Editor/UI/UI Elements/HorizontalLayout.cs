namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine.UIElements;

    internal class HorizontalLayout : VisualElement
    {
        public HorizontalLayout()
        {
            style.flexDirection = FlexDirection.Row;
            style.flexGrow = 1;
            style.width = Length.Percent(100);
            style.alignContent = Align.Stretch;
            style.alignItems = Align.Stretch;
            style.justifyContent = Justify.Center;
        }
    }
}
