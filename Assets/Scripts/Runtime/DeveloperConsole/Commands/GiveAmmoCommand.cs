using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "GiveAmmoCommand", menuName = "Console Commands/Player/GiveAmmo")]
    internal sealed class GiveAmmoCommand : PlayerCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            if (!base.Process(args, out msg)) return false;

            if (args.Length == 0)
            {
                msg = new($"Need to specify an amount ex. '{Command} 10'.", ResponseType.Warning);
                return false;
            }

            if (!int.TryParse(args[0], out int ammo))
            {
                msg = new($"'{args[0]}' is not a vaild amount of ammo.", ResponseType.Error);
                return false;
            }

            _player.PlayerHUD.UpdateAmmoCount(ammo);

            msg = new(ResponseType.None);

            return true;
        }
    }
}
