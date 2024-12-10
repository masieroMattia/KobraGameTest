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
    private AudioManager audioManager; // AudioManager class reference
    private MaskManager maskManager;// MaskManager class reference

    private List<Transform> kobraList; // Kobra List 
    private List<Transform> spawnedEggs = new List<Transform>(); // Eggs list

    private Transform kobraHeadListPosition; // Kobra Head list position

    private Vector3Int currentDirection = Vector3Int.forward; // Current Kobra Direction
    private Vector3Int nextDirection = Vector3Int.zero; // Next Kobra Direction

    private int eggsCounter = 0; // Eggs Counter

    private WallsPositionAndLength[] wallPositions;
    private List<Mask> masksList;

    [HideInInspector] public bool isResurrecting = false;
    Vector3Int kobraHeadPos;

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
        
        // Find a object with a 'MaskManager' component
        maskManager = FindObjectOfType<MaskManager>();

        if (maskManager != null)
        {
            Debug.Log("Mask Manager trovato");
        }
        if (maskManager == null)
        {
            Debug.LogError("Mask Manager non trovato");
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

        // Find a object with a 'GameManager' component
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            Debug.Log("Audio Manager trovato");
        }
        if (audioManager == null)
        {
            Debug.LogError("Audio Manager non trovato");
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
        audioManager.PlayClipAtPointOneTime(audioManager.level1,audioManager.BGM,true,true);       
        
        transform.position = Vector3.zero;
        SpawnHead();
    }
    private void Update()
    {
        // Converte la posizione della testa del Kobra a interi per allinearla alla griglia
        kobraHeadPos = Vector3Int.RoundToInt(kobraHeadListPosition.localPosition);
        HandleInput();
        movementTimer.Every(moveInterval, MoveInDirection);
        KobraAte();
        CollideWalls();
        CollideMask();
    }
    #endregion
    #region Private method
    private void HandleInput()
    {
        // Rileva l'input dell'utente per cambiare direzione
        if (Input.GetKeyDown(KeyCode.W) && currentDirection != Vector3Int.back)
        {
            audioManager.PlayClipAtPoint(audioManager.kobraDirection, audioManager.SFX, true, false);
            nextDirection = Vector3Int.forward;
        }
        else if (Input.GetKeyDown(KeyCode.A) && currentDirection != Vector3Int.right)
        {
            audioManager.PlayClipAtPoint(audioManager.kobraDirection, audioManager.SFX, true, false);
            nextDirection = Vector3Int.left;
        }
        else if (Input.GetKeyDown(KeyCode.S) && currentDirection != Vector3Int.forward)
        {
            audioManager.PlayClipAtPoint(audioManager.kobraDirection, audioManager.SFX, true, false);
            nextDirection = Vector3Int.back;
        }
        else if (Input.GetKeyDown(KeyCode.D) && currentDirection != Vector3Int.left)
        {
            audioManager.PlayClipAtPoint(audioManager.kobraDirection, audioManager.SFX, true, false);
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
        CollideWithItsSelf();
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
    private void MoveBackward()
    {
        // Calcola la direzione opposta rispetto alla direzione attuale
        Vector3Int oppositeDirection = -currentDirection;

        // Inizia a muovere il corpo, partendo dall'ultimo segmento della coda
        // Sposta ogni segmento in avanti (o meglio, in direzione opposta)
        for (int i = 0; i < kobraList.Count - 1; i++)
        {
            kobraList[i].localPosition = kobraList[i + 1].localPosition;


        }
        kobraList[kobraList.Count - 1].localPosition = WrapAround(Vector3Int.RoundToInt(kobraList[1].localPosition + oppositeDirection * moveStep));
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
            colorManager.ApplyColorsPrefab(kobraTail.gameObject, tailColor);
            kobraList.Add(kobraTail);

            if (i > 0)
            {
                Vector3Int tailPosition = Vector3Int.RoundToInt(kobraList[i - 1].position + tailOffset);
                tailPosition = WrapAround(tailPosition);
                kobraList[i].position = tailPosition;
            }

        }
        audioManager.PlayClipAtPoint(audioManager.kobraSpawn,audioManager.SFX,true, false);
        Debug.Log("Testa del serpente generata con successo.");
    }
    
    private void GrowKobra()
    {
        Transform newTail = Instantiate(tailPrefab,this.transform);
        newTail.position = (Vector3)(Vector3Int.RoundToInt(kobraList[kobraList.Count - 1].position - currentDirection * moveStep));
        kobraList.Add(newTail);
        colorManager.ApplyColorsPrefab(newTail.gameObject,tailColor);
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
                audioManager.PlayClipAtPoint(audioManager.eggEaten, audioManager.SFX, true, false);
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

    private void CollideWalls()
    {
        if (gameManager.isGameOverTransitioning) return;
        if (wallPositions == null || wallPositions.Length == 0) return;
      
        foreach (var wall in wallPositions)
        {

            // Calcola la metà della lunghezza del muro (con 'lunghezza - 1' per l'indice corretto)
            int halfLengthX = (wall.rowLength - 1) / 2;
            int halfLengthZ = (wall.colLength - 1) / 2;

            // Calcola i limiti del muro considerando la metà della lunghezza
            int wallMinX = wall.rowWallPosition - halfLengthX;
            int wallMaxX = wall.rowWallPosition + halfLengthX; // Fine inclusiva
            int wallMinZ = wall.colWallPosition - halfLengthZ;
            int wallMaxZ = wall.colWallPosition + halfLengthZ; // Fine inclusiva

            // Controlla se la testa del Kobra è all'interno dei limiti del muro
            if (kobraHeadPos.x >= wallMinX && kobraHeadPos.x <= wallMaxX &&
                kobraHeadPos.z >= wallMinZ && kobraHeadPos.z <= wallMaxZ)
            {
                Debug.Log($"Collisione con il muro a posizione X: {kobraHeadPos.x}, Z: {kobraHeadPos.z}");
                MoveBackward();
                CheckIfResurrecting();
                return;
            }
        }
    }
    
    private void CollideMask()
    {
        masksList = maskManager.masks;
        if (masksList == null)
        {
            Debug.LogError("La lista masksList è null.");
            return;
        }

        if (masksList.Count == 0)
        {
            Debug.LogError("La lista masksList è vuota.");
            return;
        }

        foreach (var mask in masksList)
        {
            // Controlla se la testa del Kobra è all'interno dei limiti del muro
            if (Vector3Int.RoundToInt(mask.MaskObject.transform.position) ==
                Vector3Int.RoundToInt(kobraHeadListPosition.position))
            {
                Debug.Log($"Collisione con maschera a posizione X: {mask.MaskObject.transform.position.x}, " +
                    $"Z: {mask.MaskObject.transform.position.z} con effetto: {mask.MaskInfo.Effect}");
                audioManager.PlayClipAtPointOneTime(audioManager.maskCollected, audioManager.SFX, true, false);
                mask.MaskObject.gameObject.SetActive(false);
                if (mask.MaskInfo.Effect == Effect.resurrect)
                {
                    isResurrecting = true;
                    Debug.Log(isResurrecting.ToString());
                }
                return;
            }
        }
    }
    private void CollideWithItsSelf()
    {
        // Controlla se la testa si scontra con altri segmenti della testa
        for (int i = 1; i < kobraList.Count; i++)
        {
            if (Vector3Int.RoundToInt(kobraList[i].localPosition) == Vector3Int.RoundToInt(kobraHeadListPosition.localPosition))
            {
                CheckIfResurrecting();
            }
        }
    }

    private void CheckIfResurrecting()
    {
        if (isResurrecting)
        {
            audioManager.PlayClipAtPoint(audioManager.kobraResurrect, audioManager.SFX, true, false);
            RespawnKobra(); // Respawna il cobra
            isResurrecting = false; // Resetta il flag

        }
        else
        {
            audioManager.PlayClipAtPoint(audioManager.kobraDeath, audioManager.SFX, true, false);
            gameManager.GameOver();
        }
    }
    private void RespawnKobra()
    {
        // Nascondi tutti i segmenti della coda corrente invece di distruggerli
        foreach (var segment in kobraList)
        {
            if (segment != null)
            {
                segment.gameObject.SetActive(false); // Nascondi il segmento

            }
        }
        kobraList.Clear();

        // Ripristina la posizione iniziale
        transform.position = Vector3.zero;

        // Ricrea la testa
        SpawnHead();

        // Ripristina la direzione
        currentDirection = Vector3Int.forward;
        nextDirection = Vector3Int.zero;

        Debug.Log("Il cobra è stato respawnato con dimensioni iniziali.");
    }

    private void UpdateSpawnedEggsList(List<Transform> eggsList)
    {
        spawnedEggs = eggsList;
        Debug.Log("Updated spawned eggs list in SneakHead.");
    }

    #endregion
    #region Public method
    #endregion


}
