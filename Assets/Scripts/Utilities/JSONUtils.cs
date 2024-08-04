using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Drawing;

public class JSONUtils: MonoBehaviour 
{
    [SerializeField]
    protected TextAsset jsonAsset;

    private void OnEnable()
    {
        IntVector2[,] grid = ParseJsonFile(jsonAsset);

        
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    Debug.Log($"JSON - {grid[i,j]}");
                }
            }
        
    }

    public static IntVector2[,] ParseJsonFile(TextAsset jsonAsset)
    {
        if (jsonAsset == null)
        {
            Debug.LogError("JSON TextAsset is null.");
            return null;
        }

        // Parse the JSON into a nested list structure
        var data = JsonConvert.DeserializeObject<Dictionary<string, List<List<IntVector2>>>>(jsonAsset.text);
        var nestedList = data["data"];

        // Determine the dimensions of the array
        int rows = nestedList.Count;
        int cols = nestedList[0].Count;

        IntVector2[,] resultArray = new IntVector2[rows, cols];

        // Fill the array with the parsed data, starting from the last line
        for (int i = 0; i < rows; i++)
        {
            var row = nestedList[rows - 1 - i];
            for (int j = 0; j < cols; j++)
            {
                resultArray[i, j] = row[j];
            }
        }

        return resultArray;
    }

}
