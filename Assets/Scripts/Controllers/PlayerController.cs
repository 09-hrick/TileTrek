using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages Player input and movement
/// Inherits its path AI finding from MoveableObject class and blueprint from InterfaceAI
/// </summary>
public class PlayerController : MoveableObject, InterfaceAI
{
    // Triggeres when the player finishes movement
    public static event Action isPlayerStationary;

    // Keeps track if player is currently moving or not
    public bool isMoving { get; private set; }

    // Reference to the EnemyAI's Manager/Parent object in the scene
    [SerializeField] private GameObject enemyManager;

    private void Awake()
    {
        LoadData();
    }

    //Listens everyframe for Left mouse click
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
           TriggerPlayerMovement();
        }
    }

    //Starts Pathfinding and movement towards the clicked location
    void TriggerPlayerMovement()
    {
        // Prevent movement while enemy is moving
        if (enemyManager.GetComponent<EnemyAI>().isMoving)
        {
            return;
        }
        //Raycast to detect clicked location on the world
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hoveredObject = hit.collider.gameObject;
            if (hoveredObject != null)
            {
                Vector3 clickedPosition = hoveredObject.transform.position;

                if (!isMoving)
                {
                    // Gets the clicked postion from the grid  while mainting the Y postion to 1 
                    endPosition = new Vector3(
                        Mathf.Floor(clickedPosition.x),
                        1,
                        Mathf.Floor(clickedPosition.z)
                    );

                    // Temporarily set the enemy’s cell as an obstacle to  avoid collision
                    SetOrResetCell(enemyManager.transform.position);

                    SetPath();

                    // Revert the enemy’s cell back to walkable  after path calculation
                    SetOrResetCell(enemyManager.transform.position, false);
                }
                //If path is found and player is at rest then start following it 
                if (path != null && path.Count > 0 && !isMoving)
                {
                    StopAllCoroutines();
                    StartCoroutine(FollowPath());
                }
            }
        }
    }

    /// <summary>
    /// Follows the calculated path to the destination while 
    /// avoiding the enemy.
    /// </summary>
    public IEnumerator FollowPath()
    {
        isMoving = true;
        yield return StartCoroutine(Startmoving());
        
        isMoving = false;
        isPlayerStationary?.Invoke(); // Notify Enemy that player has stoped moving and it can start its movement
        
    }

}
