using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    [CreateAssetMenu(fileName = "DungeonData", menuName = "Dungeon/Data")]
    public sealed class DungeonData : ScriptableObject
    {
        public DungeonProperties DungeonProperties;
        public RoomData RoomData;

        private void OnEnable()
        {
            foreach (Recipe recipe in RoomData.recipes)
            {
                recipe.Init();
            }
        }
    }
}
