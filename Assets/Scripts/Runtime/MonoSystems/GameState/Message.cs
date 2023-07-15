using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;

namespace PaperSouls.Runtime.MonoSystems.GameState
{
    internal sealed class ChangeGameStateMessage : IMessage
    {
        public GameStates NextGameState { get; private set; }

        public ChangeGameStateMessage(GameStates nextGameState)
        {
            NextGameState = nextGameState;
        }
    }
}
