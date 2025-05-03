namespace ThirteenPixels.OpenUnityMergeTool
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using UnityEditor.UIElements;
    using System;
    using UnityPropertyField = UnityEditor.UIElements.PropertyField;

    internal class PropertyField : VisualElement
    {
        public PropertyField(IMergeable mergeable, Action onChange)
        {
            style.flexGrow = 1;

            var type = mergeable.SerializedProperty.propertyType;
            if (type == UnityEditor.SerializedPropertyType.Quaternion)
            {
                CreateEulerAnglesField(mergeable, onChange);
            }
            else
            {
                CreateRegularPropertyField(mergeable, onChange);
            }
        }

        private void CreateRegularPropertyField(IMergeable mergeable, Action onChange)
        {
            var property = mergeable.SerializedProperty;

            var propertyField = new UnityPropertyField(property);
            propertyField.Bind(property.serializedObject);
            propertyField.TrackPropertyValue(property, _ =>
            {
                mergeable.AcceptNewValue();
                onChange();
            });

            propertyField.style.flexGrow = 1;
            propertyField.style.marginTop = 3;
            Add(propertyField);
        }

        private void CreateEulerAnglesField(IMergeable mergeable, Action onChange)
        {
            var property = mergeable.SerializedProperty;

            var vector3Field = new Vector3Field(property.displayName);
            vector3Field.SetValueWithoutNotify(property.quaternionValue.eulerAngles);
            vector3Field.Bind(property.serializedObject);
            vector3Field.RegisterValueChangedCallback(evt =>
            {
                mergeable.SerializedProperty.SetValue(Quaternion.Euler(evt.newValue));
            });
            vector3Field.TrackPropertyValue(mergeable.SerializedProperty, _ =>
            {
                vector3Field.SetValueWithoutNotify(property.quaternionValue.eulerAngles);
                mergeable.AcceptNewValue();
                onChange();
            });

            vector3Field.style.flexGrow = 1;
            vector3Field.style.marginTop = 3;
            Add(vector3Field);
        }
    }
}
