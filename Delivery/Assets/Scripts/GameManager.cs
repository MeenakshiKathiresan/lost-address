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
        
    }

    public void OnPlayerDead()
    {
        //if (OnGameOver != null)
        //{
        //    OnGameOver();
        //}
        // later make it on restart button click
        RestartLevel();
    }

    void RestartLevel()
    {
        if (OnGameStart != null)
        {
            OnGameStart();
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
        OnWin();
    }



    public delegate void OnGameOverHandler();
    public static event OnGameOverHandler OnGameOver;

    public delegate void OnGameStartHandler();
    public static event OnGameStartHandler OnGameStart;

    public delegate void OnWinHandler();
    public static event OnWinHandler OnWin;

    public enum GameStates {Menu, InGame, GameOver}
}



