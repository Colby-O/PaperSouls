using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;

namespace PaperSouls.Runtime
{
    internal sealed class StartGameMessage : IMessage
    {
        public StartGameMessage()
        {

        }
    }

    internal sealed class RestartGameMessage : IMessage
    {
        public RestartGameMessage()
        {

        }
    }

    internal sealed class GotoMainMenuMessage : IMessage
    {
        public GotoMainMenuMessage()
        {

        }
    }
}
