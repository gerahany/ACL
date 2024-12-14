using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GateManager : MonoBehaviour
{
    public GameObject gate; // Reference to the gate GameObject
    public float proximityDistance = 10f; // Distance threshold to track reactivation
    public GateController gateController;
    private GameObject player; // Reference to the player GameObject
    private UnityEngine.AI.NavMeshAgent agent;
    private void Start()
       
    {
        // Find the player object
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found. Make sure it is tagged as 'Player' in the scene. Gate");
            return;
        }
        agent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();

    }

    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            agent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();
            return;
        }
        if (player == null || gate == null || gateController == null)
            return;

        if (!gate.activeSelf)
        {
            BasePlayer basePlayer = player.GetComponent<BasePlayer>();

            StartCoroutine(TransformPlayerAfterDelay(basePlayer, 2f)); // 3-second delay

            float distanceToPlayer = Vector3.Distance(player.transform.position, gateController.transform.position);
            if (distanceToPlayer > gateController.proximityDistance)
            {
                gate.SetActive(true); // Reactivate the gate
                Debug.Log("Gate reactivated by manager.");
            }
        }

        if (gate.activeSelf)
        {
            gateController.CheckProximity(); // Check proximity and handle open/close
        }
    }
    private IEnumerator TransformPlayerAfterDelay(BasePlayer basePlayer, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Set the player's position to the specified coordinates after the delay
        Vector3 targetPosition = new Vector3(458.95f, 16f, 406.82f);
       Teleport(targetPosition);   
 }
    void Teleport(Vector3 targetPosition)
    {
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(targetPosition, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            Debug.Log($"Teleport successful! Adjusted to NavMesh: {hit.position}");
        }
        else
        {
            Debug.LogError("Target position is not on the NavMesh.");
        }
    }

}