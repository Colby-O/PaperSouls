using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "ClearHistoryCommand", menuName = "Console Commands/General/ClearHistory")]
    internal sealed class ClearHistoryCommand : ConsoleCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            msg = new(ResponseType.ClearHistory);
            return true;
        }
    }
}
