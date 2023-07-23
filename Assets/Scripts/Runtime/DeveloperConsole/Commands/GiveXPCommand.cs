using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "GiveXPCommand", menuName = "Console Commands/Player/GiveXP")]
    internal sealed class GiveXPCommand : PlayerCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            if (!base.Process(args, out msg)) return false;

            if (args.Length == 0)
            {
                msg = new($"Need to specify an amount ex. '{Command} 10'.", ResponseType.Warning);
                return false;
            }

            if (!float.TryParse(args[0], out float xp))
            {
                msg = new($"'{args[0]}' is not a vaild amount of xp.", ResponseType.Error);
                return false;
            }

            _player.AddXP(xp);

            msg = new(ResponseType.None);

            return true;
        }
    }
}
