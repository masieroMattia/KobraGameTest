using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class AdaptableCamera : MonoBehaviour
{
    #region Public Variables
    public Camera mainCamera;
    public MapGeneration map;
    #endregion

    #region Private Variables
    private float widthSize; // X size of the grid
    private float heightSize; // Z size of the grid
    private float tileSize; // Size of the tiles
    #endregion
    void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        // Pass the size of the grid and tiles from MapGeneration script
        widthSize = map.GetWidthSize();
        heightSize = map.GetHeightSize();
        tileSize = map.TileSize;

        // Method call
        AdaptCamera();
    }

    // Method for set camera position and orthographic size
    #region Private Metodhs
    private void AdaptCamera ()
    {
        // Center position of the grid
        Vector3 gridCenter = new Vector3(widthSize / 2, 10.0f, (heightSize / 2) - (tileSize / 2));

        mainCamera.transform.position = gridCenter;
        
        // Screen ratio
        float aspectRatio = (float)Screen.width / Screen.height;
        // Orthographic size of the camera depending on the grid size
        float orthographicSize = Mathf.Max(heightSize / 2 + tileSize, (widthSize / (2 * aspectRatio)) + tileSize);
        mainCamera.orthographicSize = orthographicSize;
    }
    #endregion
}
