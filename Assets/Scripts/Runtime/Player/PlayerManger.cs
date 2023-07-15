using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.Interfaces;
using PaperSouls.Runtime.MonoSystems.GameState;

namespace PaperSouls.Runtime.Player
{

    internal sealed class PlayerManger : MonoBehaviour, IDamageable
    {
        public PlayerSettings PlayerSettings;
        public PlayerHUDManger PlayerHUD;

        private float _currentHealth;
        private float _currentXP;
        private float _maxXP;

        /// <summary>
        /// Checks of the player's health is full
        /// </summary>
        public bool IsFullHealth()
        {
            return _currentHealth >= PlayerSettings.health;
        }

        /// <summary>
        /// Adds health to the player
        /// </summary>
        public void AddHealth(float health)
        {
            _currentHealth += health;
            PlayerHUD.UpdatePlayerHealth(_currentHealth);
        }

        /// <summary>
        /// Adds Exp to the player
        /// </summary>
        public void AddXP(float xp)
        {
            _currentXP += xp;
            PlayerHUD.UpdatePlayerXP(_currentXP);
        }

        /// <summary>
        /// Deals Damage to the player
        /// </summary>
        public void Damage(float dmg)
        {
            AddHealth(-dmg);

            if (_currentHealth <= 0) Destroy();
        }
       
        /// <summary>
        /// Process the players current level
        /// </summary>
        private void ProcessLevel()
        {
            if (_currentXP >= _maxXP)
            {
                float leftOverXP = _currentXP - _maxXP;
                PlayerHUD.LevelUp();
                _maxXP = PlayerSettings.baseXPToLevelUp + PlayerSettings.xpIncreasePerLevel * (PlayerHUD.GetCurrentLevel() - 1);
                PlayerHUD.SetMaxPlayerXP(_maxXP);
                _currentXP = leftOverXP;
                PlayerHUD.UpdatePlayerXP(_currentXP);
            }
        }

        /// <summary>
        /// Desotry the player object
        /// </summary>
        public void Destroy()
        {
            GameManager.Emit<ChangeGameStateMessage>(new(GameStates.Dead));
        }

        private void Start()
        {
            _currentHealth = PlayerSettings.health;
            _maxXP = PlayerSettings.baseXPToLevelUp;
            _currentXP = 0;

            PlayerHUD.SetMaxPlayerHealth(_currentHealth);
            PlayerHUD.SetMaxPlayerXP(_maxXP);
            PlayerHUD.UpdatePlayerHealth(_currentHealth);
            PlayerHUD.UpdatePlayerXP(_currentXP);
        }

        private void Update()
        {
            ProcessLevel();
        }
    }
}
