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
        if (collision.gameObject.CompareTag("Demon") || collision.gameObject.CompareTag("Minion") || collision.gameObject.CompareTag("MinionBoss") || collision.gameObject.CompareTag("Lilith") || collision.gameObject.CompareTag("Shield") || collision.gameObject.CompareTag("Aura"))
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

            // Deal damage if the collider is a MinionBoss
            if (collision.gameObject.CompareTag("MinionBoss"))
            {
                MinionHealthbarBoss minionHealthBoss = collision.gameObject.GetComponent<MinionHealthbarBoss>();
                if (minionHealthBoss != null)
                {
                    minionHealthBoss.TakeDamage(5);
                    Debug.Log($"Arrow dealt {damage} damage to MinionBoss: {collision.gameObject.name}");
                }
            }

            // Deal damage if the collider is a Lilith
            if (collision.gameObject.CompareTag("Lilith"))
            {
                BossBehavior bossBehavior = collision.gameObject.GetComponent<BossBehavior>();
                if (bossBehavior != null)
                {
                    bossBehavior.TakeDamage(5);
                    Debug.Log($"Arrow dealt {damage} damage to Lilith: {collision.gameObject.name}");
                }
            }

            // Deal damage if the collider is a Shield
            if (collision.gameObject.CompareTag("Shield"))
            {
                ShieldHealthbar shieldHealth = collision.gameObject.GetComponent<ShieldHealthbar>();
                if (shieldHealth != null)
                {
                    shieldHealth.TakeDamage(5);
                    Debug.Log($"Arrow dealt {damage} damage to Shield: {collision.gameObject.name}");
                }
            }

            // Deal damage if the collider is an Aura
            if (collision.gameObject.CompareTag("Aura"))
            {
                aurascript aura = collision.gameObject.GetComponent<aurascript>();
                if (aura != null)
                {
                    aura.TakeAuraDamage(20);
                    Debug.Log($"Arrow dealt {damage} damage to Aura: {collision.gameObject.name}");
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
