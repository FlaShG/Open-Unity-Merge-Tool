namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEngine.UIElements;

    internal static class StyleExtensions
    {
        public static void SetPadding(this IStyle style, float padding)
        {
            style.SetPadding(padding, padding, padding, padding);
        }

        public static void SetPadding(this IStyle style, float paddingTop, float paddingBottom, float paddingLeft, float paddingRight)
        {
            style.paddingTop = paddingTop;
            style.paddingBottom = paddingBottom;
            style.paddingLeft = paddingLeft;
            style.paddingRight = paddingRight;
        }

        public static void SetMargin(this IStyle style, float margin)
        {
            style.SetMargin(margin, margin, margin, margin);
        }

        public static void SetMargin(this IStyle style, float marginTop, float marginBottom, float marginLeft, float marginRight)
        {
            style.marginTop = marginTop;
            style.marginBottom = marginBottom;
            style.marginLeft = marginLeft;
            style.marginRight = marginRight;
        }

        public static void SetBorder(this IStyle style, float width, Color color)
        {
            style.borderTopColor = color;
            style.borderBottomColor = color;
            style.borderLeftColor = color;
            style.borderRightColor = color;

            style.borderTopWidth = width;
            style.borderBottomWidth = width;
            style.borderLeftWidth = width;
            style.borderRightWidth = width;
        }
    }
}
