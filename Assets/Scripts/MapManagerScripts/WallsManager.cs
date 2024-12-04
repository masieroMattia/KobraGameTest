using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[CreateAssetMenu(fileName = "WallsManager", menuName = "ScriptableObjectsWallsManager", order = 1)]
public class WallsManager : ScriptableObject
{
    #region Public Variables
    public WallsPositionAndLength[] positions;
    #endregion
}
