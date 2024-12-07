using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KobraHead : MonoBehaviour
{
    #region Public enums
    public enum SpawnMode : byte
    {
        RandomSpawn,
        PreciseSpawn
    }
    #endregion
    #region Public variables
    [Header("Movement Kobra")]
    [SerializeField] private int moveStep = 1; // Size step for single movement
    [SerializeField] private float moveInterval = 0.8f; // Time between movement

    [Header("Kobra prefab")]
    [Min(1)]
    [SerializeField] private int initialStartTailSize = 2; // Initial size of the tail
    [SerializeField] private GameObject headKobraPrefab; // Head Kobra prefab
    [SerializeField] private Transform tailPrefab; // Tail prefab
    [SerializeField] private Vector3Int tailOffset = new Vector3Int(0, 0, -1);

    [Header("Color Kobra Settings")]
    [SerializeField] private Color headColor = Color.black;   // Kobra head Color 
    [SerializeField] private Color tailColor = Color.green;  // tail Kobra Color

    [Header("Kobra Spawn")]
    [HideInInspector] public int initialSpawnPositionX; // Z for Precise Kobra Spawn
    [HideInInspector] public int initialSpawnPositionZ; // X for Precise Kobra Spawn
    public SpawnMode spawnMode = SpawnMode.RandomSpawn; // Spawn Mode

    [Header("Wall Reference")]
    [SerializeField] private WallsManager wallsManager;
    #endregion

    #region Private variables
    private GameManager gameManager; // GameManager class reference
    private ColorManager colorManager; // ColorManager class reference
    private MyTime movementTimer; // MyTime class reference
    private LevelGrid levelGrid; // LevelGrid class reference

    private List<Transform> kobraList; // Kobra List 
    private List<Transform> spawnedEggs = new List<Transform>(); // Eggs list

    private Transform kobraHeadListPosition; // Kobra Head list position

    private Vector3Int currentDirection = Vector3Int.forward; // Current Kobra Direction
    private Vector3Int nextDirection = Vector3Int.zero; // Next Kobra Direction

    private int eggsCounter = 0; // Eggs Counter

    private WallsPositionAndLength[] wallPositions;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if(headColor == null) // Check if is null
            Debug.LogError("Color head null");

        if(tailColor == null) // Check if is null
            Debug.LogError("Color tail null");

        // Instantiate the time
        movementTimer = new MyTime();
        
        // Instatiate the Color Manager
        colorManager = new ColorManager();
        
        // Kobra List 
        kobraList = new List<Transform>();

        // Checking the most Initial Starting Size
        //if(initialStartTailSize > levelGrid.sizeXGrid + levelGrid.sizeZGrid)
            //initialStartTailSize = levelGrid.sizeXGrid + levelGrid.sizeZGrid;

        // Find a object with a specif tag
        GameObject levelGridObject = GameObject.FindGameObjectWithTag("LevelGrid");
        if (levelGridObject != null)
        {
            levelGrid = levelGridObject.GetComponent<LevelGrid>();
            initialSpawnPositionX = Mathf.Clamp(initialSpawnPositionX, 1, levelGrid.sizeXGrid - 1);
            initialSpawnPositionZ = Mathf.Clamp(initialSpawnPositionZ, 1, levelGrid.sizeZGrid - 1);
            Debug.Log("Level Grid trovata tramite tag");
        }

        if (levelGrid == null)
        {
            Debug.LogError("LevelGrid non trovato con il tag 'LevelGrid'. Assicurati che l'oggetto abbia il tag corretto e il componente LevelGrid.");
        }

        // Find a object with a 'GameManager' component
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            Debug.Log("Game Manager trovato");
        }
        if (gameManager == null)
        {
            Debug.LogError("Game Manager non trovato");
        }

        // Find a object with a 'Egg' component
        Egg eggsManager = FindObjectOfType<Egg>();
        if (eggsManager != null)
        {
            eggsManager.OnEggSpawned += UpdateSpawnedEggsList;
        }
        else
        {
            Debug.LogError("Eggs manager not found.");
        }


        // Walls
        if (wallsManager != null)
        {
            // Accedi all'array
            wallPositions = wallsManager.positions;
            // Itera sulle posizioni dei muri
            Debug.Log($"Number of --> {wallPositions.Length}");
            foreach (var wall in wallPositions)
            {
                Debug.Log($"Wall {wall} at X:{wall.rowWallPosition}, Z:{wall.colWallPosition}, Length X:{wall.rowLength}, Length Z:{wall.colLength}");
            }
        }
        else
        {
            Debug.LogError("WallsManager non è assegnato!");
        }
    }
    void Start()
    {        
        transform.position = Vector3.zero;
        SpawnHead();
    }
    private void Update()
    {
        HandleInput();
        movementTimer.Every(moveInterval, MoveInDirection);
        KobraAte();
        CollideWalls();


    }
    #endregion
    #region Private method
    private void HandleInput()
    {
        // Rileva l'input dell'utente per cambiare direzione
        if (Input.GetKeyDown(KeyCode.W) && currentDirection != Vector3Int.back)
        {
            nextDirection = Vector3Int.forward;
        }
        else if (Input.GetKeyDown(KeyCode.A) && currentDirection != Vector3Int.right)
        {
            nextDirection = Vector3Int.left;
        }
        else if (Input.GetKeyDown(KeyCode.S) && currentDirection != Vector3Int.forward)
        {
            nextDirection = Vector3Int.back;
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentDirection != Vector3Int.left)
        {
            nextDirection = Vector3Int.right;
        }
    }
    private void MoveInDirection()
    {
        // Change direction only if its valid
        if (nextDirection != Vector3Int.zero)
        {
            currentDirection = nextDirection;

            // Save the position of the last segment of the snake's tail
            Vector3Int previousPosition = Vector3Int.RoundToInt(kobraList[kobraList.Count - 1].position);

            for (int i = kobraList.Count - 1; i > 0; i--)
            {
                kobraList[i].localPosition = kobraList[i - 1].localPosition;
                         
            }
            kobraList[0].localPosition = WrapAround(Vector3Int.RoundToInt(kobraList[0].localPosition + currentDirection * moveStep));           
        }
        CheckGameOver();
    }
    private void SpawnHead()
    {
        if (levelGrid == null)
        {
            Debug.LogError("LevelGrid è null in SpawnHead. Controlla l'inizializzazione.");
            return;
        }

        if (headKobraPrefab == null)
        {
            Debug.LogError("headKobraPrefab è null in SpawnHead. Controlla l'assegnazione.");
            return;
        }

        GameObject kobraHead;
        switch (spawnMode)
        {
            case SpawnMode.RandomSpawn:
                kobraHead = levelGrid.SpawnItemOnTheGrid(headKobraPrefab, this.gameObject);
                colorManager.ApplyColorsPrefab(kobraHead, headColor);
                kobraList.Add(kobraHead.transform);
            break;

            case SpawnMode.PreciseSpawn:
                kobraHead = levelGrid.SpawnItemOnGridPosition(headKobraPrefab, this.gameObject, initialSpawnPositionX, initialSpawnPositionZ);
                colorManager.ApplyColorsPrefab(kobraHead, headColor);
                kobraList.Add(kobraHead.transform);
            break;
        }
        kobraHeadListPosition = kobraList[0]; // Adding the Kobra Head List Position
        for (int i = 1; i < initialStartTailSize + 1; i++)
        {
            Transform kobraTail= Instantiate(tailPrefab, this.transform);
            levelGrid.ApplyColorsChild(kobraTail.gameObject, tailColor);
            kobraList.Add(kobraTail);

            if (i > 0)
            {
                Vector3Int tailPosition = Vector3Int.RoundToInt(kobraList[i - 1].position + tailOffset);
                tailPosition = WrapAround(tailPosition);
                kobraList[i].position = tailPosition;
            }

        }
        Debug.Log("Testa del serpente generata con successo.");
    }
    private void GrowKobra()
    {
        Transform newTail = Instantiate(tailPrefab,this.transform);
        newTail.position = (Vector3)(Vector3Int.RoundToInt(kobraList[kobraList.Count - 1].position - currentDirection * moveStep));
        kobraList.Add(newTail);
        levelGrid.ApplyColorsChild(newTail.gameObject,tailColor);
    }
    private void DecreaseKobra()
    {
        Transform tailDestroied = kobraList[kobraList.Count - 1];
        kobraList.Remove(tailDestroied);
        
        if (tailDestroied == kobraHeadListPosition)
            gameManager.GameOver();
        Destroy(tailDestroied.gameObject);

    }
    private void KobraAte()
    {

        for (int i = spawnedEggs.Count - 1; i >= 0; i--)
        {
            if (Vector3Int.RoundToInt(kobraHeadListPosition.localPosition) == Vector3Int.RoundToInt(spawnedEggs[i].localPosition))
            {
                Transform eggToDestroy = spawnedEggs[i];
                spawnedEggs.RemoveAt(i);
                if (eggToDestroy != null)
                    eggToDestroy.gameObject.SetActive(false);
                Debug.Log("Kobra ate egg");
                GrowKobra();
                eggsCounter++;
                gameManager.UpdateEggsText(eggsCounter);
               
            }
        }

        
    }
    private void CollideWall()
    {
        for(int i = 0; i < wallPositions.Length; i++)
        {
            if ((int)wallPositions[i].rowWallPosition == Mathf.RoundToInt(kobraHeadListPosition.localPosition.x) &&
                (int)wallPositions[i].colWallPosition == Mathf.RoundToInt(kobraHeadListPosition.localPosition.z))
            {
                gameManager.GameOver();
            }

        }
    }
    private void CollideWalls()
    {
        if (wallPositions == null || wallPositions.Length == 0) return;

        Vector3Int kobraHeadPos = Vector3Int.RoundToInt(kobraHeadListPosition.localPosition);

        foreach (var wall in wallPositions)
        {
            // Definizione dei limiti del muro
            int wallMinX = wall.rowWallPosition;
            int wallMaxX = wall.rowWallPosition + wall.rowLength - 1;
            int wallMinZ = wall.colWallPosition;
            int wallMaxZ = wall.colWallPosition + wall.colLength - 1;

            // Verifica se la testa del Kobra è all'interno dei limiti
            if (kobraHeadPos.x >= wallMinX && kobraHeadPos.x <= wallMaxX &&
                kobraHeadPos.z >= wallMinZ && kobraHeadPos.z <= wallMaxZ)
            {
                Debug.Log($"Collisione con il muro a posizione X: {kobraHeadPos.x}, Z: {kobraHeadPos.z}");
                gameManager.GameOver();
                break;
            }
        }
    }

    
    private void CheckGameOver()
    {
        // Controlla se la testa si scontra con altri segmenti della testa
        for (int i = 1; i < kobraList.Count; i++)
        {
            if (Vector3Int.RoundToInt(kobraList[i].localPosition) == Vector3Int.RoundToInt(kobraHeadListPosition.localPosition))
            {
                Debug.Log("c");
                gameManager.GameOver();
            }
        }
    }
    private void UpdateSpawnedEggsList(List<Transform> eggsList)
    {
        spawnedEggs = eggsList;
        Debug.Log("Updated spawned eggs list in SneakHead.");
    }
    private Vector3Int WrapAround(Vector3Int kobraPosition)
    {
        if (kobraPosition.x < 0)
        {
            kobraPosition.x = levelGrid.sizeXGrid - 1;
        }
        if (kobraPosition.z < 0)
        {
            kobraPosition.z = levelGrid.sizeZGrid - 1;
        }


        if (kobraPosition.x > levelGrid.sizeXGrid - 1)
        {   
            kobraPosition.x = 0;
        }   
        if (kobraPosition.z > levelGrid.sizeZGrid - 1)
        {   
            kobraPosition.z = 0;
        }

        return kobraPosition;
    }

    #endregion
    #region Public method
    #endregion


}
