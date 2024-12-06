using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[CreateAssetMenu(fileName = "SnakesManager", menuName = "ScriptableObjectsSnakesManager", order = 2)] // Add the field for select the Scriptable Object (when adding a new asset)
public class SnakesManager : ScriptableObject
{
    #region Public Variables
    public SnakesSpawnArea[] areas; // Array of snakes spawn area 
    #endregion
}

[System.Serializable] // Makes the class Serializable
public class SnakesSpawnArea
{
    // Variables for snakes spawn area configuration
    #region Public Variables
    public int rowStartTile;
    public int colStartTile;
    public int rayArea;
    #endregion
}