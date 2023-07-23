using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "SetMaxHealthCommand", menuName = "Console Commands/Player/SetMaxHealth")]
    internal sealed class SetMaxHealthCommand : PlayerCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            if (!base.Process(args, out msg)) return false;

            if (args.Length == 0)
            {
                msg = new($"Need to specify an amount ex. '{Command} 10'.", ResponseType.Warning);
                return false;
            }

            if (!float.TryParse(args[0], out float maxHealth))
            {
                msg = new($"'{args[0]}' is not a vaild amount of health.", ResponseType.Error);
                return false;
            }

            _player.SetMaxHealth(maxHealth);

            msg = new(ResponseType.None);

            return true;
        }
    }
}
