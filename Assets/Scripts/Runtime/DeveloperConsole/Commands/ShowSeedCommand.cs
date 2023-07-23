using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "ShowSeedCommand", menuName = "Console Commands/Dungeon/ShowSeed")]
    internal sealed class ShowSeedCommand : ConsoleCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            msg = new($"Seed: {PaperSoulsGameManager.Seed}", ResponseType.Response);
            return true;
        }
    }
}
