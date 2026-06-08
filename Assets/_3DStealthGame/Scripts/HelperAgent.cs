using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperAgent : MonoBehaviour
{
    public PathFinding pathfinding;

    public Transform player;
    public Transform enemy;
    public Transform exitPoint;

    public float moveSpeed = 3f;
    public float repathDistance = 3f;

    List<WayPointNodes> currentPath;

    bool activeGuide = false;
    bool finishedMoving = false;
    Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }   
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H) && !activeGuide)
        {
            ActivateGuide();
        }

        if(activeGuide && finishedMoving)
        {
            float distance =
                Vector3.Distance(
                    transform.position,
                    player.position);

            if(distance < 1.5f)
            {
                HideGuide();
            }
        }

        if (activeGuide)
        {
            float distToEnemy = Vector3.Distance(
                transform.position,
                enemy.position);

            if (distToEnemy < repathDistance)
            {
                RecalculatePath();
            }
        }
    }

    void ActivateGuide()
    {
        currentPath =
            pathfinding.FindPath(player.position, exitPoint.position);

        if (currentPath == null || currentPath.Count < 2)
        {
            return;
        }

        int index = Mathf.Min(10, currentPath.Count - 1);
        transform.position = currentPath[1].worldPosition;
        int pathLength = Mathf.Min(10, currentPath.Count - 1);
        List<WayPointNodes> shortPath = currentPath.GetRange(1, pathLength);

        activeGuide = true;
        finishedMoving = false;

        foreach (Renderer r in renderers)
            r.enabled = true;

        StopAllCoroutines();
        StartCoroutine(FollowPath());
    }

   IEnumerator FollowPath()
    {
        int nodeTravveled = 0;
        int maxNodes = 10;

        while (activeGuide)
        {
            if (nodeTravveled >= maxNodes)
            {
                finishedMoving = true;
                yield break;
            }

            if (currentPath == null || currentPath.Count == 0)
            {
                yield return null;
                continue;
            }

            WayPointNodes node = currentPath[0];

            while (Vector3.Distance(
                transform.position,
                node.worldPosition) > 0.05f)
            {
                if (!activeGuide || currentPath == null || currentPath.Count == 0)
                    yield break;

                node = currentPath[0];

                transform.position =
                    Vector3.MoveTowards(
                        transform.position,
                        node.worldPosition,
                        moveSpeed * Time.deltaTime);

                yield return null;
            }
            transform.position = node.worldPosition;
            
            currentPath.RemoveAt(0);
            nodeTravveled++;

            yield return null;
        }

        finishedMoving = true;
    }


    void RecalculatePath()
    {
        currentPath =
            pathfinding.FindPath(
                transform.position,
                exitPoint.position);
    }

    void HideGuide()
    {
        StopAllCoroutines();
        activeGuide = false;
        foreach(Renderer renderer in renderers) 
        {
            renderer.enabled = false;
        }
    }
}