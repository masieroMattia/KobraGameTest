using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SnakesSpawner : MonoBehaviour
{
    #region Public Variables
    public SnakesManager snakesManager;
    public GameObject snakeHeadPrefab;
    public GameObject snakeTailPrefab;
    public Transform snakes;
    public LevelGrid grid;
    public int initialStartTailSize = 2; // Initial size of the tail
    private List<GameObject> snakeList = new List<GameObject>(); // Initialize list here
    [SerializeField] private Vector3Int tailOffset = new Vector3Int(0, 0, -1);
    private Vector3Int currentDirection = Vector3Int.forward;
    private Vector3Int nextDirection = Vector3Int.zero;
    public LayerMask obstacleLayer; // Layer degli ostacoli
    private bool isRaycastHit = false; // Track if ray hits an obstacle

    #endregion

    #region Cycle Life
    void Start()
    {
        SpawnSnakes();
    }
    #endregion
    private void Update()
    {
        
    }
    private void HandleDirection()
    {
        if (currentDirection == Vector3Int.forward)
        {
            nextDirection = Vector3Int.right;
        }
        else if (currentDirection == Vector3Int.right)
        {
            nextDirection = Vector3Int.back; // Cambia la logica per ottenere una direzione diversa da destra
        }
        else if (currentDirection == Vector3Int.back)
        {
            nextDirection = Vector3Int.left;
        }
        else if (currentDirection == Vector3Int.left)
        {
            nextDirection = Vector3Int.forward;
        }
    }

    public void SpawnSnakes()
    {
        // Cycle for snakes spawn
        foreach (SnakesSpawnArea area in snakesManager.areas)
        {
            Vector3 spawnPoint = GetRandomPositionInArea(area);
            GameObject snakeHeadGameObject = Instantiate(snakeHeadPrefab, snakes);
            snakeHeadGameObject.transform.localPosition = spawnPoint;
            snakeList.Add(snakeHeadGameObject);
            SnakeMovement snakeMovement = snakeHeadGameObject.GetComponent<SnakeMovement>();
            snakeMovement.SetArea(area.rowStartTile, area.colStartTile, 3);

            // Position the tail behind the head and chain them
            GameObject previousSegment = snakeHeadGameObject;  // Start with the head as the previous segment
            for (int i = 1; i <= initialStartTailSize; i++)
            {
                GameObject snakeTailGameObject = Instantiate(snakeTailPrefab, snakeHeadGameObject.transform);
                snakeList.Add(snakeTailGameObject);
                if(i > 0)
                {
                    Vector3Int tailPosition = Vector3Int.RoundToInt(snakeList[i - 1].transform.position + tailOffset);
                    snakeList[i].transform.position = tailPosition;
                }
                Debug.Log($"tail number {i} is positioned  {snakeList[i].transform.position}");
                Debug.Log("---------------------------------------------------------------");
            }


        }
    }

    private void PositionSnakeTail()
    {
        for (int i = snakeList.Count - 1; i > 0; i--)
        {
            Vector3Int tailPosition = Vector3Int.RoundToInt(snakeList[i - 1].transform.position + tailOffset);
            snakeList[i].transform.localPosition = tailPosition;
            Debug.Log($"tail number {i} is positioned  {snakeList[i].transform.position}");

        }

    }
    
    public Vector3 GetRandomPositionInArea(SnakesSpawnArea area)
    {
        int maxGridSize = 100;
        int minGridSize = 0;

        // Clamp values to avoid exceeding grid bounds
        int x = Mathf.Clamp(Mathf.RoundToInt(area.rowStartTile), minGridSize, grid.sizeXGrid - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt(area.colStartTile), minGridSize, grid.sizeZGrid - 1);
        int ray = Mathf.Clamp(Mathf.RoundToInt(area.rayArea), minGridSize, grid.sizeZGrid - 1);

        area.rowStartTile = x;
        area.colStartTile = z;
        area.rayArea = ray;

        int randomRow = Mathf.Clamp(Random.Range(x, x + ray), minGridSize, grid.sizeXGrid - 1);
        int randomCol = Mathf.Clamp(Random.Range(z, z + ray), minGridSize, grid.sizeZGrid - 1);

        return new Vector3(randomRow * grid.spacingGrid, 0.0f, randomCol * grid.spacingGrid);
    }
}
