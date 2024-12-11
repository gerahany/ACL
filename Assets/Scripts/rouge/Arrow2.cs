using UnityEngine;

public class Arrow2 : MonoBehaviour
{
    public float speed = 8f; // Speed of the arrow
    public float damage = 15f; // Damage dealt to the enemy upon impact
    public float lifetime = 10f; // Lifetime of the arrow

    private Vector3 direction;
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Destroy arrow after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector3 targetDirection)
    {
        direction = targetDirection.normalized; // Normalize the direction vector for consistent speed
    }

    void Update()
    {
        // If we have a Rigidbody, set the velocity
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            // In case there's no Rigidbody (fallback to position update)
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check for valid collision
        if (collision.gameObject.CompareTag("Demon") || collision.gameObject.CompareTag("Minion"))
        {
            Debug.Log($"Arrow hit: {collision.gameObject.name}");

            // Deal damage if the collider is a Minion
            if (collision.gameObject.CompareTag("Minion"))
            {
                minionhealthbar minionHealth = collision.gameObject.GetComponent<minionhealthbar>();
                if (minionHealth != null)
                {
                    minionHealth.TakeDamage(5);
                    Debug.Log($"Arrow dealt {damage} damage to minion: {collision.gameObject.name}");
                }
            }

            // Deal damage if the collider is a Demon
            if (collision.gameObject.CompareTag("Demon"))
            {
                demonhealthbar demonHealth = collision.gameObject.GetComponent<demonhealthbar>();
                if (demonHealth != null)
                {
                    demonHealth.TakeDamage(5);
                    Debug.Log($"Arrow dealt {damage} damage to demon: {collision.gameObject.name}");
                }
            }

            // Destroy the arrow after hitting a valid target
            Destroy(gameObject);
        }

        // Check if the arrow hits other valid objects
        if (collision.gameObject.CompareTag("Breakable") || collision.gameObject.CompareTag("Flooring"))
        {
            Debug.Log($"Arrow collided with {collision.gameObject.name}");
            Destroy(gameObject);
        }
    }
}
