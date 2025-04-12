using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles spawning of player, enemy, and obstacle prefabs based on GridSpawinData.
/// </summary>
public class ObstacleManager : MonoBehaviour
{

    [Header("Prefabs")]
    [Tooltip("Prefab to spawn Obstructions")]
    [SerializeField] private GameObject obstaclePrefab;

    [Tooltip("Prefab to spawn Player")]
    [SerializeField] private GameObject playerPrefab;

    [Tooltip("Prefab to spawn Player")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("GameObjects")]
    [Tooltip("Container object for obstacles")]
    [SerializeField] private GameObject Obstacle;
    
    [Tooltip("Container object for Player parent")]
    [SerializeField] private GameObject playerManager;

    [Tooltip("Container object for Enemy parent")]
    [SerializeField] private GameObject enemyManager;

    [Header("ScriptableData Asset")]
    [SerializeField] public GridSpawnData spawingDataAsset;


    private void Start()
    {
        ValidateGridSetup();
        SpawnPrefabs();
    }

    /// <summary>
    /// Spawns prefabs according to the values set in the spawning data grid.
    /// </summary>
    private void SpawnPrefabs()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 10; z++)
            {
                CellType cell = spawingDataAsset.grid[x + z * 10];
                Vector3 spawnPos = new Vector3(x, 1, z);

                switch (cell)
                {
                    case CellType.Enemy:
                        Instantiate(enemyPrefab, enemyManager.transform);
                        enemyManager.transform.position = spawnPos;
                        break;

                    case CellType.Player:
                        Instantiate(playerPrefab, playerManager.transform);
                        playerManager.transform.position = spawnPos;
                        break;

                    case CellType.Obstruction:
                        GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
                        obstacle.transform.SetParent(Obstacle.transform);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Ensures the grid contains at least one player and one enemy cell.
    /// Logs a warning if either is missing.
    /// </summary>
    private void ValidateGridSetup()
    {
        bool hasPlayer = false;
        bool hasEnemy = false;

        foreach (CellType cell in spawingDataAsset.grid)
        {
            if (cell == CellType.Player) hasPlayer = true;
            if (cell == CellType.Enemy) hasEnemy = true;
        }

        if (!hasPlayer)
            Debug.LogWarning("No player tile found in the grid. Please restart the game with a player assigned.");

        if (!hasEnemy)
            Debug.LogWarning("No enemy tile found in the grid. Please restart the game with an enemy assigned.");
    }
}
