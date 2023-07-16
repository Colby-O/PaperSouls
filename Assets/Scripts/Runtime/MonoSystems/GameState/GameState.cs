using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.MonoSystems.GameState
{
    internal enum GameStates
    {
        /// <summary>
        /// Main menu scene is active
        /// </summary>
        MainMenu,
        /// <summary>
        /// Player is alive in the game
        /// </summary>
        Playing,
        /// <summary>
        /// Pause menu is active
        /// </summary>
        Paused,
        /// <summary>
        /// Player is dead
        /// </summary>
        Dead
    }
}
