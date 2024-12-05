using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MapGeneration : MonoBehaviour
{
    #region Public Variables
    [Min(10)]
    public int rows; // X grid dimension
    [Min(10)]
    public int cols; // Z grid dimension

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

    // Property to get the value of tileSize
    public int TileSize {  get { return tileSize; } }

    void Awake()
    {
        grid = new Vector3[rows, cols];

        // Methods calls
        PrintMap();
        GenerateWalls();
    }

    #region Public Methods
    public void PrintMap ()
    {
        // cycle for grid creation
        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < cols; z++)
            {
                position = new Vector3 (z * tileSize, 0, x * tileSize);
                tilePrefab.transform.localPosition = position;
                Instantiate(tilePrefab, tiles);

                grid[x, z] = position;
            }
        }

    }

    public void GenerateWalls()
    {
        // Cycle for set the position and length of every wall of the array
        foreach (WallsPositionAndLength wall in wallsManager.positions)
        {
            float x = wall.rowWallPosition;
            float z = wall.colWallPosition;

            Vector3 position = new Vector3(x, 0, z);
            GameObject wallObj = Instantiate(wallPrefab, walls);
            wallObj.transform.localPosition = position;

            wallObj.transform.localScale = new Vector3(wall.rowLength, wallObj.transform.localScale.y, wall.colLength);
        }
    }

    // Method that convert world space to grid space
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x);
        int z = Mathf.FloorToInt(worldPosition.z);
        return new Vector2Int(x, z);
    }

    // Method that convert grid space to world space
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

    // Methods passing the size of the grid

    public int GetWidthSize ()
    {
        return cols * tileSize;
    }

    public int GetHeightSize()
    {
        return rows * tileSize;
    }
    #endregion

    #region Private Methods

    #endregion
}
