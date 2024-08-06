using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { LevelSelect, Gameplay, GameOver }
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }
    public LevelSelectPanel levelSelectPanel;
    public GameOverPanel gameOverPanel;
    LevelManager levelManager;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        SwitchState(GameState.LevelSelect);
    }

    public void SwitchState(GameState newState)
    {
        CurrentState = newState;
        switch (CurrentState)
        {
            case GameState.LevelSelect:
                levelSelectPanel.gameObject.SetActive(true);
                gameOverPanel.gameObject.SetActive(false);
                levelSelectPanel.Show(0.2f);
                gameOverPanel.Hide(0.2f);
                break;
            case GameState.Gameplay:
                levelSelectPanel.gameObject.SetActive(false);
                gameOverPanel.gameObject.SetActive(false);
                levelSelectPanel.Hide(0.2f);
                gameOverPanel.Hide(0.2f);
                LevelManager.instance.StartLevel();
                break;
            case GameState.GameOver:
                levelSelectPanel.gameObject.SetActive(false);
                gameOverPanel.gameObject.SetActive(true);
                levelSelectPanel.Hide(0.2f);
                gameOverPanel.Show(0.2f);
                break;
        }
    }
}
