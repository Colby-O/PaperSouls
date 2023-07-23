using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.UI;
using PaperSouls.Runtime.MonoSystems.GameState;
using PaperSouls.Runtime.UI.View;

namespace PaperSouls.Runtime.Console
{
    [CreateAssetMenu(fileName = "CloseCommand", menuName = "Console Commands/General/Close")]
    internal sealed class CloseCommand : ConsoleCommand
    {
        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
            if (GameManager.GetMonoSystem<IUIMonoSystem>().GetCurrentViewIs<PlayerHUDView>()) GameManager.Emit<ChangeGameStateMessage>(new(GameStates.Playing));
            msg = new(ResponseType.None);
            return true;
        }
    }
}
