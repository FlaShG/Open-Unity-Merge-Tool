namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    internal static class StyleConstants
    {
        public static class Icons
        {
            public static Texture GameObject => GetEditorIcon("d_GameObject Icon", "GameObject Icon");

            private static Texture GetEditorIcon(string id)
            {
                return EditorGUIUtility.IconContent(id).image;
            }

            private static Texture GetEditorIcon(string darkId, string lightId)
            {
                var id = EditorGUIUtility.isProSkin ? darkId : lightId;
                return EditorGUIUtility.IconContent(id).image;
            }
        }

        public static class Colors
        {
            public static readonly Color BackgroundLine = GetThemedColor(new(0f, 0f, 0f, 0.3f), new(1f, 1f, 1f, 0.3f));

            public static readonly Color Unmerged = GetThemedColor(new(0.4f, 0.25f, 0.25f), new(0.75f, 0.6f, 0.6f));
            public static readonly Color Automerged = GetThemedColor(new(0.35f, 0.35f, 0.25f), new(0.75f, 0.75f, 0.65f));
            public static readonly Color Merged = GetThemedColor(new(0.25f, 0.4f, 0.25f), new(0.6f, 0.75f, 0.6f));

            // (0.3f, 0.7f, 1f) would be much nicer, but that collides with the default :active color
            public static readonly Color PrefabConnection = GetThemedColor(new(0f, 0f, 0.4f), new(0.6f, 0.6f, 1f));
            public static readonly Color Highlight = GetThemedColor(new(1f, 0.6f, 0f), new(1f, 0.4f, 0f));

            public static readonly Color LightText = GetThemedColor(new(0.5f, 0.5f, 0.5f), new(0.3f, 0.3f, 0.3f));
            public static readonly Color CardBorder = GetThemedColor(new(0.2f, 0f, 0f), new(0.55f, 0.45f, 0.45f));

            private static Color GetThemedColor(Color darkTheme, Color lightTheme)
            {
                return EditorGUIUtility.isProSkin ? darkTheme : lightTheme;
            }
        }

        public static readonly TimeValue TransitionDuration = new TimeValue(0.15f, TimeUnit.Second);

        public static Color GetColorFor(DecisionState decision)
        {
            switch (decision)
            {
                case DecisionState.Complete:
                    return Colors.Merged;
                case DecisionState.AutoCompleted:
                    return Colors.Automerged;
                default:
                    return Colors.Unmerged;
            }
        }
    }
}
