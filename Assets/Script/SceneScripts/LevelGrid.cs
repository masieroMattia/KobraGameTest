using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    private ColorManager colorManager;
    private UnityAction methodList;
    private GameObject[,] grids;
    private List<Transform> spawnedItems = new List<Transform>();
    private GameObject ParentFoodObject;
    public int spacingGrid = 1;

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
                GameObject newSquare = Instantiate(gridPrefab, spawnPosition, Quaternion.identity, gridParent);

                //Setting color prefab for the frame and cube
                colorManager.ApplyColorsSpecificChildPrefab(newSquare, "Border", borderColor);
                colorManager.ApplyColorsSpecificChildPrefab(newSquare, "Cube", cubeColor);

                // Adding the new gameObject with the prefab inside the array position of the two for
                grids[x, z] = newSquare;
            }
        }
        Debug.Log($"Grid created with {sizeXGrid}x{sizeZGrid} and spacing: {spacingGrid}");
    }

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
    public GameObject SpawnItemOnGridPosition(GameObject itemPrefab, GameObject parentObject, int x, int z)
    {
        // Instantiate the item at the spawn position
        GameObject itemGameObject = Instantiate(itemPrefab);
        itemGameObject.transform.position = new Vector3Int(x, 0, z);
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
        if (itemGameObject == null)
        {
            Debug.LogError("Item GameObject is null.");
            return true;
        }
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

    private void GenerateWalls()
    {
        // Definire i limiti predefiniti per le posizioni e le lunghezze dei muri
        int defaultRowLength = 3;  // Lunghezza predefinita del muro nella direzione della riga
        int defaultColLength = 3;  // Lunghezza predefinita del muro nella direzione della colonna
        int maxGridSize = 100;     // Dimensione massima della griglia (aggiusta come necessario)
        int minGridSize = 0;       // Dimensione minima della griglia (aggiusta come necessario)

        // Ciclo per impostare la posizione e la lunghezza di ogni muro nell'array
        foreach (WallsPositionAndLength wall in walls.positions)
        {
            // Impostare i valori di lunghezza della riga e della colonna se sono invalidi o non impostati
            int rowLength = wall.rowLength > 0 ? wall.rowLength : defaultRowLength;
            int colLength = wall.colLength > 0 ? wall.colLength : defaultColLength;

            // Clampa i valori di posizione del muro per assicurarti che siano all'interno dei limiti della griglia
            int x = Mathf.Clamp(Mathf.RoundToInt(wall.rowWallPosition), minGridSize, sizeXGrid - 1);
            int z = Mathf.Clamp(Mathf.RoundToInt(wall.colWallPosition), minGridSize, sizeZGrid - 1);

            // Clampa le lunghezze dei muri per assicurarti che non superino le dimensioni della griglia
            rowLength = Mathf.Clamp(rowLength, 1, sizeXGrid - x);  // Assicurati che rowLength rientri nella larghezza della griglia
            colLength = Mathf.Clamp(colLength, 1, sizeZGrid - z);  // Assicurati che colLength rientri nell'altezza della griglia

            // Salva i valori clamped nell'array walls.positions
            wall.rowWallPosition = x;  // Posizione della riga
            wall.colWallPosition = z;  // Posizione della colonna
            wall.rowLength = rowLength;  // Lunghezza del muro nella direzione della riga
            wall.colLength = colLength;  // Lunghezza del muro nella direzione della colonna

            // Calcola la posizione corretta per il muro sulla griglia
            Vector3 position = new Vector3(x * spacingGrid, 0, z * spacingGrid);

            // Instanzia il prefab del muro nella posizione calcolata
            GameObject wallObj = Instantiate(wallPrefab, wallsParent);
            wallObj.transform.localPosition = position;

            // Imposta la scala del muro usando le lunghezze clamped (moltiplicate per spacingGrid)
            wallObj.transform.localScale = new Vector3(rowLength * spacingGrid, wallObj.transform.localScale.y, colLength * spacingGrid);
        }
    }
    #endregion
}