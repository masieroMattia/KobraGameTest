using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ColorManager
{
    public void ApplyColorsSpecificChildPrefab(GameObject parent, string childName, Color color)
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
    public void ApplyColorsPrefab(GameObject parent, Color color)
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
    public void ApplyColorsComponent(GameObject parent, Component component, Color color)
    {
        if (parent == null)
        {
            Debug.LogError("Parent is null");
            return;
        }

        if (component == null)
        {
            Debug.LogError("Component is null");
            return;
        }

        if (component is Renderer renderer)
        {
            renderer.material.color = color;
        }
        else if (component is Graphic graphic)
        {
            graphic.color = color;
        }
        else
        {
            Debug.LogError($"The component '{component.GetType().Name}' does not support a color property.");
        }
    }

    public void ApplyColorToPanel(GameObject panel, Color ColorPanel)
    {
        if (panel != null)
        {
            // Get the Image component from the GameObject
            UnityEngine.UI.Image imageComponent = panel.GetComponent<UnityEngine.UI.Image>();

            // Apply the color using the ColorManager
            ApplyColorsComponent(panel, imageComponent, ColorPanel);
        }

    }





}
