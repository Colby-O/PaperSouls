using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    InMainMenu,
    Playing,
    InMenu,
    PlayerDead
}

public class GameManger : MonoBehaviour
{
    private static GameManger instance;
    public static GameManger Instance
    {
        get
        {
            if (instance == null) Debug.Log("Game Manger is null!!!");
            return instance;
        }
        private set {}
    }

    public GameObject player;
    public ItemDatabase itemDatabase;
    public GameState state;

    public static bool accpetPlayerInput { get; private set; }

    public static void UpdateGameState(GameState newState)
    {
        instance.state = newState;

        switch (instance.state)
        {
            case GameState.InMainMenu:
                break;
            case GameState.Playing:
                accpetPlayerInput = true;
                break;
            case GameState.InMenu:
                accpetPlayerInput = false;
                break;
            case GameState.PlayerDead:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                UpdateGameState(GameState.Playing);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(instance.state), instance.state, null);
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        instance = this;
        itemDatabase.SetItemIDs();
        state = GameState.Playing;
        accpetPlayerInput = true;
    }

    private void Update()
    {
        if (instance.player == null) instance.player = GameObject.Find("Player");
    }
}
