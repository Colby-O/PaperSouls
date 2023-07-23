using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "PlayerStatusCommand", menuName = "Console Commands/Player/Status")]
    internal sealed class PlayerStatusCommand : PlayerCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            if (!base.Process(args, out msg)) return false;

            string status = _player.ToString();

            msg = new(status, ResponseType.Response);

            return true;
        }
    }
}
