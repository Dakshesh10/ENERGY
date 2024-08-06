using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : PanelBase
{
    public TextMeshProUGUI currentScoreText;
    public Button restartButton;
    public Button levelSelectButton;

    protected override void Awake()
    {
        base.Awake();
        restartButton.onClick.AddListener(OnRestart);
        levelSelectButton.onClick.AddListener(OnLevelSelect);
    }

    public void SetScores(int currentScore, int highScore)
    {
        currentScoreText.text = "Current Score: " + currentScore;
    }

    private void OnRestart()
    {
        GameManager.Instance.SwitchState(GameManager.GameState.Gameplay);
    }

    private void OnLevelSelect()
    {
        GameManager.Instance.SwitchState(GameManager.GameState.LevelSelect);
    }
}
