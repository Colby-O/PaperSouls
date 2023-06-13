using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryView : View
{
    public GameObject invenotryView;
    public GameObject dyanmicView;
    public GameObject equipmentView;

    public override void Init() { }

    public override void Hide()
    {
        base.Hide();
        invenotryView.SetActive(false);
        dyanmicView.SetActive(false);
        equipmentView.SetActive(false);
    }

    public override void Show()
    {
        base.Show();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}
