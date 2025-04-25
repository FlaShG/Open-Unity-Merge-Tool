namespace ThirteenPixels.OpenUnityMergeTool
{
    using System.Collections.Generic;
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

        public static void SetButtonColor(this Button button, Color color)
        {
            button.style.backgroundColor = color;
            // TODO This will create garbage over time :(
            button.RegisterCallback<MouseOverEvent>(_ =>
            {
                Color.RGBToHSV(color, out var h, out var s, out var v);
                button.style.backgroundColor = Color.HSVToRGB(h, s, Mathf.Lerp(v, 1f, 0.1f));
            });
            button.RegisterCallback<MouseOutEvent>(_ =>
            {
                button.style.backgroundColor = color;
            });
        }

        public static void ResetButtonColor(this Button button)
        {
            button.style.backgroundColor = StyleKeyword.Null;
            // TODO Reset hover color as well... would probably work best by adding and removing a uss class instead.
        }

        public static void EnableBackgroundTransitions(this IStyle style)
        {
            style.transitionProperty = new List<StylePropertyName> { "background-color" };
            style.transitionDuration = new List<TimeValue> { StyleConstants.TransitionDuration };
            style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(EasingMode.EaseOut) };
        }
    }
}
