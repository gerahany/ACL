using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ArrowsAbility : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float arrowRainDuration = 3f;
    public float arrowSpawnInterval = 0.2f;
    public float arrowDamageRadius = 5f;
    public LayerMask targetLayer;
    public int arrowDamage = 10;
    public float rotationAngle = -90f;
    private bool canUseAbility = true;
    private bool isCooldown = false;
    public float abilityCooldown = 10f;
    public TMP_Text cooldownText;
    private float currentCooldownTime = 0f;
    public BasePlayer basePlayer;
    public GameObject arrowRingPrefab;
    private HashSet<GameObject> alreadyDamagedObjects = new HashSet<GameObject>();

    void Start()
    {
        cooldownText.text = "OK";
    }
    void Update()
    {
        // Only activate ability if no other ability is active
        if (Input.GetKeyDown(KeyCode.E) && canUseAbility && !AbilityManager.IsAbilityActive() && basePlayer.IsUltimateUnlocked)
        {
            StartSelectingPosition();
        }
        if (isCooldown)
        {
            currentCooldownTime += Time.deltaTime; // Increase cooldown time

            float timeLeft = abilityCooldown - currentCooldownTime; // Calculate remaining time

            // Update the TMP text to show the remaining cooldown time
            cooldownText.text = Mathf.Ceil(timeLeft) + "s"; // Round and show seconds

            if (currentCooldownTime >= abilityCooldown)
            {
                canUseAbility = true; // Reset ability to be ready
                currentCooldownTime = 0f; // Reset cooldown time
                cooldownText.text = "OK"; // Show "OK" when cooldown is complete
                Debug.Log("Shower of Arrows ability is ready.");
            }
        }
    }

    void StartSelectingPosition()
    {
        Debug.Log("Select a position to use Shower of Arrows with right-click.");
        StartCoroutine(WaitForPositionSelection());
    }

    IEnumerator WaitForPositionSelection()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 targetPosition = hit.point;
                    Debug.Log("Selected position for Shower of Arrows: " + targetPosition);

                    GameObject arrowRing = CreateArrowRing(targetPosition);
                    StartCoroutine(ActivateShowerOfArrows(targetPosition, arrowRing));
                    break;
                }
            }
            yield return null;
        }
    }

    GameObject CreateArrowRing(Vector3 targetPosition)
    {
        GameObject arrowRing = Instantiate(arrowRingPrefab, targetPosition, Quaternion.identity);
        arrowRing.transform.position = new Vector3(targetPosition.x, targetPosition.y + 0.5f, targetPosition.z);
        return arrowRing;
    }

    IEnumerator ActivateShowerOfArrows(Vector3 targetPosition, GameObject arrowRing)
    {
        AbilityManager.SetAbilityActive(true); // Mark ability as active
        canUseAbility = false;
        float elapsedTime = 0f;

        alreadyDamagedObjects.Clear();

        while (elapsedTime < arrowRainDuration)
        {
            SpawnArrow(targetPosition);
            elapsedTime += arrowSpawnInterval;
            yield return new WaitForSeconds(arrowSpawnInterval);
        }

        Destroy(arrowRing);
        AbilityManager.SetAbilityActive(false); // Mark ability as inactive

        isCooldown=true;
        yield return new WaitForSeconds(abilityCooldown);
        isCooldown=false;
        canUseAbility = true;

        Debug.Log("Shower of Arrows ability is ready.");
    }

    void SpawnArrow(Vector3 targetPosition)
    {
        Vector3 randomPosition = targetPosition + Random.insideUnitSphere * arrowDamageRadius;
        randomPosition.y = targetPosition.y;

        GameObject arrow = Instantiate(arrowPrefab, new Vector3(randomPosition.x, targetPosition.y + 10f, randomPosition.z), Quaternion.identity);
        arrow.transform.rotation *= Quaternion.Euler(0f, 0f, rotationAngle);
        StartCoroutine(DropArrow(arrow, targetPosition));
    }

    IEnumerator DropArrow(GameObject arrow, Vector3 targetPosition)
    {
        float fallSpeed = 10f;
        Vector3 startPosition = arrow.transform.position;

        while (arrow.transform.position.y > targetPosition.y)
        {
            arrow.transform.position = Vector3.MoveTowards(arrow.transform.position, targetPosition, fallSpeed * Time.deltaTime);
            yield return null;
        }

        DealDamage(targetPosition);
        Destroy(arrow);
    }

   void DealDamage(Vector3 position)
{
    Collider[] hitColliders = Physics.OverlapSphere(position, arrowDamageRadius, targetLayer);

    foreach (Collider collider in hitColliders)
    {
        if (collider.CompareTag("Demon") || collider.CompareTag("Minion"))
        {
            // Check if this specific game object has already been damaged
            if (!alreadyDamagedObjects.Contains(collider.gameObject))
            {
                alreadyDamagedObjects.Add(collider.gameObject); // Mark the game object as damaged

                if (collider.CompareTag("Minion"))
                {
                    minionhealthbar minionHealth = collider.GetComponent<minionhealthbar>();
                    UnityEngine.AI.NavMeshAgent agent = collider.GetComponent<UnityEngine.AI.NavMeshAgent>(); // Get the NavMeshAgent component
                    
                    if (minionHealth != null && agent != null)
                    {
                        minionHealth.TakeDamage(arrowDamage);
                        Debug.Log($"Arrow hit minion {collider.name} for {arrowDamage} damage!");

                        // Slow down the minion's movement
                        SlowDownAgent(agent);
                    }
                }

                if (collider.CompareTag("Demon"))
                {
                    demonhealthbar demonHealth = collider.GetComponent<demonhealthbar>();
                    UnityEngine.AI.NavMeshAgent agent = collider.GetComponent<UnityEngine.AI.NavMeshAgent>(); // Get the NavMeshAgent component
                    
                    if (demonHealth != null && agent != null)
                    {
                        demonHealth.TakeDamage(arrowDamage);
                        Debug.Log($"Arrow hit demon {collider.name} for {arrowDamage} damage!");

                        // Slow down the demon's movement
                        SlowDownAgent(agent);
                    }
                }
            }
        }
    }
}

    void SlowDownAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        float originalSpeed = agent.speed; // Save the original speed
        agent.speed = originalSpeed * 0.25f; // Reduce speed to a quarter

        // After 3 seconds, restore the original speed
        StartCoroutine(RestoreSpeed(agent, originalSpeed));
    }

    IEnumerator RestoreSpeed(UnityEngine.AI.NavMeshAgent agent, float originalSpeed)
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds

        agent.speed = originalSpeed; // Restore original speed
        Debug.Log("Enemy speed restored to normal.");
    }

}