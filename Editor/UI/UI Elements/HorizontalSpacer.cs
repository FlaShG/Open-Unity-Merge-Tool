namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEngine.UIElements;

    internal class HorizontalSpacer : Label
    {
        public HorizontalSpacer() : this(null)
        {
            
        }

        public HorizontalSpacer(string text) : base(text)
        {
            style.flexDirection = FlexDirection.Row;
            style.flexGrow = 1;
            style.alignContent = Align.Center;
            style.unityTextAlign = TextAnchor.MiddleCenter;
        }
    }
}
