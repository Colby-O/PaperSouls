using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UseableItemEffect : ScriptableObject
{
    public abstract bool ExcuteEffect(UseableItem item, PlayerManger player);
}
