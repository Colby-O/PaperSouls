using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerManger : MonoBehaviour, IDamageable
{
    public PlayerSettings playerSettings;
    public PlayerHUDManger playerHUD;

    private float currentHealth;
    private float currentXP;
    private float maxXP;

    public bool IsFullHealth()
    {
        return currentHealth >= playerSettings.health;
    }

    public void AddHealth(float health)
    {
        currentHealth += health;
        playerHUD.UpdatePlayerHealth(currentHealth);
    }

    public void AddXP(float xp)
    {
        currentXP += xp;
        playerHUD.UpdatePlayerXP(currentXP);
    }

    public void Damage(float dmg)
    {
        AddHealth(-dmg);

        if (currentHealth <= 0) Destroy();
    }

    public void Destroy()
    {
        GameManger.UpdateGameState(GameState.PlayerDead);
        GameObject.Destroy(gameObject);
    }

    private void Start()
    {
        currentHealth = playerSettings.health;
        maxXP = playerSettings.baseXPToLevelUp;
        currentXP = 0;

        playerHUD.SetMaxPlayerHealth(currentHealth);
        playerHUD.SetMaxPlayerXP(maxXP);
        playerHUD.UpdatePlayerHealth(currentHealth);
        playerHUD.UpdatePlayerXP(currentXP);
    }

    private void Update()
    {
        if (currentXP >= maxXP)
        {
            float leftOverXP = currentXP - maxXP;
            playerHUD.LevelUp();
            maxXP = playerSettings.baseXPToLevelUp + playerSettings.xpIncreasePerLevel * (playerHUD.GetCurrentLevel() - 1);
            playerHUD.SetMaxPlayerXP(maxXP);
            currentXP = leftOverXP;
            playerHUD.UpdatePlayerXP(currentXP);
        }
    }
}
