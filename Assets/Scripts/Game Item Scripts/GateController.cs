using UnityEngine;

public class GateController : MonoBehaviour
{
    public float proximityDistance = 10f; // Distance threshold for "near the gate"
    public int requiredRuneCount = 3; // The number of runes required to open the gate

    private GameObject player; // Reference to the player GameObject
    private bool isGateOpen = false; // Track the state of the gate
    private Vector3 initialGatePosition; // Store the initial position of the gate

    private void Start()
    {
        // Find the player object
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player object not found. Make sure it is named 'Player' in the scene.");
            return;
        }

        // Save the initial position of the gate
        initialGatePosition = transform.position;
        Debug.Log($"Gate initial position saved: {initialGatePosition}");
    }

    public void CheckProximity()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(player.transform.position, initialGatePosition);

        if (distanceToPlayer <= proximityDistance && !isGateOpen)
        {
            BasePlayer basePlayer = player.GetComponent<BasePlayer>();
            if (basePlayer != null && basePlayer.getrune() >= requiredRuneCount)
            {
                OpenGate(basePlayer);
            }
        }
        else if (distanceToPlayer > proximityDistance && isGateOpen)
        {
            CloseGate();
        }
    }

    private void OpenGate(BasePlayer basePlayer)
    {
        if (isGateOpen) return;

        isGateOpen = true;
        basePlayer.zerorune(); // Reset the player's rune count
        gameObject.SetActive(false); // Make the gate disappear
        Debug.Log("Gate opened!");
    }

    private void CloseGate()
    {
        if (!isGateOpen) return;

        isGateOpen = false;
        gameObject.SetActive(true); // Make the gate reappear
        Debug.Log("Gate closed!");
    }
}