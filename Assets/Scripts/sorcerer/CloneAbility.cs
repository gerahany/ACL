using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CloneAbility : MonoBehaviour
{
    public GameObject clonePrefab; // Prefab of the clone
    public float cloneDuration = 5f; // Time the clone lasts before exploding
    public float explosionRadius = 5f; // Radius of the explosion effect
    public float explosionDamage = 50f; // Damage dealt by the explosion
    public float cooldown = 10f; // Cooldown for the ability
    public GameObject explosionEffectPrefab;
    private bool canUseClone = true; // Tracks if the ability is on cooldown
    private bool isSelectingPosition = false; // Tracks if the player is selecting the position

    private bool isCooldownActive = false;

   void Update()
{
    // Activate clone ability when pressing "Q"
    if (Input.GetKeyDown(KeyCode.Q) && canUseClone && !isCooldownActive && !isSelectingPosition)
    {
        isSelectingPosition = true;
    }

    // If the player is selecting a position, listen for right-click to create the clone
    if (isSelectingPosition && Input.GetMouseButtonDown(1) && canUseClone && !isCooldownActive) // Right-click (button 1)
    {
        Debug.Log(canUseClone);
        TryCreateClone();
    }
}


    // void StartSelectingPosition()
    // {
    //     isSelectingPosition = true;
    //     Debug.Log("Select a position for the clone using right-click.");
    // }

    void TryCreateClone()
{
    // Make sure you can't try to create a clone if it's on cooldown
    if (!canUseClone) return;

    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit))
    {
        Vector3 targetPosition = hit.point;

        // Check if the position is walkable
        if (IsWalkable(targetPosition))
        {
            // Create the clone
            StartCoroutine(CreateClone(targetPosition));

            // Start cooldown
            StartCoroutine(CloneCooldown());
        }
        else
        {
            Debug.Log("Target position is not walkable.");
        }
    }

    // End the position selection
    isSelectingPosition = false;
}


    bool IsWalkable(Vector3 targetPosition)
    {
        // Use NavMesh.SamplePosition to check if the position is walkable
        NavMeshHit hit;
        return NavMesh.SamplePosition(targetPosition, out hit, 1f, NavMesh.AllAreas);
    }

IEnumerator CreateClone(Vector3 position)
{
    
    // Instantiate the clone at the specified position
    GameObject clone = Instantiate(clonePrefab, position, Quaternion.identity);

    // Set the clone's parent to null, ensuring that it is not part of any hierarchy that might affect camera focus
    clone.transform.SetParent(null);

    // Ensure the clone doesn't inherit any movement from the player (check if your player is set as the parent)
    clone.transform.position = position; // explicitly setting the position to avoid unexpected camera behavior

    // Enable the clone's functionality (e.g., make enemies target it)
    clone.tag = "Clone";

    Debug.Log("Clone created at position: " + position);
    


    // Wait for the clone's duration
    yield return new WaitForSeconds(cloneDuration);

    // Trigger the explosion effect
    Explode(clone.transform.position);

    // Destroy the clone
    Destroy(clone);
}


  void Explode(Vector3 position)
    {
        Debug.Log("Explosion triggered at position: " + position);

        // Trigger legacy explosion effect at the specified position
        GameObject explosionEffect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
        explosionEffect.SetActive(true);

        // Detect enemies within the explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(position, explosionRadius);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Demon") || collider.CompareTag("Minion"))
            {
                // Apply damage logic to the enemies (e.g., damage them)
                Debug.Log(collider.name + " hit by explosion!");
                // Example: collider.GetComponent<Enemy>().TakeDamage(explosionDamage);
            }
        }

        // Destroy the explosion effect after it finishes (use the duration from the effect's animation or lifecycle)
        Destroy(explosionEffect, 2f); // Adjust this value based on the lifetime of the explosion effect
    }

IEnumerator CloneCooldown()
{
   
    Debug.Log(canUseClone);
    canUseClone = false;
    isCooldownActive = true;  // Start tracking cooldown
    Debug.Log(canUseClone);
    yield return new WaitForSeconds(cooldown);
    canUseClone = true;
    isCooldownActive = false; // End cooldown tracking
    Debug.Log(canUseClone);
   
}
}
