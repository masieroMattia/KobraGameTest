using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KobraHead))]
public class KobraHeadEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Ottieni il riferimento al target
        KobraHead kobraHead = (KobraHead)target;

        // Disegna le proprietà di base (esclude quelle nascoste da [HideInInspector])
        DrawDefaultInspector();

        // Modifica dinamica della visibilità delle proprietà
        if (kobraHead.spawnMode == KobraHead.SpawnMode.PreciseSpawn)
        {
            // Mostra le coordinate solo se PreciseSpawn è selezionato
            EditorGUILayout.LabelField("Precise Spawn Settings", EditorStyles.boldLabel);
            kobraHead.initialSpawnPositionX = EditorGUILayout.IntField("Initial Spawn Position X", kobraHead.initialSpawnPositionX);
            kobraHead.initialSpawnPositionZ = EditorGUILayout.IntField("Initial Spawn Position Z", kobraHead.initialSpawnPositionZ);
        }

        // Applica eventuali modifiche
        if (GUI.changed)
        {
            EditorUtility.SetDirty(kobraHead); // Segna l'oggetto come modificato
        }
    }
}
