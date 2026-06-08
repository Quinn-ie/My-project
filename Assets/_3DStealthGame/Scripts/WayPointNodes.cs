using System.Collections.Generic;
using UnityEngine;

public class WayPointNodes
{
    public bool walkable;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;
    public int gCost;
    public int hCost;
    
    public int dangerCost;
    public WayPointNodes parent;

    public List<WayPointNodes> neighbors = new List<WayPointNodes>();
    public int fCost => gCost + hCost;

    public WayPointNodes(bool walkable, Vector3 worldPosition, int x, int y)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        gridX = x;
        gridY = y;
    }
}