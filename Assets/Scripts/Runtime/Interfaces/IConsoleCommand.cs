using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Console;

namespace PaperSouls.Runtime.Interfaces
{
    internal interface IConsoleCommand
    {
        string Command { get; }

        string Description { get; }

        bool Process(string[] args, out ConsoleResponse msg);
    }
}
