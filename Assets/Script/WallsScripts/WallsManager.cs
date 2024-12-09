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
    public int rowWallPosition;
    public int colWallPosition;
    public int rowLength;
    public int colLength;
    #endregion





}