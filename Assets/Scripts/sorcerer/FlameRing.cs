using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameRing : MonoBehaviour
{
    public float ringRadius = 5f; // Radius of the flame ring
    public float initialDamage = 10f; // Initial damage dealt to enemies in the ring
    public float damagePerSecond = 2f; // Continuous damage dealt per second
    private float duration; // Duration of the flame ring

    private List<GameObject> enemiesInsideRing = new List<GameObject>(); // Track enemies inside the ring

    public void Initialize(float ringDuration)
    {
        duration = ringDuration;

        // Deal initial damage when the ring is spawned
        ApplyInitialDamage();

        // Start continuous damage logic
        StartCoroutine(DealContinuousDamage());

        // Destroy the ring after its duration
        Destroy(gameObject, duration);
    }

    void ApplyInitialDamage()
    {
        // Find all colliders within the ring's radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, ringRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Minion") || collider.CompareTag("Demon"))
            {
                // Apply initial damage to the enemy
                ApplyDamageToEnemy(collider.gameObject, (int)initialDamage);

                // Add the enemy to the list of those inside the ring
                if (!enemiesInsideRing.Contains(collider.gameObject))
                {
                    enemiesInsideRing.Add(collider.gameObject);
                    Debug.Log($"{collider.gameObject.name} entered the flame ring.");
                }
            }
        }
    }

    IEnumerator DealContinuousDamage()
    {
        while (duration > 0)
        {
            yield return new WaitForSeconds(1f); // Apply damage every second

            for (int i = enemiesInsideRing.Count - 1; i >= 0; i--) // Iterate in reverse to avoid issues when removing elements
            {
                GameObject enemy = enemiesInsideRing[i];
                if (enemy != null)
                {
                    ApplyDamageToEnemy(enemy, (int)damagePerSecond);
                }
                else
                {
                    // If the enemy is destroyed or null, remove it from the list
                    enemiesInsideRing.RemoveAt(i);
                }
            }

            duration -= 1f;
        }
    }

    private void ApplyDamageToEnemy(GameObject enemy, int damage)
    {
        // Check for minion or demon health
        minionhealthbar minionHealth = enemy.GetComponent<minionhealthbar>();
        demonhealthbar demonHealth = enemy.GetComponent<demonhealthbar>();

        if (minionHealth != null)
        {
            minionHealth.TakeDamage(damage);
            Debug.Log($"Applied {damage} damage to minion: {enemy.name}");
        }
        else if (demonHealth != null)
        {
            demonHealth.TakeDamage(damage);
            Debug.Log($"Applied {damage} damage to demon: {enemy.name}");
        }
        else
        {
            Debug.LogWarning($"No health script found on {enemy.name}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Minion") || other.CompareTag("Demon")) && !enemiesInsideRing.Contains(other.gameObject))
        {
            enemiesInsideRing.Add(other.gameObject);
            Debug.Log($"{other.gameObject.name} entered the flame ring.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("hiinf");
        if ((other.CompareTag("Minion") || other.CompareTag("Demon")) && enemiesInsideRing.Contains(other.gameObject))
        {
            enemiesInsideRing.Remove(other.gameObject);
            Debug.Log($"{other.gameObject.name} exited the flame ring.");
        }
    }
}
