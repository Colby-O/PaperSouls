using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "DisplayDungeonCommand", menuName = "Console Commands/Dungeon/DisplayDungeon")]
    internal sealed class DisplayDungeonCommand : DungeonCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            if (!base.Process(args, out msg)) return false;

            string dungeonLayout = _dungeonManager.ToString();

            msg = new(dungeonLayout, ResponseType.Response);

            return true;
        }
    }
}
