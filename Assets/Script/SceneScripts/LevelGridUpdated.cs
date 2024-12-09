using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class LevelGridUpdated : MonoBehaviour
{
    #region Public variables
    [Header("Grid Settings")]
    [SerializeField] public int sizeXGrid; // Grid width
    [SerializeField] public int sizeZGrid; // Grid height
    [SerializeField] private GameObject gridPrefab; // Grid(Cell) Prefab
    [SerializeField] private GameObject wallPrefab; // Wall Prefab
    [SerializeField] private Transform gridParent;
    [SerializeField] private Transform wallsParent;
    [SerializeField] private WallsManager walls;

    [Header("Color Grid Settings")]
    [SerializeField] private Color cubeColor = Color.white;   // Cube Color prefab
    [SerializeField] private Color borderColor = Color.black;  // frame(cornice) Color prefab

    #endregion

    #region Private variables
    private Vector3Int eggPosition;
    private MyTime myTime = null;
    private ColorManager colorManager;
    private UnityAction methodList;
    private GameObject[,] grids;
    private List<Transform> spawnedItems = new List<Transform>();
    private GameObject ParentFoodObject;
    public int spacingGrid = 1;

    // Array to track if a grid position is occupied
    private bool[,] gridOccupied;

    #endregion

    #region Lifecycle
    void Awake()
    {
        // Instantiate the time
        myTime = new MyTime();

        // Instantiate the color manager
        colorManager = new ColorManager();

        // Verify the presence of the square prefab for the grid creation
        if (gridPrefab == null)
        {
            Debug.LogError("squarePrefab is not assigned!");
            return;
        }

        // Array size set with height and width of the grid
        grids = new GameObject[sizeXGrid, sizeZGrid];
        gridOccupied = new bool[sizeXGrid, sizeZGrid]; // Initialize gridOccupied

        // Grid creations
        SetGrid();
        // Walls generation
        GenerateWalls();
    }
    #endregion

    #region Private methods
    private void SetGrid() // Method that creates the grid
    {
        // Get the width and height of the cube prefab
        float prefabWidth = gridPrefab.GetComponent<MeshRenderer>().bounds.size.x;
        float prefabHeight = gridPrefab.GetComponent<MeshRenderer>().bounds.size.z;

        // Adjust the spacing based on the larger dimension of the prefab
        spacingGrid += Mathf.CeilToInt(Mathf.Max(prefabWidth, prefabHeight));
        for (int x = 0; x < sizeXGrid; x++)
        {
            for (int z = 0; z < sizeZGrid; z++)
            {
                // Use Vector3Int for grid positions
                Vector3Int spawnPosition = new Vector3Int(x * spacingGrid, 0, z * spacingGrid);

                // Create a gameObject with the prefab inside
                GameObject newSquare = Instantiate(gridPrefab, spawnPosition, Quaternion.identity, gridParent);

                // Setting color prefab for the frame and cube
                colorManager.ApplyColorsSpecificChildPrefab(newSquare, "Border", borderColor);
                colorManager.ApplyColorsSpecificChildPrefab(newSquare, "Cube", cubeColor);

                // Adding the new gameObject with the prefab inside the array position of the two for
                grids[x, z] = newSquare;
                gridOccupied[x, z] = false; // Mark this grid cell as unoccupied
            }
        }
        Debug.Log($"Grid created with {sizeXGrid}x{sizeZGrid} and spacing: {spacingGrid}");
    }

    // Spawn item method that checks for grid occupation and walls
    public GameObject SpawnItemOnTheGrid(GameObject itemPrefab, GameObject parentObject)
    {
        if (itemPrefab == null || parentObject == null)
        {
            Debug.LogError("Prefab or Parent Object is null in SpawnItemOnTheGrid.");
            return null;
        }

        // Pick random grid positions using only integer values
        int randomX = Random.Range(0, sizeXGrid - 1);
        int randomZ = Random.Range(0, sizeZGrid - 1);


        // Try to find an empty spot on the grid, avoiding walls
        Vector3 spawnPosition = Vector3.zero;
        bool foundEmptySpot = false;

        for (int x = 0; x < sizeXGrid; x++)
        {
            for (int z = 0; z < sizeZGrid; z++)
            {
                // Check if the grid is not occupied by an object or a wall
                if (!gridOccupied[randomX, randomZ] && !IsPositionOccupiedByWall(randomX, randomZ))
                {
                    spawnPosition = grids[randomX, randomZ].transform.position;
                    foundEmptySpot = true;
                    break;
                }
            }
            if (foundEmptySpot)
                break;
        }
        // If no empty spot was found, return null (or handle it as needed)
        if (!foundEmptySpot)
        {
            Debug.LogWarning("No empty spot found to spawn the item.");
            return null;
        }

        // Instantiate the item at the spawn position
        GameObject itemGameObject = Instantiate(itemPrefab);
        itemGameObject.transform.position = spawnPosition;
        itemGameObject.transform.SetParent(parentObject.transform);

        // Mark the grid position as occupied
        //Vector3Int gridPosition = Vector3Int.FloorToInt(itemGameObject.transform.position);
        gridOccupied[randomX, randomZ] = true;

        spawnedItems.Add(itemGameObject.transform);
        return itemGameObject;
    }

    // Method to check if a position is occupied by a wall
    private bool IsPositionOccupiedByWall(int x, int z)
    {
        Vector3Int positionToCheck = new Vector3Int(x, 0, z);

        // Loop through all the walls to check if any of them occupy the position (or part of it)
        foreach (var wall in walls.positions)
        {
            for (int i = 0; i < wall.rowLength; i++) // Check rows
            {
                for (int j = 0; j < wall.colLength; j++) // Check columns
                {
                    if (positionToCheck.x == wall.rowWallPosition + i && positionToCheck.z == wall.colWallPosition + j)
                    {
                        return true; // The position is occupied by the wall
                    }
                }
            }
        }
        return false;
    }

    // Spawn item at a specific position
    // Inside SpawnItemOnGridPosition method
    public GameObject SpawnItemOnGridPosition(GameObject itemPrefab, GameObject parentObject, int x, int z)
    {
        // Check if the position is occupied by an object or a wall
        if (gridOccupied[x, z] || IsPositionOccupiedByWall(x, z))
        {
            Debug.LogError("The specified position is already occupied.");
            return null;
        }

        // Instantiate the item at the spawn position
        GameObject itemGameObject = Instantiate(itemPrefab);
        itemGameObject.transform.position = new Vector3(x * spacingGrid, 0, z * spacingGrid);
        itemGameObject.transform.SetParent(parentObject.transform);

        // Mark the grid position as occupied
        gridOccupied[x, z] = true; // Mark as occupied

        spawnedItems.Add(itemGameObject.transform);
        return itemGameObject;
    }

    // Make sure to update the gridOccupied array when an item is removed or destroyed.
    public void RemoveItemFromGrid(GameObject itemGameObject)
    {
        Vector3Int gridPosition = Vector3Int.FloorToInt(itemGameObject.transform.position);
        gridOccupied[gridPosition.x, gridPosition.z] = false; // Mark as unoccupied
    }


    private void GenerateWalls()
    {
        // Generate walls based on the positions provided in the WallsManager
        foreach (WallsPositionAndLength wall in walls.positions)
        {
            // Loop through all the cells the wall occupies
            for (int i = 0; i < wall.rowLength; i++)
            {
                for (int j = 0; j < wall.colLength; j++)
                {
                    // Calculate wall position based on the grid size and the wall's position and length
                    int xPos = wall.rowWallPosition + i;
                    int zPos = wall.colWallPosition + j;

                    // Check if the position is within bounds
                    if (xPos >= 0 && xPos < sizeXGrid && zPos >= 0 && zPos < sizeZGrid)
                    {
                        // Mark the grid cell as occupied by a wall
                        gridOccupied[xPos, zPos] = true;

                        // Instantiate the wall prefab at the calculated position
                        Vector3 wallPosition = new Vector3(xPos * spacingGrid, 0, zPos * spacingGrid);
                        GameObject wallObj = Instantiate(wallPrefab, wallsParent);
                        wallObj.transform.localPosition = wallPosition;
                    }
                    else
                    {
                        // Optionally, you can log if a wall extends outside of the grid bounds
                        Debug.LogWarning($"Wall at position ({xPos}, {zPos}) is outside the grid bounds.");
                    }
                }
            }
        }
    }

    #endregion
}
