using System.Collections.Generic;
using UnityEngine;

public class DemonSpawner : MonoBehaviour
{

    public GameObject demonPrefab; // Assign your 3D Minion model prefab in the Inspector
    public GameObject healthBarPrefab; // Assign a prefab for the health bar in the Inspector
    public int demonCount = 2; // Number of demons to spawn
    public float minDistance = 1f; // Minimum distance between demons
    public Vector3 xRange = new Vector3(600, 650, 0); // X range (min, max, unused)
    public Vector3 zRange = new Vector3(400, 500, 0); // Z range (min, max, unused)
    public float yPosition = 17; // Fixed Y position

    public int maxAggressiveDemons = 1; // Maximum number of demons that can attack at once

    private List<Vector3> usedPositions = new List<Vector3>();
    private List<GameObject> demons = new List<GameObject>();
    private GameObject player; // Detected player object
    private List<GameObject> aggressiveDemons = new List<GameObject>();

    void Start()
    {
        SpawnDemons();
    }

    void Update()
    {
        DetectPlayer();
        
            HandleDemonBehavior();
        

        
            HanldenotAgg();
        



    }
    public void RemoveAggressiveDemon(GameObject demon)
    {
        if (aggressiveDemons.Contains(demon))
        {
            aggressiveDemons.Remove(demon);

        }
        if (demons.Contains(demon))
        {
            demons.Remove(demon);

        }
        AddAggressiveDemon();
    }

    void SpawnDemons()
    {
        int spawned = 0;

        while (spawned < demonCount)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(xRange.x, xRange.y),
                yPosition,
                Random.Range(zRange.x, zRange.y)
            );

            if (IsPositionValid(randomPosition))
            {
                GameObject demon = Instantiate(demonPrefab, randomPosition, Quaternion.identity);

                // Create and position the health bar
                GameObject healthBar = Instantiate(healthBarPrefab, randomPosition + new Vector3(0, 2.5f, 0), Quaternion.identity);
                healthBar.transform.SetParent(demon.transform); // Make the health bar a child of the demon
                demonhealthbar healthBarScript = demon.GetComponent<demonhealthbar>();
                if (healthBarScript != null)
                {
                    healthBarScript.healthBarParent = healthBar.transform; // Pass the health bar parent
                }
                demon.GetComponent<DemonBehavior>().Initialize(this); // Initialize DemonBehavior
                demons.Add(demon);

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


    private float rotationTimer = 0f; // Timer to track elapsed time

    void HandleDemonBehavior()
    {
        if (player != null && IsPlayerInRange()) // Only handle demons if player is within range
        {
            // Ensure aggressive demons count is checked and updated regularly
            AddAggressiveDemon(); // Ensure we try to add new aggressive demons if space is available

            // Go through all demons and make them aggressive if needed
            foreach (GameObject demon in demons)
            {
                DemonBehavior behavior = demon.GetComponent<DemonBehavior>();
                

                if (behavior != null && !aggressiveDemons.Contains(demon) && aggressiveDemons.Count < maxAggressiveDemons)
                {
                    aggressiveDemons.Add(demon);
                    behavior.SetAggressive(player);
                }
            }
        }
        else
        {
            // No player detected or player out of range, reset all behaviors
            foreach (GameObject demon in demons)
            {

                DemonBehavior behavior = demon.GetComponent<DemonBehavior>();
                if (behavior != null)
                {
                    if (aggressiveDemons.Contains(demon))
                        behavior.ReturnToCamp();
                }

            }
            aggressiveDemons.Clear();
        }
    }

    // This method ensures that we add a demon to the aggressive list when there's space
    void AddAggressiveDemon()
    {
        if (aggressiveDemons.Count < maxAggressiveDemons)
        {
            foreach (GameObject demon in demons)
            {
                DemonBehavior behavior = demon.GetComponent<DemonBehavior>();
                if (behavior != null && !aggressiveDemons.Contains(demon))
                {
                    aggressiveDemons.Add(demon);
                    behavior.SetAggressive(player);
                    return; // Exit once a demon is added
                }
            }
        }
    }
    void HanldenotAgg()
    {
        foreach (GameObject demon in demons)
        {
            if (!IsPlayerInRange())
            {
                // Ensure demon stays within camp bounds and rotates randomly
                Vector3 demonPosition = demon.transform.position;
                demonPosition.x = Mathf.Clamp(demonPosition.x, xRange.x, xRange.y);
                demonPosition.z = Mathf.Clamp(demonPosition.z, zRange.x, zRange.y);
                demon.transform.position = demonPosition;

                // Update rotation every 3 seconds if the demon is not aggressive
                rotationTimer += Time.deltaTime;
                if (rotationTimer >= 3f)
                {
                    rotationTimer = 0f; // Reset the timer
                    float randomRotationY = Random.Range(0, 180);
                    demon.transform.rotation = Quaternion.Euler(0, randomRotationY, 0);
                }
            }
            else if (!aggressiveDemons.Contains(demon) && IsPlayerInRange())
            {

                // Ensure demon stays within camp bounds and rotates randomly
                Vector3 demonPosition = demon.transform.position;
                demonPosition.x = Mathf.Clamp(demonPosition.x, xRange.x, xRange.y);
                demonPosition.z = Mathf.Clamp(demonPosition.z, zRange.x, zRange.y);
                demon.transform.position = demonPosition;

                // Update rotation every 3 seconds if the demon is not aggressive
                rotationTimer += Time.deltaTime;
                if (rotationTimer >= 3f)
                {
                    rotationTimer = 0f; // Reset the timer
                    float randomRotationY = Random.Range(0, 360);
                    demon.transform.rotation = Quaternion.Euler(0, randomRotationY, 0);
                }
            }
            }


    }
}