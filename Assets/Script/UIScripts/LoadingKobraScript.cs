using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LoadingKobraScript : MonoBehaviour
{
    // Public settings
    #region Public Variables
    [Header("Settings")]
    [Range(1.0f, 5.0f)]
    public float turnTimer = 2.0f;
    public float speed = 20.0f;
    public float tailSmoothness = 0.1f;
    public float tailsDistance = 2.0f;
    public float tailScale = 1.0f;

    [Header("Objects")]
    public GameObject kobraHead;
    public GameObject kobraTail;
    public GameObject tailPrefab;
    #endregion
    // Private settings
    #region Private Variables
    private float spawnTime;
    private float rotationTarget;
    private bool rotating = false;
    private float rotationSpeed = 90.0f;

    private MyTime time;
    private Vector3 currentDirection = Vector3.up;
    private List<Transform> tails = new List<Transform>();
    private List<Vector3> positions = new List<Vector3> ();
    #endregion

    #region Cycle Life
    void Start()
    {
        if (kobraHead == null)
        {
            Debug.LogError("kobraHead is not assigned.");
        }

        time = new MyTime();
        rotationTarget = kobraHead.transform.rotation.eulerAngles.z; // Inizialize where the Kobra have to turn

        positions.Add(kobraHead.transform.position); // add the Kobra initial position to the List
    }
    void Update()
    {
        LoadingMove();
        SmootRotation();
        TailMovement();
    }
    #endregion

    #region Private Method
    // Move the kobra and rotate it every tot seconds
    private void LoadingMove()
    {
        kobraHead.transform.localPosition += currentDirection * speed * Time.deltaTime;

        if (!rotating)
            time.Every(turnTimer, KobraRotation);

        positions.Insert(0, kobraHead.transform.position);
    }
    // Update direction when Kobra have to turn
    private void UpdateDirection()
    {
        switch (currentDirection)
        {
            case Vector3 dir when dir == Vector3.up:
                currentDirection = Vector3.left;
                break;
            case Vector3 dir when dir == Vector3.left:
                currentDirection = Vector3.down;
                break;
            case Vector3 dir when dir == Vector3.down:
                currentDirection = Vector3.right;
                break;
            case Vector3 dir when dir == Vector3.right:
                currentDirection = Vector3.up;
                break;
        }
    }
    // Update the Kobra rotation
    private void KobraRotation()
    {
        rotating = true;
        rotationTarget += 90.0f;
    }
    // Smoothly rotate the Kobra head
    private void SmootRotation()
    {
        if (rotating)
        {
            float currentRotation = kobraHead.transform.rotation.eulerAngles.z;
            float newRotation = Mathf.MoveTowardsAngle(currentRotation, rotationTarget, rotationSpeed * Time.deltaTime);

            kobraHead.transform.rotation = Quaternion.Euler(0, 0, newRotation);

            if (Mathf.Approximately(newRotation, rotationTarget))
            {
                rotating = false;
                UpdateDirection();
            }
        }
    }
    // Move the tail following the head
    private void TailMovement()
    {
        for (int i = 0; i < tails.Count; i++)
        {
            Vector3 targetPosition = positions[Mathf.Min((i + 1) * Mathf.RoundToInt(tailsDistance / speed * 100), positions.Count - 1)];
            tails[i].position = Vector3.Lerp(tails[i].position, targetPosition, tailSmoothness);
        }

        if (positions.Count > (tails.Count + 1) * Mathf.RoundToInt(tailsDistance / speed * 100))
        {
            positions.RemoveAt(positions.Count - 1);
        }
    }

    #endregion

    #region Public Methods
    // Add a tail segment
    public void AddTail()
    {
        Vector3 spawnPosition = tails.Count == 0 ? kobraHead.transform.localPosition : tails[tails.Count - 1].localPosition - Vector3.one;
        GameObject tailSegment = Instantiate(tailPrefab, spawnPosition, Quaternion.identity, kobraTail.transform);
        tailSegment.transform.localScale = Vector3.one * tailScale;
        tails.Add(tailSegment.transform);

        positions.Add(spawnPosition);
    }
    #endregion
}

