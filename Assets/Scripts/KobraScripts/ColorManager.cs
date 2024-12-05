using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public void ApplyColorsChildPrefab(GameObject parent, string childName, Color color)
    {
        Transform child = parent.transform.Find(childName);
        if (child == null)
        {
            Debug.LogError($"Child '{childName}' not found in prefab '{parent.name}'");
            return;
        }

        Renderer renderer = child.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError($"Renderer not found on child '{childName}' in prefab '{parent.name}'");
            return;
        }

        renderer.material.color = color;
    }
    public void ApplyColorsChild(GameObject parent, Color color)
    {
        if (parent == null)
        {
            Debug.LogError($"Parent '{parent}' not found");
            return;
        }

        Renderer renderer = parent.GetComponentInChildren<Renderer>();
        if (renderer == null)
        {
            Debug.LogError($"Renderer not found on child in parent '{parent}'");
            return;
        }

        renderer.material.color = color;
    }
}
