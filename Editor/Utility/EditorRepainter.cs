namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine;

    internal static class EditorRepainter
    {
        private const string inspector = "UnityEditor.InspectorWindow";

        public static void RepaintInspector()
        {
            var editorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            foreach (var editorWindow in editorWindows)
            {
                if (editorWindow.GetType().ToString() == inspector)
                {
                    editorWindow.Repaint();
                }
            }
        }
    }
}
