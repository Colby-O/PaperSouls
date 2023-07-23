using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "GetPosCommand", menuName = "Console Commands/Player/GetPos")]
    internal sealed class GetPosCommand : PlayerCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            if (!base.Process(args, out msg)) return false;
            msg = new(_playerController.transform.position.ToString(), ResponseType.Response);
            return true;
        }
    }
}
