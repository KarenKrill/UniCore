using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KarenKrill.UniCore.Utilities
{
    [CustomPropertyDrawer(typeof(SerializeInterfaceAttribute))]
    public class SerializeInterfaceDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);
            var buttonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y,
                position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            if (EditorGUI.DropdownButton(buttonRect, new GUIContent(GetTypeName(property)), FocusType.Keyboard))
            {
                ShowTypeMenu(property);
            }
            EditorGUI.PropertyField(position, property, GUIContent.none, true);
            EditorGUI.EndProperty();
        }

        private IEnumerable<Type> _implTypes = null;

        private void ShowTypeMenu(SerializedProperty property)
        {
            _implTypes ??= GetPropertyImplTypes();
            var menu = new GenericMenu();
            foreach (var type in _implTypes)
            {
                menu.AddItem(new GUIContent(type.Name), on: false, () => CreatePropertyInstance(property, type));
            }
            menu.ShowAsContext();
        }

        private static void CreatePropertyInstance(SerializedProperty property, Type type)
        {
            property.managedReferenceValue = Activator.CreateInstance(type);
            property.serializedObject.ApplyModifiedProperties();
        }

        private Type GetPropertyInterfaceType(Type fieldType)
        {
            Type propertyType = fieldType;
            if (propertyType.IsArray)
            {
                propertyType = propertyType.GetElementType();
            }
            else if (propertyType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(propertyType))
            {
                propertyType = propertyType.GetGenericArguments()[0];
            }
            return propertyType;
        }

        private IEnumerable<Type> GetPropertyImplTypes()
        {
            var propertyType = GetPropertyInterfaceType(fieldInfo.FieldType);
            return ReflectionUtilities.GetInheritorTypes(propertyType);
        }

        private string GetTypeName(SerializedProperty property) => property.managedReferenceValue?.GetType().Name ?? "Null";
    }
}
