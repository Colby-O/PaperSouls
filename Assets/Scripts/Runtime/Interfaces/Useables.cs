using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUseables
{
    public void Use(PlayerManger player);
    public void Use(PlayerManger player, out bool sucessful);
}
