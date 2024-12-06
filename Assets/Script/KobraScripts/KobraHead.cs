using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KobraHead : MonoBehaviour
{
    #region Public variables
    [Header("Movement Kobra")]
    [SerializeField] private int moveStep = 1; //  Size step for single movement
    [SerializeField] private float moveInterval = 0.8f; // Time between movement

    [Header("Kobra prefab")]
    [Min(1)]
    [SerializeField] private int numberOfHeadCube = 2;
    [SerializeField] private GameObject headKobraPrefab;
    [SerializeField] private Transform tailPrefab;
    [SerializeField] private Vector3Int tailOffset = new Vector3Int(0, 0, -1);

    [Header("Color Kobra Settings")]
    [SerializeField] private Color headColor = Color.black;   // Kobra head Color 
    [SerializeField] private Color tailColor = Color.green;  // tail Kobra Color


    #endregion

    #region Private variables
    private GameManager gameManager;
    private Vector3Int currentDirection = Vector3Int.zero;
    private Vector3Int nextDirection = Vector3Int.zero;
    private List<Transform> kobraList;
    private LevelGrid levelGrid;
    private List<Transform> spawnedEggs = new List<Transform>();
    private MyTime movementTimer;
    private int eggsCounter = 0;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if(headColor == null)
            Debug.Log("Color head null");
        if(tailColor == null)
            Debug.Log("Color tail null");

        // Instantiate the time
        movementTimer = new MyTime();
        //tailsList = new List<Transform>();
        kobraList = new List<Transform>();

        // Find a object with a specif tag
        GameObject levelGridObject = GameObject.FindGameObjectWithTag("LevelGrid");
        if (levelGridObject != null)
        {
            levelGrid = levelGridObject.GetComponent<LevelGrid>();
            Debug.Log("Level Grid trovata tramite tag");
        }

        if (levelGrid == null)
        {
            Debug.LogError("LevelGrid non trovato con il tag 'LevelGrid'. Assicurati che l'oggetto abbia il tag corretto e il componente LevelGrid.");
        }
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            Debug.Log("Game Manager trovato");
        }

        if (gameManager == null)
        {
            Debug.LogError("Game Manager non trovato");
        }
    }
    void Start()
    {
        Egg eggsManager = FindObjectOfType<Egg>();
        if (eggsManager != null)
        {
            eggsManager.OnEggSpawned += UpdateSpawnedEggsList;
        }
        else
        {
            Debug.LogError("Eggs manager not found.");
        }
        transform.position = Vector3.zero;
        SpawnHead();
    }
    private void Update()
    {
        HandleInput();
        movementTimer.Every(moveInterval, MoveInDirection);
        KobraAte();
        
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



            // Salva la posizione corrente dell'ultimo segmento della testa
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

        GameObject kobraHead = levelGrid.SpawnItemOnTheGrid(headKobraPrefab, this.gameObject);
        levelGrid.ApplyColorsChild(kobraHead, headColor);
        kobraList.Add(kobraHead.transform);

        for (int i = 1; i < numberOfHeadCube; i++)
        {
            Transform kobraTail= Instantiate(tailPrefab, this.transform);
            levelGrid.ApplyColorsChild(kobraTail.gameObject, tailColor);
            kobraList.Add(kobraTail);

            if (i > 0)
            {
                kobraList[i].position = (Vector3Int.RoundToInt(kobraList[i - 1].position + tailOffset));

            }

        }
        //Transform child = transform.GetChild(0);
        Debug.Log("Testa del serpente generata con successo.");
    }
    private void GrowKobra()
    {
        // First tail not showing
        Transform newTail = Instantiate(tailPrefab,this.transform);
        newTail.position = (Vector3)(Vector3Int.RoundToInt(kobraList[kobraList.Count - 1].position - currentDirection * moveStep));
        kobraList.Add(newTail);
        levelGrid.ApplyColorsChild(newTail.gameObject,tailColor);

    }
    private void KobraAte()
    {

        for (int i = spawnedEggs.Count - 1; i >= 0; i--)
        {
            if (Vector3Int.RoundToInt(kobraList[0].position) == Vector3Int.RoundToInt(spawnedEggs[i].position))
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
    
    private void CheckGameOver()
    {
        // Controlla se la testa si scontra con altri segmenti della testa
        for (int i = 1; i < kobraList.Count; i++)
        {
            if (Vector3Int.RoundToInt(kobraList[i].localPosition) == Vector3Int.RoundToInt(kobraList[0].localPosition))
            {
                Debug.LogError("Game Over: Kobra collided with itself (head collision)");
                // Aggiungi logica per gestire la fine del gioco
                gameManager.StopGame();
                return;
            }
        }
    }
    private void Turn(float angle)
    {
        transform.rotation = Quaternion.Euler(0, angle, 0);
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
