using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "ViewItemDatabaseCommand", menuName = "Console Commands/Database/ViewItemDatabase")]
    internal sealed class ViewItemDatabaseCommand : ConsoleCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            string res = PaperSoulsGameManager.ItemDatabase.ToString();

            msg = new(res, ResponseType.Response);

            return true;
        }
    }
}
