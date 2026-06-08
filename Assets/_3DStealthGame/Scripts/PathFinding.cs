using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public GridManager gridManager;

    public List<WayPointNodes> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        WayPointNodes startNode = gridManager.NodeFromWorldPoint(startPos);
        foreach(WayPointNodes node in gridManager.GetAllNodes())
        {
            node.gCost = int.MaxValue;
            node.hCost = 0;
            node.parent = null;
        }

        WayPointNodes targetNode = gridManager.NodeFromWorldPoint(targetPos);
        if(!startNode.walkable || !targetNode.walkable)
        {
            return null;
        }

        List<WayPointNodes> openSet = new List<WayPointNodes>();
        HashSet<WayPointNodes> closedSet = new HashSet<WayPointNodes>();

        openSet.Add(startNode);

        

        while (openSet.Count > 0)
        {
            WayPointNodes currentNode = openSet[0];

            for(int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost ||
                    (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                List<WayPointNodes> finalPath =
                    RetracePath(startNode, targetNode);

                gridManager.debugPath = finalPath;

                return finalPath;
            }

            foreach(WayPointNodes neighbor in currentNode.neighbors)
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newMovementCostToNeighbor =
                    currentNode.gCost + 
                    GetDistance(currentNode, neighbor) + 
                    neighbor.dangerCost;

                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    List<WayPointNodes> RetracePath(WayPointNodes startNode, WayPointNodes endNode)
    {
        List<WayPointNodes> path = new List<WayPointNodes>();
        WayPointNodes currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    int GetDistance(WayPointNodes a, WayPointNodes b)
    {
        return Mathf.Abs(a.gridX - b.gridX)
            + Mathf.Abs(a.gridY - b.gridY);
    }
}

