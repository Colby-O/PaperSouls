using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.DungeonGeneration;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.DungeonGeneration;

namespace PaperSouls.Runtime.Console
{
    internal abstract class DungeonCommand : ConsoleCommand
    {
        protected IDungeonMonoSystem _dungeonManager;

        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            msg = null;

            _dungeonManager = GameManager.GetMonoSystem<IDungeonMonoSystem>();
            if (_dungeonManager == null)
            {
                msg = new($"Object with a {nameof(IDungeonMonoSystem)} component cannot be found in the scene.", ResponseType.Error);
                return false;
            }
            
            return true;
        }
    }
}
