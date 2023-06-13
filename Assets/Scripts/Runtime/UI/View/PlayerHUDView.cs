using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUDView : View
{
    public override void Init()
    {
       
    }

    public override void Show()
    {
        base.Show();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
