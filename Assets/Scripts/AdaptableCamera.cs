using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class AdaptableCamera : MonoBehaviour
{
    #region Public Variables
    public Camera mainCamera;
    public MapGeneration mapGeneration;
    #endregion

    #region Private Variables
    private float widthSize;
    private float heightSize;
    private float tileSize;
    #endregion
    void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Start()
    {
        widthSize = mapGeneration.GetWidthSize();
        heightSize = mapGeneration.GetHeightSize();
        tileSize = mapGeneration.TileSize;

        AdaptCamera();
    }

    public void AdaptCamera ()
    {
        Vector3 gridCenter = new Vector3(widthSize / 2, 10.0f, (heightSize / 2) - (float)tileSize);

        mainCamera.transform.position = gridCenter;
        
        float aspectRatio = (float)Screen.width / Screen.height;
        float orthographicSize = Mathf.Max(heightSize / 2 + tileSize, (widthSize / (2 * aspectRatio)) + tileSize);
        mainCamera.orthographicSize = orthographicSize;
    }
}
