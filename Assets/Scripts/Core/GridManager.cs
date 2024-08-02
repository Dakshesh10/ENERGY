using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    Cell[,] Grid;

    public IntVector2 size;

    private Vector2 gridOffset;

    public Vector2 cellScale;

    public Dictionary<Defines.CellTypes, GameObject> cellTypeDictionary;

    private void OnEnable()
    {
        //Creating singleton instance.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Awake()
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

    void Start()
    {
        
    }

    public void InitializeSimulation()
    {
        
        InitializeGrid();
        GenerateGrid();
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

    void GenerateGrid()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                SpawnTile(new IntVector2(i, j), getRandomCellType());
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
    /// <param name="x">current row to spawn on</param>
    /// <param name="y">curreny column to spawn on</param>
    void SpawnTile(IntVector2 coords, Defines.CellTypes cellType, Quaternion startRotation = default(Quaternion))
    {
        if (!coords.isInRange(IntVector2.Zero, size))
            return;

        GameObject tile = Instantiate(cellTypeDictionary[cellType]);

        if (startRotation != default(Quaternion))
            tile.transform.localRotation = startRotation;

        Cell cell = tile.GetComponent<Cell>();
        tile.name = string.Format("{0}({1},{2})", cellType.ToString(), coords.x , coords.y);
        Grid[coords.x, coords.y] = cell;
        cell.Initialize(coords);

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
    public void FindNeighbors(IntVector2 gridPos, bool includeSoftWall, out List<Cell> neighbors)
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

                //if(Grid[i,j].thisCellType == Defines.CellTypes.Grass || (includeSoftWall && Grid[i, j].thisCellType == Defines.CellTypes.SoftWall)) 
                neighbors.Add(Grid[i, j]);
            }
        }
    }

    // TODO: Flood-fill type algo which will validate state of each cell recursively.
    protected void validateGrid()
    {
        List<IntVector2> queue = new List<IntVector2>();

        queue.Add(new IntVector2(0, 0));

        while (queue.Count > 0)
        {
            IntVector2 currCell = queue[queue.Count - 1];
            queue.RemoveAt(queue.Count - 1);

            int posX = currCell.x;
            int posY = currCell.y;

            /*if (isValid(screen, m, n, posX + 1, posY, prevC, newC))
            {
                // Color with newC
                // if valid and enqueue
                screen[posX + 1, posY] = newC;
                queue.Add(new IntVector2(posX + 1, posY));
            }

            if (isValid(screen, m, n, posX - 1, posY, prevC, newC))
            {
                screen[posX - 1, posY] = newC;
                queue.Add(new IntVector2(posX - 1, posY));
            }

            if (isValid(screen, m, n, posX, posY + 1, prevC, newC))
            {
                screen[posX, posY + 1] = newC;
                queue.Add(new IntVector2(posX, posY + 1));
            }

            if (isValid(screen, m, n, posX, posY - 1, prevC, newC))
            {
                screen[posX, posY - 1] = newC;
                queue.Add(new IntVector2(posX, posY - 1));
            }*/
        }
    }

    public void FindEgdeNeighbors(IntVector2 gridPos, bool includeSoftWall, out List<Cell> neighbors)
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
                neighbors.Add(Grid[i, y]);
            }
        }

        //Edge neighbors in cols.
        for (int i = yStart; i <= yEnd; i++)
        {
            if (i != y)
            {
                neighbors.Add(Grid[x, i]);
            }
        }
    }

    public void OnGenerateClicked()
    {
        Awake();
        InitializeGrid();
        GenerateGrid();
    }

    public Cell CellAt(int x, int y)
    {
        return Grid[x, y];
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
}
