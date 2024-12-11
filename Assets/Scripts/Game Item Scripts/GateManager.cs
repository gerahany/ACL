using UnityEngine;

public class GateManager : MonoBehaviour
{
    public GameObject gate; // Reference to the gate GameObject
    public float proximityDistance = 10f; // Distance threshold to track reactivation
    public GateController gateController;
    private GameObject player; // Reference to the player GameObject

    private void Start()
    {
        // Find the player object
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found. Make sure it is tagged as 'Player' in the scene.");
            return;
        }
    }

    private void Update()
    {
        if (player == null || gate == null || gateController == null)
            return;

        if (!gate.activeSelf)
        {
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
}