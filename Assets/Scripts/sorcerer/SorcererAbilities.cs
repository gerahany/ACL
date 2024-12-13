using UnityEngine;
using System.Collections;
using TMPro;

public class SorcererAbilities : MonoBehaviour
{
    public GameObject fireballPrefab; // Fireball prefab
    public Transform fireballSpawnPoint; // Point from where the fireball is spawned
    public Animator sorcererAnimator; // Animator for casting animation
    public float raycastRange = 100f; // Maximum range for detecting targets
    private Vector3 targetPosition; // Store the position of the valid target
    private bool canCastFireball = true; // Track if the fireball can be cast
    private float fireballCooldown = 1f; // Cooldown time (in seconds)
    private float teleportCooldown = 10f;
    public BasePlayer basePlayer;
    private bool isTeleporting = false; // Flag to track if teleport is activated
    private bool teleportRequested = false; // Flag to track if W + right-click is requested
    public TMP_Text fireballCooldownText;
    public TMP_Text teleportCooldownText;
    public SoundEffectHandler soundEffectHandler;

    void Start()
    {
        fireballCooldownText.text = "OK";
        teleportCooldownText.text = "OK";
        if (basePlayer == null)
    {
        basePlayer = FindObjectOfType<BasePlayer>();
    }
    }
    void Update()
    {
        // Check if "W" is pressed to enable teleportation mode
        if (Input.GetKeyDown(KeyCode.W))
        {
            teleportRequested = true; // W key was pressed, awaiting right-click to teleport
        }

        // Check if right-click is detected
        if (Input.GetMouseButtonDown(1)) // Right-click is button 1
        {
            if (teleportRequested)
            {
                // If W + right-click, trigger teleportation (disable fireball casting)
                StartCoroutine(Teleport());
            }
            else if (canCastFireball) // Fireball casting when not teleporting
            {
                AttemptFireball();
            }
        }
        if (basePlayer != null)
    {
        if (basePlayer.isCoolZero())
        {
            teleportCooldown = 0f;
            fireballCooldown = 0f;
        }
        else
        {
            teleportCooldown = 10f;
            fireballCooldown = 1f;
        }
    }
    else
    {
        Debug.LogWarning("BasePlayer is not assigned!");
    }
    }

    void AttemptFireball()
    {
        // Perform a raycast from the camera to the mouse pointer position
        soundEffectHandler.PlayFireballSound();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange))
        {
            // Check if the hit object is a valid target and not the sorcerer itself
            if ((hit.collider.CompareTag("Demon") || hit.collider.CompareTag("Minion")) && hit.collider.gameObject != gameObject)
            {
                // Save the target position
                targetPosition = hit.point;

                // Calculate direction to the target and rotate the sorcerer
                Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                directionToTarget.y = 0; // Ignore vertical rotation for better control
                transform.forward = directionToTarget; // Rotate the sorcerer to face the target

                // Play the cast animation
                sorcererAnimator.SetTrigger("CastFireball");

                // Spawn the fireball after the animation delay
                Invoke(nameof(SpawnFireball), 0.8f); // Adjust delay as needed

                // Start the cooldown
                StartCoroutine(CooldownFireball());
            }
        }
    }

    void SpawnFireball()
    {
        // Spawn the fireball at the spawn point
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);

        // Ignore collision with the sorcerer
        Collider fireballCollider = fireball.GetComponent<Collider>();
        Collider sorcererCollider = GetComponent<Collider>();
        if (fireballCollider != null && sorcererCollider != null)
        {
            Physics.IgnoreCollision(fireballCollider, sorcererCollider);
        }

        // Calculate the direction to the target
        Vector3 direction = (targetPosition - fireballSpawnPoint.position).normalized;

        // Align the fireball's rotation to its movement direction
        fireball.transform.rotation = Quaternion.LookRotation(direction);

        // Pass the direction to the fireball
        fireball.GetComponent<Fireball>().SetDirection(direction);
    }

    IEnumerator CooldownFireball()
    {
        canCastFireball = false;
        float remainingTime = fireballCooldown;

        while (remainingTime > 0)
        {
            fireballCooldownText.text = $"{remainingTime:F1}s";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        fireballCooldownText.text = "OK";
        canCastFireball = true;
    }

    IEnumerator CooldownTeleport()
    {
        isTeleporting = true;
        teleportRequested = false; // Reset teleport request flag
        float remainingTime = teleportCooldown;

        while (remainingTime > 0)
        {
            // Display cooldown time as whole seconds
            teleportCooldownText.text = $"{Mathf.FloorToInt(remainingTime)}s";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        teleportCooldownText.text = "OK";
        isTeleporting = false;
    }


    IEnumerator Teleport()
    {
        // Disable fireball casting temporarily while teleporting
        if (basePlayer.IsDefensiveUnlocked)
        {
            canCastFireball = false;
            // Simulate teleportation (e.g., move to target position or other logic)
            Debug.Log("Teleporting...");
            yield return new WaitForSeconds(1f); // Adjust duration as needed
            StartCoroutine(CooldownTeleport());
            // Reset after teleportation
            teleportRequested = false; // Reset the teleport request flag
            canCastFireball = true; // Re-enable fireball casting after teleportation
        }

    }
}