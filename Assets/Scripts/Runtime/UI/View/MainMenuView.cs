using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuView : View
{
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;

    public void StartGame()
    {
        GameManger.UpdateGameState(GameState.Playing);
        SceneManager.LoadScene("TestScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        ViewManger.Show<SettingsView>();
    }

    public override void Init()
    {
        playButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);
    }

    public override void Show()
    {
        base.Show();
        GameManger.UpdateGameState(GameState.InMainMenu);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}
