using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandMinion2 : MonoBehaviour
{
    public GameObject minionPrefab; // Assign your 3D Minion model prefab in the Inspector
    public GameObject healthBarPrefab; // Assign a prefab for the health bar in the Inspector
    public int minionCount = 15; // Number of minions to spawn
    public float minDistance = 1f; // Minimum distance between minions
    public Vector3 xRange = new Vector3(600, 650, 0); // X range (min, max, unused)
    public Vector3 zRange = new Vector3(400, 500, 0); // Z range (min, max, unused)
    public float yPosition = 17; // Fixed Y position

    private List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        SpawnMinions();
    }

    void SpawnMinions()
    {
        int spawned = 0;

        while (spawned < minionCount)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(xRange.x, xRange.y),
                yPosition,
                Random.Range(zRange.x, zRange.y)
            );

            if (IsPositionValid(randomPosition))
            {
                GameObject minion = Instantiate(minionPrefab, randomPosition, Quaternion.identity);

                // Create and position the health bar
                GameObject healthBar = Instantiate(healthBarPrefab, randomPosition + new Vector3(0, 2.5f, 0), Quaternion.identity);
                healthBar.transform.SetParent(minion.transform); // Make the health bar a child of the minion

                minionhealthbar healthBarScript = healthBar.GetComponent<minionhealthbar>();

                spawned++;
                usedPositions.Add(randomPosition);
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Vector3.Distance(position, usedPosition) < minDistance)
            {
                return false; // Position is too close to an already used position
            }
        }
        return true;
    }
}
