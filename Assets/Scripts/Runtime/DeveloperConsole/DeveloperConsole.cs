using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PaperSouls.Runtime.Interfaces;

namespace PaperSouls.Runtime.Console
{
    internal sealed class DeveloperConsole
    {
        private readonly IEnumerable<IConsoleCommand> _commands;

        public DeveloperConsole(IEnumerable<IConsoleCommand> commands)
        {
            _commands = commands;
        }

        /// <summary>
        /// Processes a input string and extracts the command
        /// </summary>
        public ConsoleResponse ProcessCommand(string input)
        {
            ConsoleResponse msg = null;

            string[] inputSplit = input.Split(' ');

            string command = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();

            CallCommand(command, args, out msg);

            if (msg == null) msg = new($"'{input}' is an invaild command.", ResponseType.Error);
            
            return msg;
        }

        /// <summary>
        /// Calls a command with args
        /// </summary>
        public void CallCommand(string commandString, string[] args, out ConsoleResponse msg)
        {
            msg = null;
            foreach (IConsoleCommand command in _commands)
            {
                if (!commandString.Equals(command.Command, System.StringComparison.OrdinalIgnoreCase)) continue;
                if (command.Process(args, out msg)) return;
            }
        }
    }
}
