using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalInventoryView : InventoryView
{
    public override void Show()
    {
        base.Show();
        invenotryView.SetActive(true);
        dyanmicView.SetActive(true);
        equipmentView.SetActive(false);
    }
}
