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

    public float campRadius = 35f; // Detection range for the player
    public float followRadius = 70f; // Radius at which demons stop following the player
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

    private float rotationTimer = 0f; // Timer to track elapsed time

    void HandleDemonBehavior()
    {
        foreach (GameObject demon in demons)
        {
            DemonBehavior behavior = demon.GetComponent<DemonBehavior>();

            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(demon.transform.position, player.transform.position);

                if (distanceToPlayer <= campRadius && aggressiveDemons.Count < maxAggressiveDemons)
                {
                    if (!aggressiveDemons.Contains(demon))
                    {
                        aggressiveDemons.Add(demon); // Add to aggressive list
                        behavior.SetAggressive(player);
                    }
                }
                else if (distanceToPlayer > followRadius)
                {
                    behavior.ReturnToCamp();
                    aggressiveDemons.Remove(demon); // Remove from aggressive list
                }
            }
            else
            {
                if (aggressiveDemons.Contains(demon))
                {
                    aggressiveDemons.Remove(demon); // Reset all behaviors
                }

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
