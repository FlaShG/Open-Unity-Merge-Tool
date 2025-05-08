namespace ThirteenPixels.OpenUnityMergeTool
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;

    internal static class StyleExtensions
    {
        public static void Show(this IStyle style)
        {
            style.display = DisplayStyle.Flex;
        }

        public static void Hide(this IStyle style)
        {
            style.display = DisplayStyle.None;
        }

        public static void SetVisible(this IStyle style, bool visible)
        {
            style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static void SetSize(this IStyle style, float width, float height)
        {
            style.width = width;
            style.height = height;
        }

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

        public static void AddButtonIcon(this Button button, Texture icon)
        {
            button.style.flexDirection = FlexDirection.Row;

            var image = new Image();
            image.image = icon;
            image.style.SetSize(16, 16);
            image.style.flexShrink = 0;
            button.Add(image);
        }

        public static void EnableBackgroundTransitions(this IStyle style)
        {
            style.transitionProperty = new List<StylePropertyName> { "background-color" };
            style.transitionDuration = new List<TimeValue> { StyleConstants.TransitionDuration };
            style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(EasingMode.EaseOut) };
        }

        public static void EnableProgressBarAnimation(this ProgressBar progressBar)
        {
            var element = progressBar.Q<VisualElement>(className: "unity-progress-bar__progress");
            element.style.transitionProperty = new List<StylePropertyName> { "right" };
            element.style.transitionDuration = new List<TimeValue> { StyleConstants.TransitionDuration };
            element.style.transitionTimingFunction = new List<EasingFunction> { new EasingFunction(EasingMode.EaseOut) };
        }
    }
}
