using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    /// <summary>
    /// Only for testing!!!!
    /// </summary>
    internal sealed class RoomManager : MonoBehaviour
    {
        public bool Regenerate = false;
        public int Seed = 0;
        public Vector2Int RoomSize;
        public Vector3 Position = Vector3.zero;
        public int NumExits;
        RoomGenerator _roomGenerator;
        public DungeonData Data;
        GameObject _parnet;
        Room _room;

        void Start()
        {
            _roomGenerator = new(Data.RoomData, Seed);
            _parnet = new("Room");
            _parnet.transform.position = Position;
            _room = _roomGenerator.CreateRoom(_parnet, RoomSize, new(4, 1, 4), NumExits, 0);
        }


        void Update()
        {
            if (Regenerate)
            {
                Destroy(_parnet);
                _parnet = new("Room");
                _parnet.transform.position = Position;
                _room = _roomGenerator.CreateRoom(_parnet, RoomSize, new(4, 1, 4), NumExits, 0);
                Regenerate = false;
            }
        }
    }
}
