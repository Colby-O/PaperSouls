using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUDManger : MonoBehaviour
{
    public UISliderController healthBar;
    public UISliderController xpBar;
    public TextMeshProUGUI levelGUI;
    public TextMeshProUGUI ammoCountGUI;
    public int ammoCount;
    public int level;

    public InventoryHolder equipmentInventory;
    public Sprite defaultSprite;
    public Image rangedWeappon;
    public Image meleeWeappon;
    public SpriteRenderer meleeWeapponHolster;

    public void UpdateAmmoCount(int amount)
    {
        ammoCount += amount;
        ammoCountGUI.text = ammoCount.ToString();
    }

    public void IncrementAmmoCount()
    {
        UpdateAmmoCount(1);
    }

    public void DecrementAmmoCount()
    {
        UpdateAmmoCount(-1);
    }

    public int GetAmmoCount()
    {
        return ammoCount;
    }

    public void SetMaxPlayerHealth(float maxHealth)
    {
        healthBar.SetMaxValue(maxHealth);
    }

    public void IncreasePlayerHealthToMax()
    {
        healthBar.IncreaseToMax();
    }

    public void UpdatePlayerHealth(float health)
    {
        healthBar.SetValue(health);
    }

    public void UpdatePlayerXP(float xp)
    {
        xpBar.SetValue(xp);
    }

    public void SetMaxPlayerXP(float maxXP)
    {
        xpBar.SetMaxValue(maxXP);
    }

    public int GetCurrentLevel()
    {
        return level;
    }

    public void LevelUp()
    {
        level++;
        levelGUI.text = level.ToString();
    }

    public void UpdateMeleeWeaponSlot()
    {
        if (equipmentInventory.inventoryManger.inventorySlots[5].itemData != null)
        {
            Sprite meleeWeaponSprite = equipmentInventory.inventoryManger.inventorySlots[5].itemData.icon;
            meleeWeappon.sprite = meleeWeaponSprite;
            if (meleeWeapponHolster != null) meleeWeapponHolster.sprite = meleeWeappon.sprite;
        }
        else
        {
            meleeWeappon.sprite = defaultSprite;
            if (meleeWeapponHolster != null) meleeWeapponHolster.sprite = meleeWeappon.sprite;
        }
    }

    public void UpdateRangeWeaponSlot()
    {
        if (equipmentInventory.inventoryManger.inventorySlots[4].itemData != null)
        {
            Sprite rangedWeaponSprite = equipmentInventory.inventoryManger.inventorySlots[4].itemData.icon;
            rangedWeappon.sprite = rangedWeaponSprite;
        }
        else
        {
            rangedWeappon.sprite = defaultSprite;
        }
    }

    private void Start()
    {
        int.TryParse(ammoCountGUI.text, out ammoCount);
        int.TryParse(levelGUI.text, out level);
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
