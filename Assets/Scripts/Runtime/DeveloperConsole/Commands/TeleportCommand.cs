using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "TeleportCommand", menuName = "Console Commands/Player/Teleport")]
    internal sealed class TeleportCommand : PlayerCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            if (!base.Process(args, out msg)) return false;

            if (args.Length < 3)
            {
                msg = new($"Need to specify an x, y, and z postion ex. '{Command} 0 0 0'.", ResponseType.Warning);
                return false;
            }

            if (!float.TryParse(args[0], out float x))
            {
                msg = new($"'{args[0]}' is not a vaild x coordinate.", ResponseType.Error);
                return false;
            }

            if (!float.TryParse(args[1], out float y))
            {
                msg = new($"'{args[1]}' is not a vaild y coordinate.", ResponseType.Error);
                return false;
            }

            if (!float.TryParse(args[2], out float z))
            {
                msg = new($"'{args[2]}' is not a vaild z coordinate.", ResponseType.Error);
                return false;
            }

            _playerController.TeleportTo(new Vector3(x, y, z));

            msg = new(ResponseType.None);

            return true;
        }
    }
}
