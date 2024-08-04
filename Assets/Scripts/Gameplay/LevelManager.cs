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
    GridManager levelGrid;

    [SerializeField]
    protected TextAsset[] levelGridsData;

    protected int currentLevel;

    public Dictionary<Defines.CellTypes, GameObject> cellTypeDictionary = null;
    // Start is called before the first frame update
    private void Awake()
    {
        if (cellTypeDictionary == null)
        {
            cellTypeDictionary = new Dictionary<Defines.CellTypes, GameObject>();
            //Load prefabs from the resources and set them to the dictionary.
            int n = (int)Defines.CellTypes.Max;
            GameObject loadedPrefab;
            for (int i = 0; i < n; i++)
            {
                //Defines.CellTypes tileType = (Defines.CellTypes)i;
                string prefabName = Defines.CellPrefabNames[i];
                if (string.IsNullOrEmpty(prefabName))
                    continue;

                loadedPrefab = Resources.Load("Prefabs/" + prefabName) as GameObject;
                if (loadedPrefab != null)
                {
                    GameObject prefab = Instantiate(loadedPrefab) as GameObject;
                    cellTypeDictionary.Add((Defines.CellTypes)i, prefab);
                    prefab.SetActive(false);
                }
            }
        }
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

    public void StartLevel()
    {
        
    }
}
