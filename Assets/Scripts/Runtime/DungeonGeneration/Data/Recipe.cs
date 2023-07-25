using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    [System.Serializable]
    internal sealed class ZonePlacementProbability
    {
        [Range(0, 1)] public float Room = 1.0f;
        [Range(0, 1)] public float Edge = 1.0f;
    }

    [System.Serializable]
    internal sealed class FillableItem
    {
        public GameObject Item;
        [Range(0, 1)] public float Probability = 1.0f;
        public Vector3 Scale = Vector3.one;
        public float Offset = 0.0f;
    }

    [System.Serializable]
    internal sealed class Recipe
    {
        public List<DecorationObject> Objects;
        public List<FillableItem> Fillables;
        public ZonePlacementProbability PlacementProbability;

        private float RoundSize(float val)
        {
            return (val < 1) ? Mathf.Ceil(val) : Mathf.Round(val);
        }

        public void Init()
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
