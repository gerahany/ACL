using UnityEngine;

public class Arrow : MonoBehaviour
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
        // Destroy the arrow on valid collision
        if (collision.gameObject.CompareTag("Demon") || collision.gameObject.CompareTag("Minion") ||
            collision.gameObject.CompareTag("Breakable") || collision.gameObject.CompareTag("Flooring"))
        {
            Destroy(gameObject);
        }
    }
}
