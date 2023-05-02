using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    List<LevelSettings> levels = new List<LevelSettings>();
    public static GameManager instance;

    [SerializeField]
    public Player player;

    int currentLevel = 0;

    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }
        set
        {
            currentLevel = value;
        }
    }

    private void OnEnable()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

    }

    GameState currentGameState = GameState.Menu;

    // update score on
    // enemy destroyed
    // collectibles
    // delivered


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && currentGameState != GameState.InGame)
        {
            SetGameState(GameState.InGame);
            RestartLevel();
        }
    }



    public void OnPlayerDead()
    {
        //if (OnGameOver != null)
        //{
        //    OnGameOver();
        //}
        // later make it on restart button click
        RestartLevel();

        SetGameState(GameState.Menu);
    }

    void RestartLevel()
    {
        if (OnGameStart != null)
        {
            OnGameStart();
            SetGameState(GameState.InGame);
        }
    }


    public LevelSettings GetCurrentLevel()
    {
        return levels[CurrentLevel];
    }

    public Vector2 GetPlayerStartPosition()
    {
        return levels[CurrentLevel].GetPlayerStartPos();
    }

    public void OnPlayerReachedDestination()
    {
        SetGameState(GameState.GameOver);
    }

    public GameState GetCurrentState()
    {
        return currentGameState;
    }

    void SetGameState(GameState gameState)
    {
        currentGameState = gameState;
        OnGameStateChange(gameState);
    }

    public delegate void OnGameStartHandler();
    public static event OnGameStartHandler OnGameStart;


    public delegate void OnGameStateChangeHandler(GameState gameState);
    public static event OnGameStateChangeHandler OnGameStateChange;


    public enum GameState {Menu, InGame, GameOver}
}



