using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    [System.Serializable]
    public class ZonePlacementProbability
    {
        [Range(0, 1)] public float Room = 1.0f;
        [Range(0, 1)] public float Edge = 1.0f;
    }

    [CreateAssetMenu(fileName = "Recipe", menuName = "Dungeon/Dectoration/Recipe", order = 2)]
    public class Recipe : ScriptableObject
    {
        public List<DecorationObject> Objects;
        public ZonePlacementProbability PlacementProbability;

        private float RoundSize(float val)
        {
            return (val < 1) ? Mathf.Ceil(val) : Mathf.Round(val);
        }

        private void OnEnable()
        {
            foreach (var obj in Objects)
            {
                obj.CalculateSize();
                if (obj.zone == RoomZone.Edge) obj.SetSize(new(RoundSize(obj.Size.x + obj.Padding), RoundSize(obj.Size.y), RoundSize(obj.Size.z)));
                else obj.SetSize(new(RoundSize(obj.Size.x + obj.Padding), RoundSize(obj.Size.y), RoundSize(obj.Size.z + obj.Padding)));
            }
        }
    }
}
