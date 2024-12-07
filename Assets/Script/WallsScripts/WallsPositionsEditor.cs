using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WallsManager))]
public class WallsPositionEditor : Editor // Derived class from class Editor for Inspector creation
{
    // Override of a Editor class Method
    public override void OnInspectorGUI()
    {
        WallsManager wallPositions = (WallsManager)target; // Casting target (from Editor class) to the custom type WallsManager for free access to Editor properties and methods 
        
        // Track Undo
        Undo.RecordObject(wallPositions, "Modify WallsManager");

        if (GUILayout.Button("Add Position")) // Button to add a new position
            ArrayUtility.Add(ref wallPositions.positions, new WallsPositionAndLength()); // Add a new wall object to the position
        if (GUILayout.Button("Clear Positions")) // Button to clear the array
            wallPositions.positions = new WallsPositionAndLength[0]; // Reset the array

        EditorGUILayout.Space();

        // Iterate the walls array
        for (int i = 0; i < wallPositions.positions.Length; i++)
        {
            // Create the layout
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.Space();

            // Create the field for variables
            wallPositions.positions[i].rowWallPosition = EditorGUILayout.IntField("X", wallPositions.positions[i].rowWallPosition);
            wallPositions.positions[i].colWallPosition = EditorGUILayout.IntField("Z", wallPositions.positions[i].colWallPosition);
            wallPositions.positions[i].rowLength = EditorGUILayout.IntField("X Length", wallPositions.positions[i].rowLength);
            wallPositions.positions[i].colLength = EditorGUILayout.IntField("Z Length", wallPositions.positions[i].colLength);

            // Button for remove elements from the array
            if (GUILayout.Button("Remove"))
                ArrayUtility.RemoveAt(ref wallPositions.positions, i);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical(); 
            EditorGUILayout.Space();
        }
        // Mark as dirty to save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(wallPositions);
        }
        
    }
}

