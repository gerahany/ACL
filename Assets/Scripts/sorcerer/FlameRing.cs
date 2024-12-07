using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameRing : MonoBehaviour
{
    private float duration; // How long the ring exists
    private List<GameObject> enemiesInside = new List<GameObject>(); // Track enemies in the ring

    public void Initialize(float duration)
    {
        this.duration = duration;

        // Start the ring's lifetime countdown
        StartCoroutine(HandleLifetime());
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is a Demon or Minion
        if (other.CompareTag("Demon") || other.CompareTag("Minion"))
        {
            Debug.Log(other.name + " entered the flame ring.");
            enemiesInside.Add(other.gameObject); // Add to the list of tracked enemies
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object is a Demon or Minion
        if (other.CompareTag("Demon") || other.CompareTag("Minion"))
        {
            Debug.Log(other.name + " exited the flame ring.");
            enemiesInside.Remove(other.gameObject); // Remove from the list of tracked enemies
        }
    }

    IEnumerator HandleLifetime()
    {
        // Wait for the ring's duration to complete
        yield return new WaitForSeconds(duration);

        // Destroy the flame ring GameObject
        Destroy(gameObject);
        Debug.Log("Flame ring destroyed.");
    }
}
