using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[CreateAssetMenu(fileName = "WallsManager", menuName = "ScriptableObjectsWallsManager", order = 1)] // Add the field for select the Scriptable Object (when adding a new asset)
public class WallsManager : ScriptableObject
{
    #region Public Variables
    public WallsPositionAndLength[] positions; // Array of wall positions and lengths
    #endregion
}

[System.Serializable] // Makes the class Serializable
public class WallsPositionAndLength
{
    // Variables for walls configuration
    #region Public Variables
    public float rowWallPosition;
    public float colWallPosition;
    public float rowLength;
    public float colLength;
    #endregion
}