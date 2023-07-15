using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;

namespace PaperSouls.Runtime.MonoSystems.GameState
{
    internal interface IGameStateMonoSystem : IMonoSystem
    {
        public void ChangeToMainMenuState();

        public void ChangeToPausedState();

        public void ChangeToPlayingState();

        public void ChangeToDeadState();
    }
}
