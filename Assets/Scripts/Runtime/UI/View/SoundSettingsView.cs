using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsView : View
{

    public Slider overallSound;
    public Slider musicSound;
    public Slider sfxSound;
    public Button backButton;

    public void ChangeOverallVolume()
    {
        AudioManger.Instance.overallSound = overallSound.value;
    }

    public void ChangeMusicVolume()
    {
        AudioManger.Instance.musicSound = musicSound.value;
    }

    public void ChangeSfxVolume()
    {
        AudioManger.Instance.sfxSound = sfxSound.value;
    }

    public override void Init()
    {
        overallSound.onValueChanged.AddListener(delegate { ChangeOverallVolume(); });
        musicSound.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
        sfxSound.onValueChanged.AddListener(delegate { ChangeSfxVolume(); });
        backButton.onClick.AddListener(ViewManger.ShowLast);
    }
}
