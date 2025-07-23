namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;

    internal static class SerializedPropertyExtensions
    {
        public static object GetValue(this SerializedProperty property)
        {
            if (property.IsRealArray())
            {
                property = property.Copy();
                property.Next(true);
                var array = new object[property.arraySize];
                property.Next(true);
                for (var i = 0; i < array.Length; i++)
                {
                    property.Next(false);
                    array[i] = property.GetValue();
                }
                property.Dispose();
                return array;
            }

            try
            {
                return property.boxedValue;
            }
            catch
            {
                return null;
            }
        }

        public static bool HasValue(this SerializedProperty property, object value)
        {
            if (property.IsRealArray() && value is object[] array)
            {
                // TODO Compare array size, then iterate over all elements and call HasValue on each.
                return false;
            }

            try
            {
                return Equals(property.boxedValue, value);
            }
            catch
            {
                return false;
            }
        }

        public static void SetValue(this SerializedProperty property, object value)
        {
            if (property.IsRealArray() && value is object[] array)
            {
                property = property.Copy();
                property.Next(true);
                property.arraySize = array.Length;
                property.Next(true);
                for (var i = 0; i < array.Length; i++)
                {
                    property.Next(false);
                    property.SetValue(array[i]);
                }
                property.Dispose();
            }
            else
            {
                property.boxedValue = value;
            }
            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        public static bool HasSameValueAs(this SerializedProperty ourProperty, SerializedProperty theirProperty)
        {
            if (ourProperty.IsPrefabDefault() != theirProperty.IsPrefabDefault())
            {
                return false;
            }

            if (ourProperty.propertyType == SerializedPropertyType.ObjectReference)
            {
                var our = ourProperty.objectReferenceValue;
                var their = theirProperty.objectReferenceValue;

                if (our != null && their != null)
                {
                    return Equals(ObjectId.GetFor(our), ObjectId.GetFor(their));
                }
                return our == null && their == null;
            }

            if (ourProperty.propertyType == SerializedPropertyType.Generic)
            {
                using var ourProp = ourProperty.Copy();
                using var theirProp = theirProperty.Copy();
                if (ourProp.hasChildren)
                {
                    var returnDepth = ourProperty.depth;
                    ourProp.Next(true);
                    theirProp.Next(true);
                    do
                    {
                        if (!HasSameValueAs(ourProp, theirProp))
                        {
                            return false;
                        }
                    }
                    while (ourProp.Next(false) && theirProp.Next(false) && ourProp.depth > returnDepth);
                }

                return true;
            }

            return SerializedProperty.DataEquals(ourProperty, theirProperty);
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
