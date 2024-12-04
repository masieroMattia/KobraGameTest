using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WallsManager))]
public class WallsPositionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WallsManager wallPositions = (WallsManager)target;
        if (GUILayout.Button("Add Position"))
            ArrayUtility.Add(ref wallPositions.positions, new WallsPositionAndLength());
        if (GUILayout.Button("Clear Positions"))
            wallPositions.positions = new WallsPositionAndLength[0];

        EditorGUILayout.Space();

        for (int i = 0; i < wallPositions.positions.Length; i++)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.Space();

            wallPositions.positions[i].rowWallPosition = EditorGUILayout.FloatField("X", wallPositions.positions[i].rowWallPosition);
            wallPositions.positions[i].colWallPosition = EditorGUILayout.FloatField("Z", wallPositions.positions[i].colWallPosition);
            wallPositions.positions[i].rowLength = EditorGUILayout.IntField("X Length", wallPositions.positions[i].rowLength);
            wallPositions.positions[i].colLength = EditorGUILayout.IntField("Z Length", wallPositions.positions[i].colLength);

            if (GUILayout.Button("Remove"))
                ArrayUtility.RemoveAt(ref wallPositions.positions, i);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical(); 
            EditorGUILayout.Space();
        }
        EditorUtility.SetDirty(target);
    }
}
