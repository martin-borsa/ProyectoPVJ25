using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damageAmount = 10;
    public string targetTag = "Player";

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
           other.gameObject.GetComponent<IDamage>().TakeDamage(damageAmount);
            
            
        }
    }
}
