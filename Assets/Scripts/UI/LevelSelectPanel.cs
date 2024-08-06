using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPanel : PanelBase
{
    public Button[] levelButtons;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNumber = i + 1;
            levelButtons[i].onClick.AddListener(() => OnLevelSelect(levelNumber));
        }
    }

    private void OnLevelSelect(int levelNumber)
    {
        LevelManager.instance.SetCurrentLevelNumber(levelNumber);
        GameManager.Instance.SwitchState(GameManager.GameState.Gameplay);
    }
}
