using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "TeleportToRoomCommand", menuName = "Console Commands/Dungeon/TeleportToRoom")]
    internal sealed class TeleportToRoomCommand : DungeonCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            if (!base.Process(args, out msg)) return false;

            if (args.Length == 0)
            {
                msg = new($"Need to specify a roomID ex. '{Command} 1'.", ResponseType.Warning);
                return false;
            }

            if (!int.TryParse(args[0], out int roomID))
            {
                msg = new($"'{args[0]}' is not a vaild roomID.", ResponseType.Error);
                return false;
            }

            if(!_dungeonManager.TeleportPlayerToRoom(roomID))
            {
                msg = new($"Failed to teleport player to room {roomID}. Check if {roomID} is a vaild ID.", ResponseType.Error);
                return false;
            }

            msg = new(ResponseType.None);

            return true;
        }
    }
}
