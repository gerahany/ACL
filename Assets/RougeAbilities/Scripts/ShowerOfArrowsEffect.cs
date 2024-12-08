using UnityEngine;

public class ShowerOfArrowsEffect : MonoBehaviour
{
    public GameObject arrowRainPrefab; // A visual prefab to represent raining arrows
    public float areaRadius = 5f;
    public float damagePerSecond = 10f;
    public float slowDuration = 3f;
    public float slowMultiplier = 0.25f;
    public float duration = 3f;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Example keybinding for ability
        {
            ActivateShowerOfArrows();
        }
    }

    void ActivateShowerOfArrows()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            GameObject arrowRain = Instantiate(arrowRainPrefab, targetPosition, Quaternion.identity);
            arrowRain.transform.localScale = new Vector3(areaRadius * 2, 1, areaRadius * 2);

            Collider[] enemies = Physics.OverlapSphere(targetPosition, areaRadius, LayerMask.GetMask("Enemy"));
            foreach (Collider enemy in enemies)
            {
                StartCoroutine(SlowEnemy(enemy.gameObject));
            }

            Destroy(arrowRain, duration);
        }
    }

    System.Collections.IEnumerator SlowEnemy(GameObject enemy)
    {
        // Example of slowing an enemy
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.SetSpeedMultiplier(slowMultiplier);
            yield return new WaitForSeconds(slowDuration);
            enemyController.ResetSpeed();
        }
    }
}
