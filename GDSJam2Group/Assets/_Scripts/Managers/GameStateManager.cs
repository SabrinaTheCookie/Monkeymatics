using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameState
{
    public string name;
    public float score;
    public bool isVictorious;
    public float playTime;
}
public class GameStateManager : Singleton<GameStateManager>
{
    [field: SerializeField]
    public bool IsPaused { get; private set; }

    private GameState gameState;
    
    public static event Action OnGameOver;
    public static event Action OnGameStart;
    public static event Action<bool> OnPaused;

    public int currentStage = 1;
    public int numberOfStages;
    public static event Action<int> StageStarted;

    public List<CableLineRenderer> cables;
    public MonkeyManager monkeyManager;


    void OnEnable()
    {
        Fabricator.OnFabricated += NextStage;
    }
    void OnDisable()
    {
        Fabricator.OnFabricated -= NextStage;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    public void NextStage(int numberOfFabrications)
    {
        currentStage++;
        if (currentStage > numberOfStages)
        {
            GameOver(true);
            return;
        }
        StageStarted?.Invoke(currentStage);
    }

    public void StartGame()
    {
        if(IsPaused || Time.timeScale == 0) ResumeGame();
        gameState = new GameState();
        StageStarted?.Invoke(currentStage);
        OnGameStart?.Invoke();

    }

    // Update is called once per frame
    void Update()
    {
        if (IsPaused) return;
        GameUpdate();
    }

    void GameUpdate()
    {
        // Renders dynamic cables
        foreach (var cable in cables)
        {
            cable.CableUpdate();
        }
        if(monkeyManager.currentMonkey)
            monkeyManager.currentMonkey.PlayerUpdate();

    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        IsPaused = true;
        OnPaused?.Invoke(IsPaused);
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1;
        IsPaused = false;
        OnPaused?.Invoke(IsPaused);
    }

    public void GameOver(bool isVictory)
    {
        if (isVictory)
        {
            UpdateGameState(null, 0, 0, true); 
        }

        OnGameOver?.Invoke();
    }

    public void UpdateGameState(string name=null, float scoreToAdd=0, float playTimeToAdd=0, bool isVictorious=false)
    {
        if (name != null)
        {
            gameState.name = name;
        }

        if (scoreToAdd > 0)
        {
            gameState.score += scoreToAdd;
        }

        if (playTimeToAdd > 0)
        {
            gameState.playTime += playTimeToAdd;
        }

        if (isVictorious)
        {
            gameState.isVictorious = true;
        }
    }

    public GameState GetGameState()
    {
        return gameState;
    }

}
