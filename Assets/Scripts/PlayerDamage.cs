using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public int damageAmount = 25;
    public string targetTag = "Enemy";

    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        if (other.CompareTag(targetTag))
        {
            var health = other.GetComponent<EnemyHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
    }
}