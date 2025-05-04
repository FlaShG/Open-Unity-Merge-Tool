namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    internal static class StyleConstants
    {
        public static class Icons
        {
            public static Texture GameObject => GetEditorIcon("GameObject Icon");
        }

        public static readonly Color BackgroundLineColor = GetThemedColor(new(0f, 0f, 0f, 0.3f), new(1f, 1f, 1f, 0.3f));

        public static readonly Color UnmergedColor = GetThemedColor(new(0.4f, 0.25f, 0.25f), new(0.75f, 0.6f, 0.6f));
        public static readonly Color AutomergedColor = GetThemedColor(new(0.25f, 0.35f, 0.35f), new(0.65f, 0.75f, 0.75f));
        public static readonly Color MergedColor = GetThemedColor(new(0.25f, 0.4f, 0.25f), new(0.6f, 0.75f, 0.6f));
        // (0.3f, 0.7f, 1f) would be much nicer, but that collides with the default :active color
        public static readonly Color PrefabColor = GetThemedColor(new(0f, 0f, 0.4f), new(0.6f, 0.6f, 1f));
        public static readonly Color HighlightColor = GetThemedColor(new(1f, 0.6f, 0f), new(1f, 0.4f, 0f));
        public static readonly Color LightTextColor = GetThemedColor(new(0.5f, 0.5f, 0.5f), new(0.3f, 0.3f, 0.3f));
        public static readonly Color CardBorderColor = GetThemedColor(new(0.2f, 0f, 0f), new(0.55f, 0.45f, 0.45f));

        public static readonly TimeValue TransitionDuration = new TimeValue(0.15f, TimeUnit.Second);

        public static Color GetColorFor(DecisionState resolution)
        {
            switch (resolution)
            {
                case DecisionState.Complete:
                    return MergedColor;
                case DecisionState.AutoCompleted:
                    return AutomergedColor;
                default:
                    return UnmergedColor;
            }
        }

        private static Texture GetEditorIcon(string id)
        {
            return EditorGUIUtility.IconContent(id).image;
        }

        private static Color GetThemedColor(Color darkTheme, Color lightTheme)
        {
            return EditorGUIUtility.isProSkin ? darkTheme : lightTheme;
        }
    }
}
