using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effects/ExpPotionEffect")]
public class ExpPotionEffect : UseableItemEffect
{
    public float expIncrease = 30;

    public override bool ExcuteEffect(UseableItem item, PlayerManger player)
    {
        if (player != null)
        {
            AudioManger.Instance.PlaySFX("Health Potion Use");
            player.AddXP(expIncrease);
            return true;
        }

        return false;
    }
}
