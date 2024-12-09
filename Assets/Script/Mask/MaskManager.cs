using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    private LevelGrid levelGrid; // Assuming you have a LevelGrid script
    public List<Mask> masks { get; set; } // List to hold both MaskInfo and spawned GameObjects
    private ColorManager colorManager;
    private MyTime myTime;
    private Color[] maskColors;
    public GameObject maskPrefab; // Reference to the mask prefab to be spawned
    public Color colorResurrectMask;
    public Color colorCollectEggsMask;
    public Color colorcollectLittleSnakeMask;


    
    void Start()
    {
        masks = new List<Mask>(2);
        maskColors = new Color[] {
            colorResurrectMask,
            colorCollectEggsMask,
            colorcollectLittleSnakeMask
        };
        // Instantiate the time
        myTime = new MyTime();

        // Instantiate the color manager
        colorManager = new ColorManager();

        // Find the LevelGrid object by tag and get its script component
        GameObject levelGridObject = GameObject.FindGameObjectWithTag("LevelGrid");
        if (levelGridObject != null)
        {
            levelGrid = levelGridObject.GetComponent<LevelGrid>();
            Debug.Log("Level Grid found via tag.");
        }
        else
        {
            Debug.LogError("LevelGrid object not found.");
        }

        if (levelGrid == null)
        {
            Debug.LogError("LevelGrid object not found.");
        }

        if (maskPrefab == null)
        {
            Debug.LogError("Mask Prefab not found.");
        }

        // Add three Mask objects (MaskInfo + MaskObject) to the list
        for (int i = 0; i < 3; i++)
        {
            MaskInfo maskInfo = new MaskInfo(); // Create new MaskInfo with random effect
            GameObject maskObject = levelGrid.SpawnItemOnTheGrid(maskPrefab, this.gameObject); // Spawn the mask object

            // Ensure the mask position has integer coordinates
            Vector3Int roundedPosition = Vector3Int.RoundToInt(maskObject.transform.position);
            maskObject.transform.position = (Vector3Int)roundedPosition;

            Mask mask = new Mask(maskInfo, maskObject); // Create a new Mask that holds both
            GetMaskColorEffect(mask.MaskInfo.Effect, maskObject, maskColors);
            masks.Add(mask); // Add to the list
            Debug.Log($"Randomly selected effect for mask: {mask.MaskInfo.Effect}");
            Debug.Log($"Mask {i} position : {mask.MaskObject.transform.position}");
        }

    }
    void Update()
    {
    }
    public List<Mask> GetMasks()
    {
        return masks;
    }
    private void GetMaskColorEffect(Effect effect,GameObject maskObject, Color[] masksColors)
    {
        if (effect == Effect.resurrect)
        {
            colorManager.ApplyColorsPrefab(maskObject, masksColors[0]);
        }
        if (effect == Effect.collectEggs)
        {
            colorManager.ApplyColorsPrefab(maskObject, masksColors[1]);
        }
        if (effect == Effect.collectLittleSnake)
        {
            colorManager.ApplyColorsPrefab(maskObject, masksColors[2]);
        }
    }
}

// New Mask class to hold both MaskInfo and the spawned GameObject
public class Mask
{
    public MaskInfo MaskInfo { get; set; }
    public GameObject MaskObject { get; set; }

    // Constructor to initialize both MaskInfo and the GameObject
    public Mask(MaskInfo maskInfo, GameObject maskObject)
    {
        MaskInfo = maskInfo;
        MaskObject = maskObject;
    }
}

// MaskInfo class to hold the effect information
public class MaskInfo
{
    public Effect Effect { get; set; }
    // Constructor that randomly assigns an effect
    public MaskInfo()
    {
        Effect = GetRandomEffect();
    }

    // Randomly generates an effect
    private Effect GetRandomEffect()
    {
        int randomEffectValue = UnityEngine.Random.Range(1, 4); // Random range 1 to 3
        return (Effect)randomEffectValue; // Convert the int to the Effect enum
    }
    
}

// Enum for effects
public enum Effect
{
    resurrect = 1,
    collectEggs = 2,
    collectLittleSnake = 3
};
