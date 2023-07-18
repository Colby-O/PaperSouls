using UnityEngine;
using UnityEditor;
using PaperSouls.Runtime.DungeonGeneration;

namespace PaperSouls.PaperSoulsEditor.DungeonGeneration
{
    internal sealed class DungeonSetupEditorWindow : ExtendedEditorWindow
    {
        private string _assetPath;

        [MenuItem("Window/Dungeon Setup")]
        public static void OpenWindow(DungeonData data)
        {
            DungeonSetupEditorWindow window = GetWindow<DungeonSetupEditorWindow>("Dungeon Setup");
            window._serializedObject = new SerializedObject(data);
        }

        private void OnGUI()
        {
            _currentProperty = _serializedObject.FindProperty("DungeonProperties");
            DrawProperties(_currentProperty);
            _currentProperty = _serializedObject.FindProperty("RoomData");
            DrawProperties(_currentProperty);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));

            DrawSidebar(_currentProperty);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            _serializedObject.ApplyModifiedProperties();
        }
    }
}
