using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "KillCommand", menuName = "Console Commands/Player/Kill")]
    internal sealed class KillCommand : PlayerCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            if (!base.Process(args, out msg)) return false;

            _player.Damage(Mathf.Infinity);

            msg = new(ResponseType.None);

            return true;
        }
    }
}
