using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlSettingsView : View
{
    public Button backButton;

    public override void Init()
    {
        backButton.onClick.AddListener(ViewManger.ShowLast);
    }
}
