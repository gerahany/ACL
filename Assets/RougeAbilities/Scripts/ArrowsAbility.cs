using System.Collections;
using UnityEngine;

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
    public float abilityCooldown = 10f;

    public GameObject arrowRingPrefab;

    void Update()
    {
        // Only activate ability if no other ability is active
        if (Input.GetKeyDown(KeyCode.E) && canUseAbility && !AbilityManager.IsAbilityActive())
        {
            StartSelectingPosition();
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

        while (elapsedTime < arrowRainDuration)
        {
            SpawnArrow(targetPosition);
            elapsedTime += arrowSpawnInterval;
            yield return new WaitForSeconds(arrowSpawnInterval);
        }

        Destroy(arrowRing);
        AbilityManager.SetAbilityActive(false); // Mark ability as inactive

        yield return new WaitForSeconds(abilityCooldown);
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

    void DealDamage(Vector3 hitPosition)
    {
        Collider[] hitTargets = Physics.OverlapSphere(hitPosition, arrowDamageRadius, targetLayer);
        foreach (Collider target in hitTargets)
        {
            Debug.Log("Damaged: " + target.name);
        }
    }
}