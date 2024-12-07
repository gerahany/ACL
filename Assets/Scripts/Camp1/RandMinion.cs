using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandMinion : MonoBehaviour
{
    public GameObject minionPrefab; // Assign your 3D Minion model prefab in the Inspector
    public GameObject healthBarPrefab; // Assign a prefab for the health bar in the Inspector
    public int minionCount = 15; // Number of minions to spawn
    public float minDistance = 1f; // Minimum distance between minions
    public Vector3 xRange = new Vector3(600, 650, 0); // X range (min, max, unused)
    public Vector3 zRange = new Vector3(400, 500, 0); // Z range (min, max, unused)
    public float yPosition = 17; // Fixed Y position

    public float campRadius = 35f; // Detection range for the player
    public float followRadius = 70f; // Radius at which minions stop following the player
    public int maxAggressiveMinions = 5; // Maximum number of minions that can attack at once

    private List<Vector3> usedPositions = new List<Vector3>();
    private List<GameObject> minions = new List<GameObject>();
    private GameObject player; // Detected player object
    private List<GameObject> aggressiveMinions = new List<GameObject>();

    void Start()
    {
        SpawnMinions();
    }

    void Update()
    {
        DetectPlayer();
        HandleMinionBehavior();
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
                minionhealthbar healthBarScript = minion.GetComponent<minionhealthbar>();
                if (healthBarScript != null)
                {
                    healthBarScript.healthBarParent = healthBar.transform; // Pass the health bar parent
                }
                minion.GetComponent<MinionBehavior>().Initialize(this); // Initialize MinionBehavior
                minions.Add(minion);

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

    void DetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, campRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player")) // Check for the player by tag
            {
                player = collider.gameObject; // Detect player
                return;
            }
        }
        player = null; // No player detected
    }

    void HandleMinionBehavior()
    {
        foreach (GameObject minion in minions)
        {
            MinionBehavior behavior = minion.GetComponent<MinionBehavior>();

            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(minion.transform.position, player.transform.position);

                if (distanceToPlayer <= campRadius && aggressiveMinions.Count < maxAggressiveMinions)
                {
                    if (!aggressiveMinions.Contains(minion))
                    {
                        aggressiveMinions.Add(minion); // Add to aggressive list
                        behavior.SetAggressive(player);
                    }
                }
                else if (distanceToPlayer > followRadius)
                {
                    behavior.ReturnToCamp();
                    aggressiveMinions.Remove(minion); // Remove from aggressive list
                }
            }
            else
            {
                behavior.ReturnToCamp();
                aggressiveMinions.Remove(minion); // Reset all behaviors
            }
        }
    }
}
