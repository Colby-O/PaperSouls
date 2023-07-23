using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "QuitCommand", menuName = "Console Commands/General/Quit")]
    internal sealed class QuitCommand : ConsoleCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            GameManager.Emit<QuitGameMessage>(new());
            msg = new(ResponseType.None);
            return true;
        }
    }
}
