using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class LevelGrid : MonoBehaviour
{
    #region Public variables
    [Header("Grid Settings")]
    [SerializeField] public int sizeXGrid; // Grid width
    [SerializeField] public int sizeZGrid; // Grid heigth
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
    private UnityAction methodList;
    private GameObject[,] grids;
    private List<Transform> spawnedItems = new List<Transform>();
    private GameObject ParentFoodObject;
    private int spacingGrid = 1;

    #endregion

    #region Lifecycle
    void Awake()
    {
        // Instantiate the time
        myTime = new MyTime();

        // Verify the presence of the square prefab for the grid creation
        if (gridPrefab == null)
        {
            Debug.LogError("squarePrefab is not assigned!");
            return;
        }

        // Array size set with heigth and width of the grid 
        grids = new GameObject[sizeXGrid, sizeZGrid];

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
                GameObject newSquare = Instantiate(gridPrefab);
                // Giving the random position to the new gameObject with the prefab inside
                newSquare.transform.position = spawnPosition;
                // Setting the parent of the new gameObject with the prefab inside
                newSquare.transform.SetParent(gridParent);
                //Setting color prefab for the frame and cube
                ApplyColorsChildPrefab(newSquare, "Border", borderColor);
                ApplyColorsChildPrefab(newSquare, "Cube", cubeColor);
                // Adding the new gameObject with the prefab inside the array position of the two for
                grids[x, z] = newSquare;
            }
        }
        Debug.Log($"Grid created with {sizeXGrid}x{sizeZGrid} and spacing: {spacingGrid}");
    }

    public GameObject SpawnItemOnTheGrid(GameObject itemPrefab, GameObject parentObject)
    {
        // Pick random grid positions using only integer values
        int randomX = Random.Range(0, sizeXGrid - 1);
        int randomZ = Random.Range(0, sizeZGrid - 1);

        Vector3 spawnPosition = grids[randomX, randomZ].transform.position;




        // Instantiate the item at the spawn position
        GameObject itemGameObject = Instantiate(itemPrefab);
        itemGameObject.transform.position = spawnPosition;
        itemGameObject.transform.SetParent(parentObject.transform);

        // Check for overlap with existing items on the grid
        if (!SpawnOverlap(spawnedItems, itemGameObject))
        {
            spawnedItems.Add(itemGameObject.transform);
            return itemGameObject;
        }
        else
        {
            // If overlap is found, destroy the item and try again
            Destroy(itemGameObject.gameObject);
            return SpawnItemOnTheGrid(itemPrefab, parentObject);
        }
    }

    private bool SpawnOverlap(List<Transform> spawnedItems, GameObject itemGameObject)
    {
        bool positionOccupied = false;

        foreach (Transform existingItem in this.spawnedItems)
        {
            // Compare the positions using Vector3Int to avoid fractional differences
            Vector3Int existingPosition = Vector3Int.FloorToInt(existingItem.position);
            Vector3Int newPosition = Vector3Int.FloorToInt(itemGameObject.transform.position);

            if (existingPosition == newPosition)
            {
                positionOccupied = true;
                break;
            }
        }

        return positionOccupied;
    }
    public void ApplyColorsChildPrefab(GameObject parent, string childName, Color color)
    {
        Transform child = parent.transform.Find(childName);
        if (child == null)
        {
            Debug.LogError($"Child '{childName}' not found in prefab '{parent.name}'");
            return;
        }

        Renderer renderer = child.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError($"Renderer not found on child '{childName}' in prefab '{parent.name}'");
            return;
        }

        renderer.material.color = color;
    }
    public void ApplyColorsChild(GameObject parent, Color color)
    {
        if (parent == null)
        {
            Debug.LogError($"Parent '{parent}' not found");
            return;
        }

        Renderer renderer = parent.GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            Debug.LogError($"Renderer not found on child in parent '{parent}'");
            return;
        }

        renderer.material.color = color;
    }

    private void GenerateWalls()
    {
        // Cycle for set the position and length of every wall of the array
        foreach (WallsPositionAndLength wall in walls.positions)
        {
            float x = wall.rowWallPosition;
            float z = wall.colWallPosition;

            Vector3 position = new Vector3(x, 0, z);
            GameObject wallObj = Instantiate(wallPrefab, wallsParent);
            wallObj.transform.localPosition = position;

            wallObj.transform.localScale = new Vector3(wall.rowLength, wallObj.transform.localScale.y, wall.colLength);
        }
    }
    #endregion
}
