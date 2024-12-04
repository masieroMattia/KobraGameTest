using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SnakesManager))]
public class SnakesSpawnEditor : Editor // Derived class from class Editor for Inspector creation
{
    // Override of a Editor class Method
    public override void OnInspectorGUI()
    {
        SnakesManager snakesArea = (SnakesManager)target; // Casting target (from Editor class) to the custom type SnakesManager for free access to Editor properties and methods 
        if (GUILayout.Button("Add Area")) // Button to add a new position
            ArrayUtility.Add(ref snakesArea.areas, new SnakesSpawnArea()); // Add a new snake spawn area object to the position
        if (GUILayout.Button("Clear Area")) // Button to clear the array
            snakesArea.areas = new SnakesSpawnArea[0]; // Reset the array

        EditorGUILayout.Space();

        // Iterate the walls array
        for (int i = 0; i < snakesArea.areas.Length; i++)
        {
            // Create the layout
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.Space();

            // Create the field for variables
            snakesArea.areas[i].rowStartTile = EditorGUILayout.IntField("X start", snakesArea.areas[i].rowStartTile);
            snakesArea.areas[i].colStartTile = EditorGUILayout.IntField("Z start", snakesArea.areas[i].colStartTile);
            snakesArea.areas[i].rayArea = EditorGUILayout.IntField("Ray area", snakesArea.areas[i].rayArea);

            // Button for remove elements from the array
            if (GUILayout.Button("Remove"))
                ArrayUtility.RemoveAt(ref snakesArea.areas, i);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        // Mark the object as "dirty" to ensure changes are saved
        EditorUtility.SetDirty(target);
    }
}

