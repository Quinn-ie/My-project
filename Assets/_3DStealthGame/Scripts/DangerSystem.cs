using UnityEngine;

public class DangerSystem : MonoBehaviour
{
    public GridManager gridManager;
    public Transform enemy;

    public float dangerRadius = 5f;

    void Update()
    {
        UpdateDanger();
    }

    void UpdateDanger()
    {
        foreach (WayPointNodes node in gridManager.grid)
        {
            node.dangerCost = 0;

            float dist =
                Vector3.Distance(
                    node.worldPosition,
                    enemy.position);

            // Extremely dangerous
            if (dist < 2f)
            {
                node.dangerCost = 1000;
            }
            // Nearby danger
            else if (dist < dangerRadius)
            {
                node.dangerCost = 50;
            }
        }
    }
}