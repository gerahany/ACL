using UnityEngine;
using System.Collections;
using TMPro;

public class RogueAbilities : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public Animator rogueAnimator;
    public float raycastRange = 100f;
    private Vector3 targetPosition;
    public TMP_Text cooldownText;
    private bool canShootArrow = true;
    private bool isCooldown = false;
    private float currentCooldownTime = 0f;
    private float arrowCooldown = 1f;

    void Start()
    {
        cooldownText.text = "OK";
    }

    void Update()
    {
        // Only activate ability if no other ability is active
        if (Input.GetMouseButtonDown(1) && canShootArrow)
        {
            AttemptArrow();
        }
        if (isCooldown)
        {
            currentCooldownTime += Time.deltaTime; // Increase cooldown time

            float timeLeft = 1 - currentCooldownTime; // Calculate remaining time

            // Update the TMP text to show the remaining cooldown time
            cooldownText.text = Mathf.Ceil(timeLeft) + "s"; // Round and show seconds

            if (currentCooldownTime >= 1)
            {
                //canUseAbility = true; // Reset ability to be ready
                currentCooldownTime = 0f; // Reset cooldown time
                cooldownText.text = "OK"; // Show "OK" when cooldown is complete
                Debug.Log("Shower of Arrows ability is ready.");
            }
        }
    }

    void AttemptArrow()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange))
        {
            if ((hit.collider.CompareTag("Demon") || hit.collider.CompareTag("Minion")) && hit.collider.gameObject != gameObject)
            {
                targetPosition = hit.point;

                Debug.Log($"Target Position: {targetPosition}");
                Debug.Log($"Arrow Spawn Point Position: {arrowSpawnPoint.position}");

                Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                directionToTarget.y = 0;
                transform.forward = directionToTarget;

                rogueAnimator.SetTrigger("shootArrow");

                StartCoroutine(CooldownArrow());

                StartCoroutine(SpawnArrowWithDelay(0.6f));
            }
        }
    }

    private IEnumerator SpawnArrowWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnArrow();
    }

    void SpawnArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);

        Collider arrowCollider = arrow.GetComponent<Collider>();
        Collider rogueCollider = GetComponent<Collider>();
        if (arrowCollider != null && rogueCollider != null)
        {
            Physics.IgnoreCollision(arrowCollider, rogueCollider);
        }

        Vector3 direction = (new Vector3(targetPosition.x, arrowSpawnPoint.position.y, targetPosition.z) - arrowSpawnPoint.position).normalized;

        Debug.Log($"Arrow Direction: {direction}");

        Quaternion rotation = Quaternion.LookRotation(direction);
        rotation *= Quaternion.Euler(0f, -90f, 0f);

        arrow.transform.rotation = rotation;

        arrow.GetComponent<Rigidbody>().AddForce(direction * 10f, ForceMode.Impulse);
    }

    private IEnumerator CooldownArrow()
    {
        AbilityManager.SetAbilityActive(true); // Mark ability as active
        canShootArrow = false;
        isCooldown=true;
        Debug.Log("before cool");
        yield return new WaitForSeconds(arrowCooldown);
        Debug.Log("after cool");
        isCooldown=false;
        currentCooldownTime = 0f; // Reset cooldown time
        cooldownText.text = "OK";
        canShootArrow = true;

    }
}
