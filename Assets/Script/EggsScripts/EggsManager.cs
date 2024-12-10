using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[CreateAssetMenu(fileName = "EggsManager", menuName = "ScriptableObjectsEggsManager", order = 2)] // Add the field for select the Scriptable Object (when adding a new asset)
public class EggsManager : ScriptableObject
{
    #region Public Variables
    public EggsSpawnArea[] areas; // Array of eggs spawn area 
    #endregion
}

[System.Serializable] // Makes the class Serializable
public class EggsSpawnArea
{
    // Variables for eggs spawn area configuration
    #region Public Variables
    public int rowStartTile;
    public int colStartTile;
    public int rayArea;
    #endregion
}