namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEditor;
    using UnityEngine;
    using System;

    // You don't want to know.
    // Turn back while you can, traveller.
    // ...
    // This is the dumbest thing for the tiniest edge case.
    // So basically, when you have a merge conflict on a property, and the conflict is between
    // two equal value, but one of them is a prefab override and the other isn't,
    // the button that sets the value as an override will not work.
    // Unity expects us to set a value different from the prefab's value, which will mark the property as a prefabOverride.
    // However, we cannot do that unless we go nuts and pick some random value to set - this is what this class does.
    internal static class SerializedPropertyPrefabOverrideEnabler
    {
        /// <summary>
        /// Sets the <paramref name="property"/>'s <c>prefabOverride</c> property to <c>true</c>.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown if the property's type is too exotic.</exception>
        public static void EnablePrefabOverride(SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.ArraySize:
                    if (property.numericType == SerializedPropertyNumericType.UInt64)
                    {
                        property.ulongValue = PickDifferentValue(property.ulongValue, 0u, 1u);
                    }
                    else
                    {
                        property.longValue = PickDifferentValue(property.longValue, 0L, 1L);
                    }

                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = PickDifferentValue(property.boolValue, false, true);
                    break;
                case SerializedPropertyType.Float:
                    if (property.numericType == SerializedPropertyNumericType.Double)
                    {
                        property.doubleValue = PickDifferentValue(property.doubleValue, 0d, 1d);
                    }
                    else
                    {
                        property.floatValue = PickDifferentValue(property.floatValue, 0f, 1f);
                    }
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = PickDifferentValue(property.stringValue, string.Empty, "x");
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = PickDifferentValue(property.colorValue, Color.black, Color.white);
                    break;
                case SerializedPropertyType.ObjectReference:
                    var obj = new UnityEngine.Object();
                    property.objectReferenceValue = PickDifferentValue(property.objectReferenceValue, null, obj);
                    UnityEngine.Object.DestroyImmediate(obj);
                    break;
                case SerializedPropertyType.LayerMask:
                    property.intValue = PickDifferentValue(property.intValue, 0, 1);
                    break;
                case SerializedPropertyType.RenderingLayerMask:
                    property.uintValue = PickDifferentValue(property.uintValue, 0u, 1u);
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = PickDifferentValue(property.vector2Value, Vector2.zero, Vector2.one);
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = PickDifferentValue(property.vector3Value, Vector3.zero, Vector3.one);
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = PickDifferentValue(property.vector4Value, Vector4.zero, Vector4.one);
                    break;
                case SerializedPropertyType.Rect:
                    property.rectValue = PickDifferentValue(property.rectValue,
                        new Rect(0, 0, 0, 0),
                        new Rect(0, 0, 1, 1));
                    break;
                case SerializedPropertyType.Character:
                    property.uintValue = PickDifferentValue(property.uintValue, 0u, 1u);
                    break;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = PickDifferentValue(property.animationCurveValue, null, new AnimationCurve());
                    break;
                case SerializedPropertyType.Bounds:
                    property.boundsValue = PickDifferentValue(property.boundsValue,
                        new Bounds(Vector3.one, Vector3.one),
                        new Bounds(Vector3.zero, Vector3.one));
                    break;
                case SerializedPropertyType.Gradient:
                    var a = new Gradient { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.black, 0) } };
                    var b = new Gradient { colorKeys = new GradientColorKey[] { new GradientColorKey(Color.white, 0) } };
                    property.gradientValue = PickDifferentValue(property.gradientValue, a, b);
                    break;
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = PickDifferentValue(property.quaternionValue, Quaternion.identity, Quaternion.Euler(90, 0, 0));
                    break;
                case SerializedPropertyType.ExposedReference:
                    var eobj = new UnityEngine.Object();
                    property.exposedReferenceValue = PickDifferentValue(property.exposedReferenceValue, null, eobj);
                    UnityEngine.Object.DestroyImmediate(eobj);
                    break;
                case SerializedPropertyType.Vector2Int:
                    property.vector2IntValue = PickDifferentValue(property.vector2IntValue, Vector2Int.zero, Vector2Int.one);
                    break;
                case SerializedPropertyType.Vector3Int:
                    property.vector3IntValue = PickDifferentValue(property.vector3IntValue, Vector3Int.zero, Vector3Int.one);
                    break;
                case SerializedPropertyType.RectInt:
                    property.rectIntValue = PickDifferentValue(property.rectIntValue,
                        new RectInt(Vector2Int.zero, Vector2Int.zero),
                        new RectInt(Vector2Int.zero, Vector2Int.one));
                    break;
                case SerializedPropertyType.BoundsInt:
                    property.boundsIntValue = PickDifferentValue(property.boundsIntValue,
                        new BoundsInt(Vector3Int.zero, Vector3Int.zero),
                        new BoundsInt(Vector3Int.zero, Vector3Int.one));
                    break;
                case SerializedPropertyType.ManagedReference:
                    property.managedReferenceValue = PickDifferentValue(property.managedReferenceValue, null, new object());
                    break;
                case SerializedPropertyType.Hash128:
                    property.hash128Value = PickDifferentValue(property.hash128Value, new Hash128(0, 0), new Hash128(1, 2));
                    break;
                default:
                    throw new NotSupportedException();
            }

            property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static T PickDifferentValue<T>(T current, T a, T b)
        {
            if (Equals(current, a))
            {
                return b;
            }
            return a;
        }
    }
}
