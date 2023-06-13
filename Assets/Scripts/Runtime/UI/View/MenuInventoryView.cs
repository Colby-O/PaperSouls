using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInventoryView : InventoryView
{
    public override void Show()
    {
        base.Show();
        invenotryView.SetActive(true);
        dyanmicView.SetActive(false);
        equipmentView.SetActive(true);
    }
}
