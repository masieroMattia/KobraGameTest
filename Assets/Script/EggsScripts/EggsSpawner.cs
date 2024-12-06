using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SnakesSpawner : MonoBehaviour
{
    #region Public Variables
    public EggsManager eggsManager;
    public GameObject eggPrefab;
    public Transform foods;
    public LevelGrid grid;
    #endregion

    #region Private Variables
    EggsSpawnArea spawnArea;
    #endregion

    public void SpawnEggs()
    {
        Vector3 spawnPoint = GetRandomPositionInArea(spawnArea);
        GameObject eggObj = Instantiate(eggPrefab, foods);
        eggObj.transform.localPosition = spawnPoint;            
    }

    public Vector3 GetRandomPositionInArea(EggsSpawnArea area)
    {
        int x = area.rowStartTile;
        int z = area.colStartTile;
        int ray = area.rayArea;


        int randomRow = Random.Range(x, x + ray);
        int randomCol = Random.Range(z, z + ray);
        return new Vector3(randomRow, 0.0f, randomCol);
    }
}
