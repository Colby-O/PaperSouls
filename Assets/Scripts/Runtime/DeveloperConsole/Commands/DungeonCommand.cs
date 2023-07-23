using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.DungeonGeneration;

namespace PaperSouls.Runtime.Console
{
    internal abstract class DungeonCommand : ConsoleCommand
    {
        protected DungeonManager _dungeonManager;

        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            msg = null;

            _dungeonManager = FindObjectOfType<DungeonManager>();
            if (_dungeonManager == null)
            {
                msg = new($"Object with a {nameof(DungeonManager)} component cannot be found in the scene.", ResponseType.Error);
                return false;
            }

            return true;
        }
    }
}
