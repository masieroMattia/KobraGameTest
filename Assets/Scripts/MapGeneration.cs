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
    [Range(0.1f, 1.0f)]
    public float wallWidth;
    [Range(1.0f, 10.0f)]
    public float wallHeight;

    public float rowsOffset;
    public float colsOffset;

    public GameObject tilePrefab;
    public GameObject wallPrefab;
    public Transform map;
    #endregion

    #region Private Methods
    private Vector3 position;
    private float tileSize = 1.0f;
    #endregion
    void Awake()
    {
        PrintMap(position);
    }

    #region Public Methods
    public void PrintMap (Vector3 position)
    {
        float gridHeight = rows * (tileSize + margin);
        float gridWidth = cols * (tileSize + margin);

        Vector3[] wallPositions = new Vector3[]
        {
            new Vector3((gridWidth / 2) - (rowsOffset + tileSize / 2), 0.5f, - tileSize - colsOffset),
            new Vector3((gridWidth / 2) - (rowsOffset + tileSize / 2), 0.5f, gridHeight - colsOffset),
            new Vector3(- tileSize - rowsOffset, 0.5f, (gridHeight / 2) - (colsOffset + tileSize / 2)),
            new Vector3(gridWidth - rowsOffset, 0.5f, (gridHeight / 2) - (colsOffset + tileSize / 2))
        };

        Vector3[] wallDimension = new Vector3[]
        {
            new Vector3(gridWidth + tileSize, wallHeight, wallWidth),
            new Vector3(gridWidth + tileSize, wallHeight, wallWidth),
            new Vector3(wallWidth, wallHeight, gridHeight + tileSize),
            new Vector3(wallWidth, wallHeight, gridHeight + tileSize)
        };

        for (int i = 0; i < wallPositions.Length; i++)
        {
            wallPrefab.transform.localPosition = wallPositions[i];
            wallPrefab.transform.localScale = wallDimension[i];
            Instantiate(wallPrefab, map);

        }

        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < cols; z++)
            {
                position = new Vector3 (z * (tileSize + margin) - rowsOffset, 0, x * (tileSize + margin) - colsOffset);
                tilePrefab.transform.localPosition = position;
                Instantiate(tilePrefab, map);

            }
        }
    }
    #endregion
}