namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityObject = UnityEngine.Object;

    internal static class UnityObjectExtensions
    {
        public static bool GetEnabled(this UnityObject obj)
        {
            if (obj is GameObject gameObject)
            {
                return gameObject.activeSelf;
            }
            else if (obj is Behaviour behaviour)
            {
                return behaviour.enabled;
            }
            else if (obj is Collider collider)
            {
                return collider.enabled;
            }
            else if (obj is Collider2D collider2D)
            {
                return collider2D.enabled;
            }

            return true;
        }

        public static void SetEnabled(this UnityObject obj, bool enabled)
        {
            if (obj is GameObject gameObject)
            {
                gameObject.SetActive(enabled);
            }
            else if (obj is Behaviour behaviour)
            {
                behaviour.enabled = enabled;
            }
            else if (obj is Collider collider)
            {
                collider.enabled = enabled;
            }
            else if (obj is Collider2D collider2D)
            {
                collider2D.enabled = enabled;
            }
        }
    }
}
