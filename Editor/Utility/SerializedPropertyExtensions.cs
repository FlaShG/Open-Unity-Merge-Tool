namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;

    internal static class SerializedPropertyExtensions
    {
        public static object GetValue(this SerializedProperty property)
        {
            try
            {
                return property.boxedValue;
            }
            catch
            {
                return null;
            }
        }

        public static void SetValue(this SerializedProperty property, object value)
        {
            property.boxedValue = value;
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public static bool IsPrefabDefault(this SerializedProperty property)
        {
            return property.isInstantiatedPrefab && !property.prefabOverride;
        }

        public static bool IsPrefabOverride(this SerializedProperty property)
        {
            return property.isInstantiatedPrefab && property.prefabOverride;
        }

        public static void SetPrefabOverride(this SerializedProperty property, bool isOverride)
        {
            if (isOverride)
            {
                try
                {
                    SerializedPropertyPrefabOverrideEnabler.EnablePrefabOverride(property);
                }
                catch
                {
                    EditorUtility.DisplayDialog(DialogConstants.title, "We've run into an extremely rare edge case.\n" +
                        "The property you're trying to set to a prefab override value (that equals the prefab's default value) has an unsupported type.\n" +
                        "Please play around with the property field to give property an override value.", "OK");
                }
            }
            else
            {
                property.prefabOverride = false;
                // TODO This would work equally well?
                // PrefabUtility.RevertPropertyOverride(SerializedProperty, InteractionMode.AutomatedAction);
            }
        }

        /// <summary>
        /// Removes any hungarian prefixes (like <c>"m_"</c>) from name of the given <paramref name="property"/>.
        /// </summary>
        public static string GetPlainName(this SerializedProperty property)
        {
            var name = property.name;
            var underscoreIndex = name.IndexOf('_');
            if (underscoreIndex >= 0)
            {
                name = name.Substring(underscoreIndex + 1);
            }
            return name;
        }

        public static bool IsRealArray(this SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.Generic && property.isArray;
        }
    }
}