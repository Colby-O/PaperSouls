using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Console
{
    [System.Serializable]
    internal sealed class ConsoleResponse
    {
        public ResponseType Type;
        public string Message = string.Empty;
        public Color MessageColor { get; private set; }

        public ConsoleResponse(ResponseType type)
        {
            Type = type;
        }

        public ConsoleResponse(string msg, ResponseType type)
        {
            Message = msg;
            Type = type;
            MessageColor = GetMessageColor();
        }

        private Color GetMessageColor()
        {
            return Type switch
            {
                ResponseType.Error => Color.red,
                ResponseType.Warning => Color.yellow,
                ResponseType.Response => Color.magenta,
                ResponseType.Help => Color.magenta,
                _ => Color.white,
            };
        }
    }
}
