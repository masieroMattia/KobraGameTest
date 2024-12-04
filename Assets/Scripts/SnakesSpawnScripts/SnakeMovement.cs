using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class SnakeMovement : MonoBehaviour
{
    #region Private Variables
    private int rowStart;
    private int colStart;
    private int ray;
    private float margin;
    #endregion

    #region Cycle Life
    void Update()
    {
        RandomMovement();
        KeepInArea();
    }
    #endregion

    public void SetArea(int rowStartTile, int colStartTile, int rayArea, float tilesMargin)
    {
        rowStart = rowStartTile;
        colStart = colStartTile;
        ray = rayArea;
        margin = tilesMargin;
    }

    #region Private Methods
    private void RandomMovement()
    {

    }

    private void KeepInArea()
    {
        float rowOffset = rowStart * margin;
        float colOffset = colStart * margin;

        Vector3 position = transform.localPosition;
        position.x = Mathf.Clamp(position.x, rowStart + rowOffset, ray + (ray * margin));
        position.z = Mathf.Clamp(position.z, colStart + margin, ray + (ray * margin));

        transform.localPosition = position;
    }
    #endregion
}
