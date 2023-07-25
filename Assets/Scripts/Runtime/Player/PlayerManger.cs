using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.Interfaces;
using PaperSouls.Runtime.MonoSystems.GameState;
using PaperSouls.Runtime.Inventory;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.Player
{

    internal sealed class PlayerManger : MonoBehaviour, IDamageable, IDataPersistence
    {
        public PlayerSettings PlayerSettings;
        public PlayerHUDManger PlayerHUD;

        public InventoryHolder ItemInventory;
        public InventoryHolder EquipmentInventory;

        private float _currentHealth;
        private float _maxHealth;
        private float _currentXP;
        private float _maxXP;
        private int _level;
        private int _ammoCount = 30;

        private Vector3 lastPos;
        public void SetMaxHealth(float maxHealth)
        {
            PlayerHUD.SetMaxPlayerHealth(maxHealth);
        }

        /// <summary>
        /// Checks of the player's health is full
        /// </summary>
        public bool IsFullHealth()
        {
            return _currentHealth >= PlayerHUD.GetMaxPlayerHealth();
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
            if (GameManager.GetMonoSystem<IGameStateMonoSystem>().GetCurrentState() == GameStates.Dead) return;

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
                _maxXP = PlayerSettings.baseXPToLevelUp + PlayerSettings.xpIncreasePerLevel * (PlayerHUD.GetLevel() - 1);
                PlayerHUD.SetMaxPlayerXP(_maxXP);
                _currentXP = leftOverXP;
                PlayerHUD.UpdatePlayerXP(_currentXP);
            }
        }

        /// <summary>
        /// Resets the player's health to max value.
        /// </summary>
        public void ResetHealth()
        {
            _currentHealth = PlayerSettings.health;
            PlayerHUD.UpdatePlayerHealth(_currentHealth);
        }

        /// <summary>
        /// Desotry the player object
        /// </summary>
        public void Destroy()
        {
            GameManager.Emit<ChangeGameStateMessage>(new(GameStates.Dead));
            ResetHealth();
        }

        public bool SaveData(ref GameData data)
        {
            data.CurrentHealth = _currentHealth;
            data.CurrentXP = _currentXP;
            data.CurrentLevel = PlayerHUD.GetLevel();
            data.AmmoCount = PlayerHUD.GetAmmoCount();
            data.Position = lastPos;
            data.MaxHealth = PlayerHUD.GetMaxPlayerHealth();
            return true;
        }

        public bool LoadData(GameData data)
        {
            _currentHealth = data.CurrentHealth;
            _currentXP = data.CurrentXP;
            _level = data.CurrentLevel;
            _ammoCount = (data.AmmoCount >= 0) ? data.AmmoCount : _ammoCount;
            _maxHealth = data.MaxHealth;
            return true;
        }

        public override string ToString()
        {
            return $"Health: {_currentHealth} / {PlayerHUD.GetMaxPlayerHealth()}\n" +
                   $"Level: {PlayerHUD.GetLevel()}\n" +
                   $"Current XP: {_currentXP}\n" +
                   $"XP needed for next level: {_maxXP}\n" +
                   $"Ammo Count: {PlayerHUD.GetAmmoCount()}";
        }

        private void Start()
        {
            _currentHealth = (_currentHealth <= 0) ? PlayerSettings.health : _currentHealth;
            _maxHealth = (_maxHealth <= 0) ? PlayerSettings.health : _maxHealth;
            _maxXP = (_level <= 1) ? PlayerSettings.baseXPToLevelUp : PlayerSettings.baseXPToLevelUp + PlayerSettings.xpIncreasePerLevel * (_level - 1);
            _currentXP = (_currentXP <= 0) ? 0 : _currentXP;

            PlayerHUD.SetAmmoCount(_ammoCount);
            PlayerHUD.SetLevel(_level);
            PlayerHUD.SetMaxPlayerHealth(_maxHealth);
            PlayerHUD.SetMaxPlayerXP(_maxXP);
            PlayerHUD.UpdatePlayerHealth(_currentHealth);
            PlayerHUD.UpdatePlayerXP(_currentXP);
        }

        private void Update()
        {
            ProcessLevel();
            lastPos = transform.position;
        }
    }
}
