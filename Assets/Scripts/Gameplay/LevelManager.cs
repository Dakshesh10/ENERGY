using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will be the main container/Entry point for all levels. This class is responsible for managing the 
/// level grid, managing the state of current level in play.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;

    [SerializeField]
    protected GridManager levelGrid;

    [SerializeField]
    protected int testLevel = 1;

    [SerializeField]
    protected TextAsset[] levelGridsData;

    protected int currentLevel;

    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    private void Start()
    {
        StartLevel(testLevel);
    }

    private void OnEnable()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        string data = PlayerPrefsUtil.GetData(Defines.PlayerCurrentLevel);
        if (string.IsNullOrEmpty(data))
        {
            currentLevel = 1;
            PlayerPrefsUtil.AddOrUpdateData(Defines.PlayerCurrentLevel, currentLevel.ToString());
        }
        else 
        {
            currentLevel = int.Parse(data);
        }
    }

    public IntVector2[,] GetLevelData(int level) 
    {
        return JSONUtils.ParseLevelGridJsonFile(levelGridsData[level-1]);
    }

    public void StartLevel(int level)
    {
        levelGrid.OnGameStart(GetLevelData(level));
    }
}
