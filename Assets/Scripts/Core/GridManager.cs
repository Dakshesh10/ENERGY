using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GridManager : MonoBehaviour
{
    Cell[,] Grid;

    public IntVector2 size;

    private Vector2 gridOffset;

    public Vector2 cellScale;

    public Dictionary<Defines.CellTypes, GameObject> cellTypeDictionary = null;

    public List<Cell> sourceCells;

    protected IntVector2[,] currentLevelData;

    [Tooltip("JSON for editor script and test JSON based level data. Generate button uses this JSON to generate grid in Scene in Editor")]
    public TextAsset editorLevelJSON;
    public IntVector2[,] CurrentLevelData {
        get {  return currentLevelData; }
        
        set 
        { 
            currentLevelData = new IntVector2[value.GetLength(0), value.GetLength(1)];
            currentLevelData = value;
            size.x = currentLevelData.GetLength(0);
            size.y = currentLevelData.GetLength(1);
        } 
    }

    private void OnEnable()
    {
        
    }

    private void Awake()
    {
        if(cellTypeDictionary == null)
            cellTypeDictionary = new Dictionary<Defines.CellTypes, GameObject>();
        //Load prefabs from the resources and set them to the dictionary.
        int n = (int)Defines.CellTypes.Max;
        GameObject loadedPrefab;
        for (int i = 0; i < n; i++)
        {
            //Defines.CellTypes tileType = (Defines.CellTypes)i;
            string prefabName = Defines.CellPrefabNames[i];
            if (string.IsNullOrEmpty(prefabName) || cellTypeDictionary.ContainsKey((Defines.CellTypes)i))
                continue;

            loadedPrefab = Resources.Load("Prefabs/" + prefabName) as GameObject;
            if (loadedPrefab != null)
            {
                GameObject prefab = Instantiate(loadedPrefab) as GameObject;
                cellTypeDictionary.TryAdd((Defines.CellTypes)i, prefab);
                prefab.SetActive(false);
            }
        }
    }

    private void Start()
    {
        //OnGameStart();
    }

    void InitializeGrid()
    {
        //Destroy all child objects for new Grid.
        if (transform.childCount > 0)
        {
            var children = new List<GameObject>();
            foreach (Transform child in transform)
                children.Add(child.gameObject);
            children.ForEach((c) =>
            {
                if (Application.isPlaying)
                    Destroy(c.gameObject);
                else
                {
                    DestroyImmediate(c.gameObject);
                }
                //DestroyImmediate(child.gameObject);
            });
        }
        Grid = new Cell[size.x, size.y];
    }

    /// <summary>
    /// Generates the grid based on 2D array deserialized from level-JSON file.
    /// </summary>
    /// <param name="gridData">2D array buffer which maps as is to the actual grid, 
    /// and properties X and Y represent Cell type and initial rotation of cell respectively</param>
    void GenerateGrid(bool isEditorTriggered = false)
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                SpawnTile(new IntVector2(i, j), (Defines.CellTypes)currentLevelData[i, j].x, currentLevelData[i, j].y, isEditorTriggered);
            }
        }
    }

    public Defines.CellTypes getRandomCellType()
    {
        return (Defines.CellTypes)UnityEngine.Random.Range(0, (int)Defines.CellTypes.Max);
    }

    /// <summary>
    /// Spawns tile at requested x and y matrix indeces. 
    /// </summary>
    /// <param name="coords"> X and Y coords in Grid space to spawn at </param>
    /// <param name="cellType"> Cell type </param>
    /// <param name="startRotation"> Initial rotation of cell. </param>
    /// <param name="isEditorTriggered"> Flag for if cell spawn was triggered by Editor button callback.</param>
    void SpawnTile(IntVector2 coords, Defines.CellTypes cellType, int startRotation = 0, bool isEditorTriggered = false)
    {
        if (!coords.isInRange(IntVector2.Zero, size))
            return;

        GameObject tile = Instantiate(cellTypeDictionary[cellType]);


        Cell cell = tile.GetComponent<Cell>();
        tile.name = $"{cellType.ToString()}-{coords.ToString()}";
        Grid[coords.x, coords.y] = cell;
        cell.Initialize(coords, startRotation);

        if (!isEditorTriggered && cell.thisCellType != Defines.CellTypes.Solid)
        {
            cell.OnGameStart(OnCellClickedComplete);
        }

        gridOffset.x = -(size.x / 2);
        gridOffset.y = -(size.y / 2);

        if (size.y % 2 == 0)
            gridOffset.y += 0.5f;

        if (size.x % 2 == 0)
            gridOffset.x += 0.5f;

        tile.transform.position = GridToWorld(coords);
        //Debug.Log(tile.name + " at: " + tile.transform.position);
        tile.transform.parent = transform;

        if (!tile.activeSelf)
        {
            tile.SetActive(true);
        }
    }

    public Vector3 GridToWorld(IntVector2 coords)
    {
        return new Vector3(transform.position.x + ((coords.x + gridOffset.x) * cellScale.x), transform.position.y + ((coords.y + gridOffset.y) * cellScale.y), 0f);
    }

    public IntVector2 WorldToGridPos(Vector3 pos)
    {
        float xOffset = 0;
        float yOffset = 0;

        if (size.y % 2 == 0)
            yOffset += 0.5f;

        if (size.x % 2 == 0)
            xOffset += 0.5f;
        //Debug.Log(xOffset + " " + yOffset);
        Vector3 posCell00 = GridToWorld(IntVector2.Zero);// - new Vector3(0.25f, 0.25f, 0);
        return new IntVector2(Mathf.Abs((int)(((pos - posCell00).x + xOffset) / cellScale.x)), Mathf.Abs((int)(((pos - posCell00).z + yOffset) / cellScale.y)));
    }

    /// <summary>
    /// Outs a list of neighboring cells.
    /// </summary>
    /// <param name="x">Grid x coordinate of the cell.</param>
    /// <param name="y">Grid y coordinate of the cell</param>
    /// <param name="neighbors">List to out to.</param>
    public void FindNeighbors(IntVector2 gridPos, out List<Cell> neighbors)
    {
        int x = gridPos.x;
        int y = gridPos.y;
        neighbors = new List<Cell>();
        int xStart = x - 1, xEnd = x + 1;
        int yStart = y - 1, yEnd = y + 1;

        for (int i = xStart; i <= xEnd; i++)
        {
            for (int j = yStart; j <= yEnd; j++)
            {
                if (i < 0 || j < 0 || i > size.x - 1 || j > size.y - 1)
                    break;

                neighbors.Add(Grid[i, j]);
            }
        }
    }

    public bool isValidPosition(IntVector2 pos)
    {
        return pos.isInRange(IntVector2.Zero, size);
    }

    protected bool validateGrid(IntVector2 start)
    {
        //Debug.Log("[GridManager] ValidateGrid() -- " + start);
        HashSet<IntVector2> visited = new HashSet<IntVector2>();
        Stack<IntVector2> stack = new Stack<IntVector2>();
        List<Cell> neighbors = new List<Cell>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            IntVector2 current = stack.Pop();
            if (Grid[current.x, current.y].thisCellType == Defines.CellTypes.Bulb) return true;     // If a bulb is found, return.
            if (!visited.Add(current)) continue;

            Cell cell = Grid[current.x, current.y];
            FindEgdeNeighbors(cell.coordinates, out neighbors);

            if (neighbors.Count > 0)
            {
                for (int i = 0; i < neighbors.Count; i++)
                {
                    if (IsCellConnected(cell, neighbors[i]) && !visited.Contains(neighbors[i].coordinates))
                    {
                        stack.Push(neighbors[i].coordinates);
                        neighbors[i].SetNewColor(1);
                    }
                    //visited.Add(neighbors[i].coordinates);
                }
            }
        }
        return false;
    }

    public void FindEgdeNeighbors(IntVector2 gridPos, out List<Cell> neighbors)
    {
        int x = gridPos.x;
        int y = gridPos.y;
        neighbors = new List<Cell>();
        int xStart = x - 1, xEnd = x + 1;
        int yStart = y - 1, yEnd = y + 1;

        //Edge neighbors in row.
        for (int i = xStart; i <= xEnd; i++)
        {
            if (i != x)        //Exclude the curr tile
            {
                if (i < 0 || i > size.x - 1)
                    continue;
                neighbors.Add(Grid[i, y]);
            }
        }

        //Edge neighbors in cols.
        for (int i = yStart; i <= yEnd; i++)
        {
            if (i != y)
            {
                if (i < 0 || i > size.y - 1)
                    continue;
                neighbors.Add(Grid[x, i]);
            }
        }
    }

    public Cell GetNeighborCellInDirection(Cell current, Defines.Directions direction)
    {
        Vector2 targetDirection = Vector2.zero;
        switch (direction)
        {
            case Defines.Directions.North: targetDirection = Vector2.up; break;
            case Defines.Directions.South: targetDirection = Vector2.down; break;
            case Defines.Directions.West: targetDirection = Vector2.left; break;
            case Defines.Directions.East: targetDirection = Vector2.right; break;
        }
        Vector2 result = new Vector2(current.coordinates.x, current.coordinates.y) + targetDirection;

        return CellAt((int)result.x, (int)result.y);
    }

    //Callback for Generate button in inspector of GridManager. This is for level design where we can
    // create a grid and customize it.
    public void OnGenerateClicked(IntVector2[,] levelData)
    {
        CurrentLevelData = levelData;
        Awake();
        InitializeGrid();
        GenerateGrid(true);
    }

    public Cell CellAt(int x, int y)
    {
        return Grid[x, y];
    }

    public Cell CellAt(IntVector2 pos)
    {
        return Grid[pos.x, pos.y];
    }

    public void OnGameStart(IntVector2[,] levelData)
    {
        CurrentLevelData = levelData;
        /*if(Grid == null)
        {
            int childIndex = 0;
            Grid = new Cell[size.x, size.y];
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    
                    Grid[i, j] = transform.GetChild(childIndex).GetComponent<Cell>();
                    childIndex++;
                }
            }
        }*/

        InitializeGrid();
        GenerateGrid();
        sourceCells = new List<Cell>();
        sourceCells = GetCellsOfType(Defines.CellTypes.Source);
    }

    void OnCellClickedComplete()
    {
        //Debug.Log("[GridManager] OnCellClickedComplete()");
        List<bool> validationResult = new List<bool>();
        for(int i = 0;i < sourceCells.Count;i++) 
        {
            validationResult.Add(validateGrid(sourceCells[i].coordinates));
        }

        //Since we have results for all the source   
        Debug.Log("[GridManager] Result: " + validationResult.ToArray().ToString());
    }

    private void OnValidate()
    {
        if (size.x < 2)
            size.x = 2;

        if (size.y < 2)
            size.y = 2;

        if (cellScale.x < 1)
            cellScale.x = 1f;

        if (cellScale.y < 1)
            cellScale.y = 1f;
    }

    public List<Cell> GetCellsOfType(Defines.CellTypes targetType)
    {
        List<Cell> result = new List<Cell>();

        int rows = Grid.GetLength(0);
        int cols = Grid.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (Grid[i, j].thisCellType == targetType)
                {
                    result.Add(Grid[i, j]);
                }
            }
        }

        return result;
    }

    public bool IsCellConnected(Cell cell, Cell neighbor)
    {
        foreach(Slot slot in cell.slots)
        {
            Defines.Directions slotDirForNeighbor = Defines.directionCheckMatrix[slot.CurrentDirection];
            Slot slotToCheck = neighbor.GetSlotInDirection(slotDirForNeighbor);
            if (slotToCheck != null)
            {
                if(slot.IsActive && slotToCheck.IsActive)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
