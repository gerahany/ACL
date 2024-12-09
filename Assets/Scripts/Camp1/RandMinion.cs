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
    public GameObject runeFragmentPrefab; // Assign the rune fragment prefab in the Inspector

    public int maxAggressiveMinions = 5; // Maximum number of minions that can attack at once

    private List<Vector3> usedPositions = new List<Vector3>();
    private List<GameObject> minions = new List<GameObject>();
    private GameObject player; // Detected player object
    private List<GameObject> aggressiveMinions = new List<GameObject>();
    private bool runeFragmentSpawned = false; // To track if a rune fragment has been spawned

    void Start()
    {
        SpawnMinions();
    }

    void Update()
    {
        DetectPlayer();
        HandleMinionBehavior();
        CheckAndSpawnRuneFragment();
    }
    void CheckAndSpawnRuneFragment()
    {
        if (!runeFragmentSpawned && AreNoMinionsOrDemonsInCamp())
        {
            // Spawn rune fragment at the center of the camp
            Vector3 runePosition = new Vector3((xRange.x + xRange.y) / 2, 20, (zRange.x + zRange.y) / 2);
            Quaternion runeRotation = Quaternion.Euler(-90f, 0f, 0f); // 90 degrees on the X-axis

            Instantiate(runeFragmentPrefab, runePosition, runeRotation);
            runeFragmentSpawned = true;
        }
    }
    bool AreNoMinionsOrDemonsInCamp()
    {
        // Check for minions and demons in the defined camp radius
        Collider[] minionsInRange = Physics.OverlapBox(
            new Vector3((xRange.x + xRange.y) / 2, yPosition, (zRange.x + zRange.y) / 2),
            new Vector3((xRange.y - xRange.x) / 2, 10f, (zRange.y - zRange.x) / 2),
            Quaternion.identity);

        foreach (Collider collider in minionsInRange)
        {
            if (collider.CompareTag("Minion") || collider.CompareTag("Demon"))
            {
                return false; // If there are any minions or demons, return false
            }
        }

        return true; // No minions or demons found, return true
    }

   
    public void RemoveAggressiveMinion(GameObject minion)
    {
        if (aggressiveMinions.Contains(minion))
        {
            aggressiveMinions.Remove(minion);
            Debug.Log($"{minion.name} removed from aggressive list.");

        }
        if (minions.Contains(minion))
        {
            minions.Remove(minion);
            Debug.Log($"{minion.name} removed from aggressive list.");

        }
        AddAggressiveMinion();
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
        if (player == null)
        {
            Collider[] colliders = Physics.OverlapBox(
                new Vector3((xRange.x + xRange.y) / 2, yPosition, (zRange.x + zRange.y) / 2),
                new Vector3((xRange.y - xRange.x) / 2, 10f, (zRange.y - zRange.x) / 2),
                Quaternion.identity);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Player")) // Check for the player by tag
                {
                    player = collider.gameObject; // Detect player
                    return;
                }
            }
        }
        else
        {
            // If player is already detected, check if they're still within the zone
            if (!IsPlayerInRange())
            {
                player = null; // Reset player if outside the defined range
            }
        }
    }

    bool IsPlayerInRange()
    {
        if (player == null) return false;

        Vector3 playerPos = player.transform.position;
        return playerPos.x >= xRange.x && playerPos.x <= xRange.y && playerPos.z >= zRange.x && playerPos.z <= zRange.y;
    }

    void HandleMinionBehavior()
    {
        if (player != null && IsPlayerInRange()) // Only handle minions if player is within range
        {
            // Ensure aggressive minions count is checked and updated regularly
            AddAggressiveMinion(); // Ensure we try to add new aggressive minions if space is available

            // Go through all minions and make them aggressive if needed
            foreach (GameObject minion in minions)
            {
                MinionBehavior behavior = minion.GetComponent<MinionBehavior>();

                if (behavior != null && !aggressiveMinions.Contains(minion) && aggressiveMinions.Count < maxAggressiveMinions)
                {
                    aggressiveMinions.Add(minion);
                    behavior.SetAggressive(player);
                }
            }
        }
        else
        {
            // No player detected or player out of range, reset all behaviors
            foreach (GameObject minion in minions)
            {
                MinionBehavior behavior = minion.GetComponent<MinionBehavior>();
                if (behavior != null)
                {
                    behavior.ReturnToCamp();
                }
            }
            aggressiveMinions.Clear();
        }
    }

    // This method ensures that we add a minion to the aggressive list when there's space
    void AddAggressiveMinion()
    {
        if (aggressiveMinions.Count < maxAggressiveMinions)
        {
            foreach (GameObject minion in minions)
            {
                MinionBehavior behavior = minion.GetComponent<MinionBehavior>();
                if (behavior != null && !aggressiveMinions.Contains(minion))
                {
                    aggressiveMinions.Add(minion);
                    behavior.SetAggressive(player);
                    return; // Exit once a minion is added
                }
            }
        }
    }
}