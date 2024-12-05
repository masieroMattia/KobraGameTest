using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KobraSpawnManager : MonoBehaviour
{
    private GameManager gameManager;
    private ColorManager colorManager;
    private MapGeneration map;
    private List<Transform> spawnedItems = new List<Transform>();

    void Start()
    {
        transform.position = Vector3.zero;
        map = FindObjectOfType<MapGeneration>(); // Initialize the map
        colorManager = FindObjectOfType<ColorManager>(); // Initialize the color manager
        if (map == null) 
            Debug.LogError("MapGeneration not found. Make sure it's in the scene.");
        if (colorManager == null) 
            Debug.LogError("ColorManager not found. Make sure it's in the scene.");
    }

    #region Methods
    public GameObject SpawnItemOnTheGrid(GameObject itemPrefab, GameObject parentObject)
    {
        // Pick random grid positions using only integer values
        int randomX = Random.Range(0, map.rows - 1);
        int randomZ = Random.Range(0, map.cols - 1);

        Vector3 spawnPosition = map.GridToWorld(randomX, randomZ);




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

    public void SpawnHead(List<Transform> kobraList, GameObject headKobraPrefab, Transform parentTransform, int numberOfHeadCube, Transform tailPrefab, Vector3Int tailOffset, Color headColor, Color tailColor, ColorManager colorManager)
    {
        if (map == null)
        {
            Debug.LogError("map is null.");
            return;
        }

        if (headKobraPrefab == null)
        {
            Debug.LogError("headKobraPrefab is null.");
            return;
        }

        GameObject kobraHead = SpawnItemOnTheGrid(headKobraPrefab, this.gameObject);
        colorManager.ApplyColorsChild(kobraHead, headColor);
        kobraList.Add(kobraHead.transform);

        for (int i = 1; i < numberOfHeadCube; i++)
        {
            Transform kobraTail = Instantiate(tailPrefab, this.transform);
            colorManager.ApplyColorsChild(kobraTail.gameObject, tailColor);
            kobraList.Add(kobraTail);

            if (i > 0)
            {
                kobraList[i].position = (Vector3Int.RoundToInt(kobraList[i - 1].position + tailOffset));

            }

        }
        //Transform child = transform.GetChild(0);
        Debug.Log("Head successfully spawned.");
    }
    #endregion
}
