using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PaperSouls.PaperSoulsEditor
{
    internal class ExtendedEditorWindow : EditorWindow
    {
        protected SerializedObject _serializedObject;
        protected SerializedProperty _currentProperty;

        private string _selectedPropertyPath;
        protected SerializedProperty _selectedProperty;

        private string _lastPath;

        private void DrawPropertyField(SerializedProperty property)
        {
            if (property.isArray && property.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.displayName, true);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                if (!string.IsNullOrEmpty(_lastPath) && property.propertyPath.Contains(_lastPath)) return;
                _lastPath = property.propertyPath;
                EditorGUILayout.PropertyField(property);
            }

        }

        protected void DrawProperties(SerializedProperty property)
        {
            if (property == null)
            {
                EditorGUILayout.HelpBox("SerializedProperty was null!", MessageType.Error);
                return;
            }

            int startingDepth = property.depth;
            _lastPath = string.Empty;

            do
            {
                EditorGUI.indentLevel = property.depth;
                DrawPropertyField(property);
            } while 
                (
                property.NextVisible(property.isExpanded) &&
                property.depth > startingDepth
                );
        }

        protected void DrawSidebar(SerializedProperty property)
        {
            foreach (SerializedProperty prop in property)
            {
                if (GUILayout.Button(prop.displayName)) _selectedPropertyPath = prop.propertyPath;
            }

            if (!string.IsNullOrEmpty(_selectedPropertyPath))
            {
                _selectedProperty = _serializedObject.FindProperty(_selectedPropertyPath);
            }
        }
    }
}
