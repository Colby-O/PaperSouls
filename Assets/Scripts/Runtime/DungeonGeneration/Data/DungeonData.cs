using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    [CreateAssetMenu(fileName = "DungeonData", menuName = "Dungeon/Data")]
    internal sealed class DungeonData : ScriptableObject
    {
        public DungeonProperties DungeonProperties;
        public RoomData RoomData;

        private void OnEnable()
        {
            if (RoomData == null || RoomData.Recipes == null) return;
            // Calculates the size of each object in the recipe.
            foreach (Recipe recipe in RoomData.Recipes)
            {
                recipe.Init();
            }
        }
    }
}
