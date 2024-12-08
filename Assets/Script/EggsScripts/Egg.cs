using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Egg : MonoBehaviour
{

    #region Public variables
    [Header("Egg Setting")]
    [SerializeField] private GameObject eggPrefab;
    [Tooltip("seconds")]
    [SerializeField] private float eggSpawnEvery = 10f;
    [SerializeField] private Transform kobraHead;
   
    public delegate void EggSpawnedEventHandler(List<Transform> spawnedEggs);
    public event EggSpawnedEventHandler OnEggSpawned;
    
    [Header("Color Eggs Settings")]
    [SerializeField] private Color eggColor = Color.black;   // egg Color 
    #endregion

    #region Private variables
    private MyTime myTime;
    private ColorManager colorManager;
    private UnityAction methodList;
    private List<Transform> spawnedEggs = new List<Transform>();
    private LevelGrid levelGrid;
    #endregion

    #region Lifecycle
    void Awake()
    {
        // Instantiate the time
        myTime = new MyTime();
        // Instantiate the color manager
        colorManager = new ColorManager();
        // Verify the presence of the square prefab for the grid creation
        if (eggPrefab == null)
        {
            Debug.LogError("EggPrefab is not assigned!");
            return;
        }
        // Find a object with a specif tag
        GameObject levelGridObject = GameObject.FindGameObjectWithTag("LevelGrid");
        if (levelGridObject != null)
        {
            levelGrid = levelGridObject.GetComponent<LevelGrid>();
            Debug.Log("Level Grid found by tag");
        }

        if (levelGrid == null)
        {
            Debug.LogError("LevelGrid not founded by tag 'LevelGrid'.");
        }
        
        if (kobraHead == null)
        {
            Debug.LogError("kobra head is not assigned!");
        }
        if (eggSpawnEvery <= 0)
        {
            Debug.Log("Egg Spawn Every must be at least 1 seconds");
            return;
        }

        // Adding spawn Egg methods to the delegate
        methodList += SpawnEgg;
    }
    void Start()
    {
       
    }
    void Update()
    {
        StartSpawningEgg();
    }
    #endregion

    #region Private method
    private void SpawnEgg()
    {
       
        GameObject eggObject = levelGrid.SpawnItemOnTheGrid(eggPrefab, this.gameObject);
        spawnedEggs.Add(eggObject.transform);
        colorManager.ApplyColorsPrefab(eggObject, eggColor);
        // Attiva l'evento per notificare SneakHead
        OnEggSpawned?.Invoke(spawnedEggs);
    }
    private void StartSpawningEgg()
    {
        // Spawn egg every seconds 
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
        {
            Debug.Log("Kobra started moving");
            myTime.Every(eggSpawnEvery, methodList);// Spawn egg every decided seconds 
        }
    }
 

    #endregion
    #region Public method
    #endregion

}








