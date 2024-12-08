using UnityEngine;

public class SmokeBombEffect : MonoBehaviour
{
    public float stunDuration = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Minion") || other.CompareTag("Enemy")) 
        {
            other.GetComponent<EnemyInteractions>()?.Stun(stunDuration);
        }
    }

    private void Start()
    {
        Destroy(gameObject, stunDuration); // Destroy smoke bomb after the duration
    }
}
