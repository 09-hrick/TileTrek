using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Container to store where the element is located in the grid
/// </summary>
[CreateAssetMenu(fileName = "New GridSpawnData", menuName = "Scriptable Objects/Spawning Data Asset")]
public class GridSpawnData : ScriptableObject
{
    // A 10x10 grid represented as a flat list of cell types
    public List<CellType> grid = Enumerable.Repeat(CellType.EmptyCell, 100).ToList();

    /// <summary>
    /// Resets all grid cells to EmptyCell.
    /// </summary>
    public void ResetGrid()
    {
        for (int i = 0; i < 100; i++)
        {
            grid[i] = CellType.EmptyCell;
        }
    }
}
