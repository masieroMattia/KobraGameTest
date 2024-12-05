using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SnakeMovement : MonoBehaviour
{
    public float speed = 1.0f;

    #region Private Variables
    private int rowStart;
    private int colStart;
    private int ray;
    
    #endregion

    #region Cycle Life

    private void Start()
    {
    
    }

    void Update()
    {
        RandomMovement();
        KeepInArea();
    }
    #endregion

    public void SetArea(int rowStartTile, int colStartTile, int rayArea)
    {
        rowStart = rowStartTile;
        colStart = colStartTile;
        ray = rayArea;
    }

    #region Private Methods
    private void RandomMovement()
    {
        transform.localPosition += Vector3.forward * speed * Time.deltaTime;

    }

    private void KeepInArea()
    {

        Vector3 position = transform.localPosition;

        position.x = Mathf.Clamp(position.x, rowStart, rowStart + ray);
        position.z = Mathf.Clamp(position.z, colStart, colStart + ray);

        transform.localPosition = position;

        Debug.Log(position);

    }
    #endregion
}
