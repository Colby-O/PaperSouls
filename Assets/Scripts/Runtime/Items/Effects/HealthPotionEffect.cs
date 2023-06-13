using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effects/HealthPotionEffect")]
public class HealthPotionEffect : UseableItemEffect
{
    public float healthIncrease = 30;

    public override bool ExcuteEffect(UseableItem item, PlayerManger player)
    {
        if (player != null && !player.IsFullHealth())
        {
            AudioManger.Instance.PlaySFX("Health Potion Use");
            player.AddHealth(healthIncrease);
            return true;
        }

        return false;
    }
}
