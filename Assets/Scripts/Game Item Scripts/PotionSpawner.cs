using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PotionSpawner : MonoBehaviour
{
    public GameObject potionPrefab; // Healing potion prefab
    public Vector3 globalXRange = new Vector3(0, 100); // Global X range (min, max)
    public Vector3 globalZRange = new Vector3(0, 100); // Global Z range (min, max)
    public float yPosition = 16f; // Y position for spawning potions
    public int potionsToSpawn = 50; // Number of potions to spawn
    public float minDistance = 1f; // Min distance between potions
    public float collisionCheckRadius = 0.5f; // Radius to check for collisions

    private List<Vector3> usedPositions = new List<Vector3>(); // Tracks used positions

    void Start()
    {
        SpawnPotionsGlobally();
    }

    void SpawnPotionsGlobally()
    {
        int spawned = 0;

        while (spawned < potionsToSpawn)
        {
            // Generate random spawn position
            float randomX = Random.Range(globalXRange.x, globalXRange.y);
            float randomZ = Random.Range(globalZRange.x, globalZRange.y);
            Vector3 spawnPosition = new Vector3(randomX, yPosition, randomZ);

            // Check if the position is valid
            if (IsPositionValid(spawnPosition) && IsPositionFreeOfObstacles(spawnPosition))
            {
                // Instantiate potion at spawn position
                GameObject potion = Instantiate(potionPrefab, spawnPosition, Quaternion.identity);
                spawned++;

                // Add collider to potion to detect when player collides with it
                Collider potionCollider = potion.AddComponent<SphereCollider>();
                potionCollider.isTrigger = true;
                potion.tag = "Potion"; // Tag it as a "Potion"
            }
        }
    }

    bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 usedPosition in usedPositions)
        {
            if (Vector3.Distance(position, usedPosition) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    bool IsPositionFreeOfObstacles(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, collisionCheckRadius);
        return colliders.Length == 0;
    }

    // Detect when the player enters the potion's trigger area
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BasePlayer player = other.GetComponent<BasePlayer>();
            if (player != null)
            {
                player.AddPotion(); // Add a potion to the player's inventory
                Destroy(gameObject); // Destroy the potion after it's collected
            }
        }
    }
}

