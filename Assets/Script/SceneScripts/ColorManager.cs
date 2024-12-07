using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager
{
    // Metodo per applicare un materiale Unlit a un oggetto figlio specificato dal nome
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

        Material unlitMaterial = new Material(Shader.Find("Unlit/Color"));
        unlitMaterial.color = color;  // Imposta il colore nel materiale Unlit
        renderer.material = unlitMaterial;
    }

    // Metodo per applicare il materiale Unlit a un prefab
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

        Material unlitMaterial = new Material(Shader.Find("Unlit/Color"));
        unlitMaterial.color = color;  // Imposta il colore nel materiale Unlit
        renderer.material = unlitMaterial;
    }

    // Metodo per applicare il materiale Unlit a un componente specifico
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
            Material unlitMaterial = new Material(Shader.Find("Unlit/Color"));
            unlitMaterial.color = color;  // Imposta il colore nel materiale Unlit
            renderer.material = unlitMaterial;
        }
        else if (component is Graphic graphic)
        {
            graphic.color = color;  // Imposta direttamente il colore nel componente Graphic (come Image, Text, etc.)
        }
        else
        {
            Debug.LogError($"The component '{component.GetType().Name}' does not support a color property.");
        }
    }

    // Metodo per applicare un colore a un pannello (Image)
    public void ApplyColorToPanel(GameObject panel, Color ColorPanel)
    {
        if (panel != null)
        {
            // Ottieni il componente Image dal GameObject
            UnityEngine.UI.Image imageComponent = panel.GetComponent<UnityEngine.UI.Image>();

            if (imageComponent != null)
            {
                ApplyColorsComponent(panel, imageComponent, ColorPanel);  // Applica il colore tramite la funzione di colore
            }
            else
            {
                Debug.LogError("Image component not found on panel.");
            }
        }
    }
}
