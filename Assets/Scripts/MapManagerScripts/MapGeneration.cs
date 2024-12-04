using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    #region Public Variables
    [Min(10)]
    public uint rows;
    [Min(10)]
    public uint cols;
    [Range(0.1f, 0.5f)]
    public float margin = 0.1f;

    public GameObject tilePrefab;
    public GameObject wallPrefab;
    public Transform tiles;
    public Transform walls;
    public WallsManager wallsManager;

    public static Vector3[,] grid;
    #endregion

    #region Private Variables
    private Vector3 position;
    private int tileSize = 1;
    #endregion

    public int TileSize {  get { return tileSize; } }

    void Awake()
    {
        grid = new Vector3[rows, cols];
        PrintMap();
        GenerateWalls();
    }

    #region Public Methods
    public void PrintMap ()
    {

        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < cols; z++)
            {
                position = new Vector3 (z * (tileSize + margin), 0, x * (tileSize + margin) );
                tilePrefab.transform.localPosition = position;
                Instantiate(tilePrefab, tiles);

                grid[x, z] = position;
            }
        }

    }

    public void GenerateWalls()
    {
        foreach (WallsPositionAndLength wall in wallsManager.positions)
        {
            float x = wall.rowWallPosition;
            float z = wall.colWallPosition;

            Vector3 position = new Vector3(x + (margin * x), 0, z + (margin * z));
            GameObject wallObj = Instantiate(wallPrefab, walls);
            wallObj.transform.localPosition = position;

            wallObj.transform.localScale = new Vector3(wall.rowLength, wallObj.transform.localScale.y, wall.colLength);
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x);
        int z = Mathf.FloorToInt(worldPosition.z);
        return new Vector2Int(x, z);
    }

    public Vector3 GridToWorld(int x, int y)
    {
        if (x >= 0 && x < rows && y >= 0 && y < cols)
            return grid[x, y];
        else
        {
            Debug.LogError("Grid coordinates out of bounds!");
            return Vector3.zero;
        }
    }
    #endregion

    public float GetWidthSize ()
    {
        return cols * (tileSize + margin);
    }

    public float GetHeightSize()
    {
        return rows * (tileSize + margin);
    }
}
