using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EggsManager))]
public class EggsSpawnEditor : Editor // Derived class from class Editor for Inspector creation
{
    // Override of a Editor class Method
    public override void OnInspectorGUI()
    {
        EggsManager eggsArea = (EggsManager)target; // Casting target (from Editor class) to the custom type EggsManager for free access to Editor properties and methods 
        if (GUILayout.Button("Add Area")) // Button to add a new position
            ArrayUtility.Add(ref eggsArea.areas, new EggsSpawnArea()); // Add a new egg spawn area object to the position
        if (GUILayout.Button("Clear Area")) // Button to clear the array
            eggsArea.areas = new EggsSpawnArea[0]; // Reset the array

        EditorGUILayout.Space();

        // Iterate the walls array
        for (int i = 0; i < eggsArea.areas.Length; i++)
        {
            // Create the layout
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.Space();

            // Create the field for variables
            eggsArea.areas[i].rowStartTile = EditorGUILayout.IntField("X start", eggsArea.areas[i].rowStartTile);
            eggsArea.areas[i].colStartTile = EditorGUILayout.IntField("Z start", eggsArea.areas[i].colStartTile);
            eggsArea.areas[i].rayArea = EditorGUILayout.IntField("Ray area", eggsArea.areas[i].rayArea);

            // Button for remove elements from the array
            if (GUILayout.Button("Remove"))
                ArrayUtility.RemoveAt(ref eggsArea.areas, i);

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        // Mark the object as "dirty" to ensure changes are saved
        EditorUtility.SetDirty(target);
    }
}

