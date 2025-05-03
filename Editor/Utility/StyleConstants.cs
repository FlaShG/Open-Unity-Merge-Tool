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

        public static readonly Color BackgroundLineColor = new Color(0f, 0f, 0f, 0.3f);

        public static readonly Color UnmergedColor = new Color(0.4f, 0.25f, 0.25f);
        public static readonly Color AutomergedColor = new Color(0.25f, 0.35f, 0.35f);
        public static readonly Color MergedColor = new Color(0.25f, 0.4f, 0.25f);
        // (0.3f, 0.7f, 1f) would be much nicer, but that collides with the default :active color
        public static readonly Color PrefabColor = new Color(0f, 0f, 0.4f);

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
    }
}
