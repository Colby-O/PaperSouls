using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : View
{
    public Button graphicSettingsButton;
    public Button controlSettingsButton;
    public Button soundSettingsButton;
    public Button backButton;


    public void OpenSoundSettings()
    {
        ViewManger.Show<SoundSettingsView>();
    }

    public void OpenControlSettings()
    {
        ViewManger.Show<ControlSettingsView>();
    }

    public override void Init()
    {
        controlSettingsButton.onClick.AddListener(OpenControlSettings);
        soundSettingsButton.onClick.AddListener(OpenSoundSettings);
        backButton.onClick.AddListener(ViewManger.ShowLast);
    }
}
