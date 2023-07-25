using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;

namespace PaperSouls.Runtime.MonoSystems.DungeonGeneration
{
    internal interface IDungeonMonoSystem : IMonoSystem
    {
        public bool TeleportTo(int id);
        public string DungeonToString();
    }
}
