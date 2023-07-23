using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Interfaces;

namespace PaperSouls.Runtime.Console
{
    internal abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
    {
        [SerializeField] private string _command = string.Empty;
        [SerializeField] private string _description = string.Empty;

        public string Command => _command;

        public string Description => _description;


        /// <summary>
        /// Processes a console command and outputs a response from the console.
        /// </summary>
        public abstract bool Process(string[] args, out ConsoleResponse msg);
    }
}
