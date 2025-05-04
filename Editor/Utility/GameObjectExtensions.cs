namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine;
    using System.Text;
    using System.Reflection;

    internal static class GameObjectExtensions
    {
        /// <summary>
        /// Activates/deactivates the GameObjct, and hides it when it is disabled.
        /// This is used for "their" objects to hide them while merging.
        /// </summary>
        /// <param name="gameObject">The object do activate/deactivate.</param>
        /// <param name="active">Activate or deactivate the object?</param>
        /// <param name="realActive">Set to <c>false</c> while activating to activate it with <c>activeSelf</c> set to false.</param>
        public static void SetActiveForMerging(this GameObject gameObject, bool active, bool realActive = true)
        {
            gameObject.SetActive(active && realActive);
            gameObject.hideFlags = active ? HideFlags.None : HideFlags.HideAndDontSave;
        }

        /// <summary>
        /// Adds the copy of a Component to a GameObject.
        /// </summary>
        /// <param name="gameObject">The GameObject that will get the new Component</param>
        /// <param name="original">The original component to copy</param>
        /// <returns>The reference to the newly added Component copy</returns>
        public static Component AddComponent(this GameObject gameObject, Component original)
        {
            var newComponent = gameObject.AddComponent(original.GetType());
            EditorUtility.CopySerialized(original, newComponent);

            return newComponent;
        }

        /// <summary>
        /// Ping the GameObject in the hierarchy, select it, and center it in the scene view.
        /// </summary>
        /// <param name="gameObject">The GameObject of interest</param>
        public static void Highlight(this GameObject gameObject)
        {
            Selection.activeGameObject = gameObject;
            EditorGUIUtility.PingObject(gameObject);

            var view = SceneView.lastActiveSceneView;
            if (view)
            {
                view.in2DMode = gameObject.GetComponent<RectTransform>() || EditorIsIn2dMode();
                EditorApplication.delayCall += () => view.FrameSelected();
            }
        }

        private static bool EditorIsIn2dMode()
        {
            var editorSettingsType = typeof(EditorSettings);
            var property = editorSettingsType.GetProperty("defaultBehaviorMode", BindingFlags.Static | BindingFlags.NonPublic);
            if (property != null)
            {
                var mode = property.GetValue(null, null);
                return mode.ToString() == "Mode2D";
            }
            return false;
        }

        /// <summary>
        /// Gets the path of this GameObject in the hierarchy.
        /// </summary>
        public static string GetPath(this GameObject gameObject)
        {
            var transform = gameObject.transform;
            var sb = new StringBuilder(transform.name);
            while (transform.parent != null)
            {
                transform = transform.parent;
                sb.Insert(0, transform.name + "/");
            }
            return sb.ToString();
        }
    }
}
