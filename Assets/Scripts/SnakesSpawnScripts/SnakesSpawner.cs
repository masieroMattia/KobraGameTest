using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SnakesSpawner : MonoBehaviour
{
    #region Public Variables
    public SnakesManager snakesManager;
    public GameObject snakePrefab;
    public Transform snakes;
    public MapGeneration map;
    #endregion

    #region Cycle Life
    void Start()
    {
        SpawnSnakes();
    }

    #endregion

    public void SpawnSnakes()
    {
        // Cycle for snakes spawn
        foreach (SnakesSpawnArea area in snakesManager.areas)
        {

            Vector3 spawnPoint = GetRandomPositionInArea(area);
            GameObject snakeObj = Instantiate(snakePrefab, snakes);
            snakeObj.transform.localPosition = spawnPoint;
            
            SnakeMovement snakeMovement = snakeObj.GetComponent<SnakeMovement>();
            snakeMovement.SetArea(area.rowStartTile, area.colStartTile, area.rayArea, map.margin);

        }
    }

    public Vector3 GetRandomPositionInArea(SnakesSpawnArea area)
    {
        int x = area.rowStartTile;
        int z = area.colStartTile;
        int ray = area.rayArea;


        int randomRow = Random.Range(x, x + ray);
        int randomCol = Random.Range(z, z + ray);
        return new Vector3(randomRow + (randomRow * map.margin), 0.0f, randomCol + (randomRow * map.margin));
    }
}
