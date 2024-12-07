using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[CreateAssetMenu(fileName = "WallsManager", menuName = "ScriptableObjects/WallsManager", order = 1)]
public class WallsManager : ScriptableObject
{
    public WallsPositionAndLength[] positions = new WallsPositionAndLength[0];
}

[System.Serializable]
public class WallsPositionAndLength
{
    public int rowWallPosition;
    public int colWallPosition;
    public int rowLength;
    public int colLength;
}
