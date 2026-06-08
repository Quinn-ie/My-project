using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public LayerMask wallMask;

    public Vector2 gridWorldSize;
    public float nodeRadius;

    public WayPointNodes[,] grid;
    public List<WayPointNodes> debugPath;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;

        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }


    void CreateGrid()
    {
        grid = new WayPointNodes[gridSizeX, gridSizeY];

        Vector3 worldBottomLeft =
            transform.position
            - Vector3.right * gridWorldSize.x / 2
            - Vector3.forward * gridWorldSize.y / 2;

        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint =
                    worldBottomLeft
                    + Vector3.right * (x * nodeDiameter + nodeRadius)
                    + Vector3.forward * (y * nodeDiameter + nodeRadius);

                bool walkable =
                    !Physics.CheckSphere(worldPoint, nodeRadius, wallMask);

                grid[x, y] = new WayPointNodes(walkable, worldPoint, x, y);
            }
        }

        AssignNeighbors();
    }

    void AssignNeighbors()
    {
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                WayPointNodes node = grid[x, y];

                int[,] dirs =
                {
                    {0,1},
                    {0,-1},
                    {-1,0},
                    {1,0}
                };

                for(int i = 0; i < 4; i++)
                {
                    int checkX = x + dirs[i,0];
                    int checkY = y + dirs[i,1];

                    if(checkX >= 0 && checkX < gridSizeX &&
                       checkY >= 0 && checkY < gridSizeY)
                    {
                        node.neighbors.Add(grid[checkX, checkY]);
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if(grid == null)
            return;

        foreach(WayPointNodes node in grid)
        {
            Gizmos.color = node.walkable ? Color.white : Color.red;

            Gizmos.DrawCube(node.worldPosition, Vector3.one * 0.3f);
        }

        if(debugPath != null)
        {
            foreach(WayPointNodes node in debugPath)
            {
                Gizmos.color = Color.green;

                Gizmos.DrawCube(
                    node.worldPosition,
                    Vector3.one * 0.5f);
            }
        }
    }

    public WayPointNodes NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3 localPos = worldPosition - transform.position;

        float percentX =
            (localPos.x + gridWorldSize.x / 2) / gridWorldSize.x;

        float percentY =
            (localPos.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<WayPointNodes> GetPathNeighbors(WayPointNodes node)
    {
        return node.neighbors;
    }

    public List<WayPointNodes> GetAllNodes()
    {
        List<WayPointNodes> nodes = new List<WayPointNodes>();

        foreach (WayPointNodes node in grid)
        {
            nodes.Add(node);
        }

        return nodes;
    }  
}