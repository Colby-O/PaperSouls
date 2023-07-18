using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using PaperSouls.Runtime.DungeonGeneration;

namespace PaperSouls.PaperSoulsEditor.DungeonGeneration
{
    internal sealed class AssetHandler
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceID, int line)
        {
            DungeonData obj = EditorUtility.InstanceIDToObject(instanceID) as DungeonData;
            if (obj)
            {
                DungeonSetupEditorWindow.OpenWindow(obj);
                return true;
            }

            return false;
        }
    }

    //[CustomEditor(typeof(DungeonData))]
    internal sealed class DungeonSetupEditor : Editor
{
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                DungeonSetupEditorWindow.OpenWindow((DungeonData)target);
            }
        }
    }
}
