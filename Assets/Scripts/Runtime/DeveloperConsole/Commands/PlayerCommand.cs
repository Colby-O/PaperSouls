using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.Console
{
    internal abstract class PlayerCommand : ConsoleCommand
    {
        protected PlayerManger _player;
        protected PlayerController _playerController;

        public override bool Process(string[] args, out ConsoleResponse msg)
        {
            msg = null;

            if (PaperSoulsGameManager.Player == null)
            {
                msg = new("Player is not in the scene.", ResponseType.Error);
                return false;
            }

            if (!PaperSoulsGameManager.Player.TryGetComponent<PlayerManger>(out _player))
            {
                msg = new($"Player does not have a {nameof(PlayerManger)} component.", ResponseType.Error);
                return false;
            }

            if (!PaperSoulsGameManager.Player.TryGetComponent<PlayerController>(out _playerController))
            {
                msg = new($"Player does not have a {nameof(PlayerController)} component.", ResponseType.Error);
                return false;
            }

            return true;
        }
    }
}
