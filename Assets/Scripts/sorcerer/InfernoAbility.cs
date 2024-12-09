using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfernoAbility : MonoBehaviour
{
    public float cooldown = 15f; // Cooldown duration
    public GameObject flameRingPrefab; // Prefab for the flame ring
    public float ringDuration = 5f; // Duration of the flame ring
    private bool canUseInferno = true; // Can the ability be used?
    private bool isSelectingPosition = false; // Is the player selecting a position?
    public BasePlayer basePlayer;
    public TMP_Text infernoCooldownText;

    void Start()
    {
        infernoCooldownText.text = "OK";
    }

    void Update()
    {
        // Activate Inferno when pressing "E" and not in cooldown
        if (Input.GetKeyDown(KeyCode.E) && canUseInferno && !isSelectingPosition && basePlayer.IsUltimateUnlocked)
        {
            StartSelectingPosition();
        }

        // Check for right-click to confirm the target position
        if (isSelectingPosition && Input.GetMouseButtonDown(1)) // Right-click
        {
            CastInferno();
        }
    }

    void StartSelectingPosition()
    {
        isSelectingPosition = true;
        Debug.Log("Select a position to cast Inferno using right-click.");
    }

    void CastInferno()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            // Spawn the flame ring
            GameObject flameRing = Instantiate(flameRingPrefab, targetPosition, Quaternion.identity);

            // Initialize the flame ring
            FlameRing ringScript = flameRing.GetComponent<FlameRing>();
            if (ringScript != null)
            {
                ringScript.Initialize(ringDuration);
            }

            Debug.Log("Inferno cast at position: " + targetPosition);

            // Start cooldown
            StartCoroutine(StartCooldown());
        }
        else
        {
            Debug.Log("Invalid target position.");
        }

        isSelectingPosition = false;
    }

    IEnumerator StartCooldown()
    {
        canUseInferno = false;
        Debug.Log("Inferno cooldown started.");
        yield return new WaitForSeconds(cooldown);
        float remainingTime = cooldown;

        while (remainingTime > 0)
        {
            // Update cooldown text dynamically with remaining seconds
            infernoCooldownText.text = $"{Mathf.FloorToInt(remainingTime)}s";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // Cooldown is complete
        infernoCooldownText.text = "OK";
        canUseInferno = true;
        Debug.Log("Inferno is ready.");
        canUseInferno = true;
    }
}