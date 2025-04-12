using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Manages Enemy and movement
/// Inherits its path AI finding from MoveableObject class and blueprint from InterfaceAI
/// </summary>
public class EnemyAI : MoveableObject, InterfaceAI
{
    // Reference to the Player object's Manager/Parent object in the scene
    [SerializeField] private GameObject playerManager;

    private void Awake()
    {
        LoadData();
    }
    // Keeps track if Enemy is currently moving or not
    public bool isMoving { get; private set; }

    private void Start()
    {
        isMoving = false;
        PlayerController.isPlayerStationary += TriggerEnemyMovement;
        TriggerEnemyMovement();
    }

    //Starts Pathfinding and movement towards the player's position
    private void TriggerEnemyMovement()
    {

        // Gets the player postion from the grid while mainting the Y postion to 1 
        endPosition = new Vector3(
            Mathf.Floor(playerManager.transform.position.x),
            1,
            Mathf.Floor(playerManager.transform.position.z)
        );
        
        SetPath();

        //if path found start following it
        if (path != null && path.Count > 0 && !isMoving)
        {
            StopAllCoroutines();
            StartCoroutine(FollowPath());
        }
    }


    /// <summary>
    /// Moves the enemy along the calculated path.
    /// Stops before reaching the player's final position.
    /// </summary>
    public IEnumerator FollowPath()
    {
        isMoving = true;

        // Remove the final step to avoid occupying the player's tile
        path.RemoveAt(path.Count - 1);
        
        yield return StartCoroutine(Startmoving());

        isMoving = false;
    }

}
