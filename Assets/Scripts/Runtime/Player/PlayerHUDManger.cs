using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PaperSouls.Runtime.UI;
using PaperSouls.Runtime.Inventory;

namespace PaperSouls.Runtime.Player
{

    internal sealed class PlayerHUDManger : MonoBehaviour
    {
        [SerializeField] private UISliderController _healthBar;
        [SerializeField] private UISliderController _xpBar;
        [SerializeField] private TextMeshProUGUI _levelGUI;
        [SerializeField] private TextMeshProUGUI _ammoCountGUI;
        public InventoryHolder EquipmentInventory;
        public UnityEngine.Sprite DefaultSprite;
        public Image RangedWeappon;
        public Image MeleeWeappon;
        public SpriteRenderer MeleeWeapponHolster;

        private int _ammoCount;
        private int _level;
        private float _maxHealth;

        /// <summary>
        /// Updates UI ammo count
        /// </summary>
        public void UpdateAmmoCount(int amount)
        {
            _ammoCount += amount;
            _ammoCount = (_ammoCount >= 0) ? _ammoCount : 0;
            _ammoCountGUI.text = _ammoCount.ToString();
        }

        /// <summary>
        /// Increases the ammo count by 1
        /// </summary>
        public void IncrementAmmoCount()
        {
            UpdateAmmoCount(1);
        }

        /// <summary>
        /// Decreases the ammo count by 1
        /// </summary>
        public void DecrementAmmoCount()
        {
            UpdateAmmoCount(-1);
        }

        public void SetAmmoCount(int amount)
        {
            _ammoCount = (amount >= 0) ? amount : 0;
            _ammoCountGUI.text = _ammoCount.ToString();
        }

        /// <summary>
        /// Gets the current ammo count
        /// </summary>
        public int GetAmmoCount()
        {
            return _ammoCount;
        }

        /// <summary>
        /// Gets the player max health in the UI
        /// </summary>
        public float GetMaxPlayerHealth()
        {
            return _maxHealth;
        }

        /// <summary>
        /// Sets the player max health in the UI
        /// </summary>
        public void SetMaxPlayerHealth(float maxHealth)
        {
            _maxHealth = maxHealth;
            _healthBar.SetMaxValue(maxHealth);
        }

        /// <summary>
        /// Increasees the players max health to max value
        /// </summary>
        public void IncreasePlayerHealthToMax()
        {
            _healthBar.IncreaseToMax();
        }

        /// <summary>
        /// Updates the players health in the UI
        /// </summary>
        public void UpdatePlayerHealth(float health)
        {
            _healthBar.SetValue(health);
        }

        /// <summary>
        /// Updates the players exp in the UI
        /// </summary>
        public void UpdatePlayerXP(float xp)
        {
            _xpBar.SetValue(xp);
        }

        /// <summary>
        /// Sets the player max exp in the UI
        /// </summary>
        public void SetMaxPlayerXP(float maxXP)
        {
            _xpBar.SetMaxValue(maxXP);
        }

        /// <summary>
        /// Gets the players current level
        /// </summary>
        public int GetLevel()
        {
            return _level;
        }

        public void SetLevel(int level)
        {
            _level = (level >= 1) ? level : 1;
            _levelGUI.text = _level.ToString();
        }

        /// <summary>
        /// Increases the players level in the UI
        /// </summary>
        public void LevelUp()
        {
            _level++;
            _levelGUI.text = _level.ToString();
        }

        /// <summary>
        /// Update Melee weapon slot
        /// </summary>
        public void UpdateMeleeWeaponSlot()
        {
            if (EquipmentInventory.InventoryManger.InventorySlots.Count < 5) return;
            if (EquipmentInventory.InventoryManger.InventorySlots[5].ItemData != null)
            {
                UnityEngine.Sprite meleeWeaponSprite = EquipmentInventory.InventoryManger.InventorySlots[5].ItemData.icon;
                MeleeWeappon.sprite = meleeWeaponSprite;
                if (MeleeWeapponHolster != null) MeleeWeapponHolster.sprite = MeleeWeappon.sprite;
            }
            else
            {
                MeleeWeappon.sprite = DefaultSprite;
                if (MeleeWeapponHolster != null) MeleeWeapponHolster.sprite = MeleeWeappon.sprite;
            }
        }

        /// <summary>
        /// Update Ranged weapon slot
        /// </summary>
        public void UpdateRangeWeaponSlot()
        {
            if (EquipmentInventory.InventoryManger.InventorySlots.Count < 4) return;
            if (EquipmentInventory.InventoryManger.InventorySlots[4].ItemData != null)
            {
                UnityEngine.Sprite rangedWeaponSprite = EquipmentInventory.InventoryManger.InventorySlots[4].ItemData.icon;
                RangedWeappon.sprite = rangedWeaponSprite;
            }
            else
            {
                RangedWeappon.sprite = DefaultSprite;
            }
        }

        private void Start()
        {
            int.TryParse(_ammoCountGUI.text, out _ammoCount);
            int.TryParse(_levelGUI.text, out _level);
            UpdateRangeWeaponSlot();
            UpdateMeleeWeaponSlot();
        }

        private void Update()
        {
            // TODO: Make Below OnChange Events
            UpdateRangeWeaponSlot();
            UpdateMeleeWeaponSlot();
        }
    }
}
