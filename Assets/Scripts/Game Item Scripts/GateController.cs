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
        // Find the staged player object
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError(" Player object not found. Make sure it is named 'Player' in the scene. Gate ");
            return;
        }

        // Save the initial position of the gate
        initialGatePosition = transform.position;
        Debug.Log($"Gate initial position saved: {initialGatePosition}");
    }

    private void Update()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(initialGatePosition, player.transform.position);
        Debug.Log($"gate: {isGateOpen}");

        if (distanceToPlayer <= 10)
        {
            Debug.Log($"Player is near the gate.");
            BasePlayer basePlayer = player.GetComponent<BasePlayer>();
            if (basePlayer != null && basePlayer.getrune() >= requiredRuneCount)
            {
                Debug.Log($"Player has enough runes to open the gate.");
                OpenGate(basePlayer);

            }
            else
            {
                CloseGate();
            }
        }
        else
        {
            CloseGate();
        }
    }

    private void OpenGate(BasePlayer basePlayer)
    {
       
        basePlayer.zerorune(); // Reset the player's rune count
        gameObject.SetActive(false); // Make the gate disappear
        Debug.Log("Gate opened!");
    }

    private void CloseGate()
    {
        isGateOpen = false;
        gameObject.SetActive(true); // Make the gate reappear
        Debug.Log("Gate closed!");
    }
}
