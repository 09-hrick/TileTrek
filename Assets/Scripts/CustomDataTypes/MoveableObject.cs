using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObject : MonoBehaviour
{
    /// <summary>
    /// Represents a node in the A* pathfinding grid.
    /// Stores position, costs, and parent reference for path reconstruction.
    /// </summary>
    public class Node
    {
        public Vector3 position;
        public Node parent;
        public float gCost;
        public float hcost;
        public float fCost => gCost + hcost;

        public Node(Vector3 pos, Node par = null)
        {
            position = pos;
            parent = par;
        }
    }

    [SerializeField] protected GridSpawnData spawingDataAsset;
    [SerializeField] protected float speed = 1.0f;
    private int gridSize = 10;
    private bool[,] obstacleGrid = new bool[10, 10];
    protected List<Vector3> path { get; private set; }
    protected Vector3 endPosition;
    
    protected void LoadData()
    {
        if (spawingDataAsset != null)
        {
            for (int x = 0; x < 10; x++)
                for (int z = 0; z < 10; z++)
                    obstacleGrid[x, z] = spawingDataAsset.grid[x + z * 10] == CellType.Obstruction;
        }
        else
        {
            Debug.LogWarning("Missing GridSpawnData Scriptable object");
        }
    }

    /// <summary>
    /// Sets the specified cell as either an obstacle or walkable.
    /// </summary>
    /// <param name="position">Grid position to modify.</param>
    /// <param name="isObstacle">True to mark as obstacle, false to mark as walkable. Default is true.</param>
    protected void SetOrResetCell(Vector3 position, bool isObstacle = true)
    {
        int x = Mathf.FloorToInt(position.x);
        int z = Mathf.FloorToInt(position.z);
        obstacleGrid[x, z] = isObstacle;
    }

    /// <summary>
    /// Computes the path from the current object position to the target end position.
    /// </summary>
    protected void SetPath()
    {
        path = GetPath(transform.position, endPosition);
    }

    protected IEnumerator Startmoving()
    {
        foreach (Vector3 nextPos in path)
        {
            Vector3 startPos = transform.position;
            float elapsedTime = 0f;
            float journeyLength = Vector3.Distance(startPos, nextPos);
            float duration = journeyLength / speed;

            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(startPos, nextPos, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Snap to exact grid position 
            transform.position = nextPos;
        }
    }
    /// <summary>
    /// Checks if the given position exists in the current path.
    /// </summary>
    /// <param name="otherObjectPos">Position to check.</param>
    /// <returns>True if the position is part of the path.</returns>
    protected bool CheckIfOtherObjectCoordinatesInPath(Vector3 otherObjectPos)
    {
        Vector3Int otherIntPos = ToGridPos(otherObjectPos);
        foreach (Vector3 pos in path)
            if (pos == otherIntPos)
                return true;
        return false;
    }

    /// <summary>
    /// Calculates the shortest path using the A* algorithm.
    /// </summary>
    /// <param name="start">Starting position.</param>
    /// <param name="end">Target position.</param>
    /// <returns>List of positions from start to end if path exists, otherwise null.</returns>
    private List<Vector3> GetPath(Vector3 start, Vector3 end)
    {
        List<Node> openList = new List<Node>();
        HashSet<Vector3> closedSet = new HashSet<Vector3>();
        Node startNode = new Node(start);
        startNode.gCost = 0;
        startNode.hcost = CalulateHCost(start, end);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            openList.Sort((a, b) => a.fCost.CompareTo(b.fCost));
            Node curr = openList[0];
            openList.RemoveAt(0);

            if (Vector3.Distance(curr.position, end) < Mathf.Epsilon)
                return ReconstructPath(curr);

            closedSet.Add(curr.position);

            foreach (Vector3 neighbourPos in GetNeighbors(curr.position))
            {
                if (!IsWalkable(neighbourPos) || closedSet.Contains(neighbourPos))
                    continue;

                float tentativeGcost = curr.gCost + 1.0f;
                Node neighborNode = openList.Find(n => n.position == neighbourPos);
                if (neighborNode == null)
                {
                    neighborNode = new Node(neighbourPos, curr);
                    neighborNode.gCost = tentativeGcost;
                    neighborNode.hcost = CalulateHCost(neighbourPos, end);
                    openList.Add(neighborNode);
                }
                else if (tentativeGcost < neighborNode.gCost)
                {
                    neighborNode.parent = curr;
                    neighborNode.gCost = tentativeGcost;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Calculates the Manhattan distance between two points.
    /// </summary>
    private float CalulateHCost(Vector3 start, Vector3 end)
    {
        return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.z - end.z);
    }

    /// <summary>
    /// Reconstructs the path from the end node to the start node.
    /// </summary>
    /// <param name="end">Final node reached by the algorithm.</param>
    /// <returns>List of positions representing the path.</returns>
    private List<Vector3> ReconstructPath(Node end)
    {
        List<Vector3> path = new List<Vector3>();
        Node curr = end;
        while (curr != null)
        {
            path.Add(curr.position);
            curr = curr.parent;
        }
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Returns valid walkable neighbors in the XZ plane.
    /// </summary>
    private List<Vector3> GetNeighbors(Vector3 pos)
    {
        List<Vector3> neighbors = new List<Vector3>();
        List<Vector3> directions = new List<Vector3>
        {
            new Vector3(1,0,0),
            new Vector3(-1,0,0),
            new Vector3(0,0,1),
            new Vector3(0,0,-1)
        };

        foreach (var direction in directions)
        {
            Vector3 neighbor = pos + direction;
            if (IsWalkable(neighbor))
                neighbors.Add(neighbor);
        }
        return neighbors;
    }

    /// <summary>
    /// Checks if a position lies within grid bounds.
    /// </summary>
    private bool IsInBound(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int z = Mathf.FloorToInt(pos.z);
        return x >= 0 && x < gridSize && z >= 0 && z < gridSize;
    }

    /// <summary>
    /// Checks if the specified position is within bounds and not obstructed.
    /// </summary>
    private bool IsWalkable(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int z = Mathf.FloorToInt(pos.z);
        return IsInBound(pos) && !obstacleGrid[x, z];
    }
    private Vector3Int ToGridPos(Vector3 pos)
    {
        Vector3Int posInt = new Vector3Int(
            Mathf.FloorToInt(pos.x),
            1,
            Mathf.FloorToInt(pos.z)
        );
        return posInt;
    }
}
