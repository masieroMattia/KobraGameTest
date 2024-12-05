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
    private MapGeneration map;
    private ColorManager colorManager;
    private KobraSpawnManager kobraSpawn;
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
        // TailsList = new List<Transform>();
        kobraList = new List<Transform>();

        kobraSpawn.SpawnHead(kobraList, headKobraPrefab, this.transform, numberOfHeadCube, tailPrefab, tailOffset, headColor, tailColor, colorManager);
    }
    
    private void Update()
    {
        HandleInput();
        movementTimer.Every(moveInterval, MoveInDirection);
        
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
                kobraList[i].position = kobraList[i - 1].position;
                         
            }
            kobraList[0].position = WrapAround(Vector3Int.RoundToInt(kobraList[0].position + currentDirection * moveStep));

           
        }
        CheckGameOver();
    }
    
    private void GrowKobra()
    {
        // First tail not showing
        Transform newTail = Instantiate(tailPrefab,this.transform);
        newTail.position = (Vector3)(Vector3Int.RoundToInt(kobraList[kobraList.Count - 1].position - currentDirection * moveStep));
        kobraList.Add(newTail);
        colorManager.ApplyColorsChild(newTail.gameObject,tailColor);

    }
    
    private void CheckGameOver()
    {
        // Controlla se la testa si scontra con altri segmenti della testa
        for (int i = 1; i < kobraList.Count; i++)
        {
            if (Vector3Int.RoundToInt(kobraList[i].position) == Vector3Int.RoundToInt(kobraList[0].position))
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
    
    private Vector3Int WrapAround(Vector3Int kobraPosition)
    {
        if (kobraPosition.x < 0)
        {
            kobraPosition.x = map.rows - 1;
        }
        if (kobraPosition.z < 0)
        {
            kobraPosition.z = map.cols - 1;
        }


        if (kobraPosition.x > map.rows - 1)
        {   
            kobraPosition.x = 0;
        }   
        if (kobraPosition.z > map.cols - 1)
        {   
            kobraPosition.z = 0;
        }

        return kobraPosition;
    }

    #endregion
    #region Public method
    #endregion


}
