using System.Collections.Generic;
using UnityEngine;

public class DemonSpawner3 : MonoBehaviour
{
    public GameObject demonPrefab; // Assign your Demon prefab here
    public GameObject healthBarPrefab;
    public int demonCount = 2; // Number of demons to spawn
    public float minDistance = 5f; // Minimum distance between demons and minions
    public Vector3 xRange = new Vector3(500, 700, 0); // X range (min, max)
    public Vector3 zRange = new Vector3(450, 500, 0); // Z range (min, max)
    public float yPosition = 16f; // Fixed Y position for spawning
    public float moveSpeed = 3f; // Movement speed of the demon
    public float rotationSpeed = 200f; // Rotation speed of the demon
    public float moveInterval = 1f; // Time interval to change direction
    public float maxRotationAngle = 30f; // Max angle for random rotation to avoid large rotations

    private List<Vector3> usedPositions = new List<Vector3>();
    private List<GameObject> demons = new List<GameObject>(); // Store the actual demons
    private float rotationCooldown = 0f; // Cooldown for random rotations

    void Start()
    {
        SpawnDemons();
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

            // Ensure the random position doesn't overlap with minions or other demons
            if (IsPositionValid(randomPosition))
            {
                GameObject demon = Instantiate(demonPrefab, randomPosition, Quaternion.identity);
                GameObject healthBar = Instantiate(healthBarPrefab, randomPosition + new Vector3(0, 2.5f, 0), Quaternion.identity);
                healthBar.transform.SetParent(demon.transform); // Make the health bar a child of the demon
                // Add the demon to the list to keep track of them
                demons.Add(demon);

                // Play walking animation for the demon
                Animator animator = demon.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Play("Walking"); // Replace "Walking" with your actual walking animation state
                }

                // Add this position to the used positions list for validation
                usedPositions.Add(randomPosition);
                spawned++;
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        // Check against all used positions (Demons and Minions)
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Vector3.Distance(position, usedPosition) < minDistance)
            {
                return false; // Too close to an existing object (Minion or Demon)
            }
        }
        return true;
    }

    void Update()
    {
        MoveDemons();
    }

    void MoveDemons()
    {
        foreach (GameObject demon in demons)
        {
            // Get the current position of the demon
            Vector3 currentPosition = demon.transform.position;

            // Check if it's time to rotate the demon (after a cooldown period)
            if (rotationCooldown <= 0f)
            {
                // Apply a small random rotation around the Y-axis
                float randomYRotation = Random.Range(-maxRotationAngle, maxRotationAngle); // Smaller rotation angles
                demon.transform.Rotate(0f, randomYRotation, 0f, Space.Self);

                // Reset the cooldown to wait for the next rotation
                rotationCooldown = Random.Range(1f, 3f); // Random interval between 1 to 3 seconds
            }
            else
            {
                // Decrease the cooldown time
                rotationCooldown -= Time.deltaTime;
            }

            // Calculate the movement direction based on the current rotation
            Vector3 forwardDirection = demon.transform.forward;

            // Check for collision with other demons or minions
            AvoidCollisions(demon, forwardDirection);

            // Move the demon in the direction it's facing
            Vector3 newPosition = currentPosition + forwardDirection * moveSpeed * Time.deltaTime;

            // Ensure the demon stays within bounds (xRange and zRange)
            newPosition.x = Mathf.Clamp(newPosition.x, xRange.x, xRange.y);
            newPosition.z = Mathf.Clamp(newPosition.z, zRange.x, zRange.y);

            // Apply the new position to the demon
            demon.transform.position = newPosition;
        }
    }

    // Avoid collisions with other demons and minions
    void AvoidCollisions(GameObject demon, Vector3 forwardDirection)
    {
        // Get all minions in the scene
        GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");

        // Avoid collisions with other demons
        foreach (GameObject otherDemon in demons)
        {
            if (otherDemon != demon && Vector3.Distance(demon.transform.position, otherDemon.transform.position) < minDistance)
            {
                // Rotate away from the other demon
                Vector3 avoidanceDirection = (demon.transform.position - otherDemon.transform.position).normalized;
                demon.transform.Rotate(0f, Random.Range(-maxRotationAngle, maxRotationAngle), 0f, Space.Self);
                return; // Apply one rotation to avoid collision and exit early
            }
        }

        // Avoid minions as well
        foreach (GameObject minion in minions)
        {
            if (Vector3.Distance(demon.transform.position, minion.transform.position) < minDistance)
            {
                // Rotate away from the minion
                Vector3 avoidanceDirection = (demon.transform.position - minion.transform.position).normalized;
                demon.transform.Rotate(0f, Random.Range(-maxRotationAngle, maxRotationAngle), 0f, Space.Self);
                return; // Apply one rotation to avoid collision and exit early
            }
        }
    }
}
