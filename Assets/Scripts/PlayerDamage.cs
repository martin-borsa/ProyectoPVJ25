using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    public bool melee = false;
    public int damageAmount = 25;
    private float damageAmountMelee;
    public string targetTag = "Enemy";
    public GameObject player;
    private BoomerangFunctions boomerangFunctions;

    void Start()
    {
        if (melee)
        {
            boomerangFunctions = player.GetComponent<BoomerangFunctions>();
            if (boomerangFunctions == null)
                Debug.LogWarning("BoomerangFunctions no encontrado en " + gameObject.name);
        }
    }

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
            var health = other.GetComponent<IDamage>();
            if (health != null)
            {
                if (melee == true && boomerangFunctions != null)
                {
                    damageAmountMelee = damageAmount * boomerangFunctions.multi;
                    health.TakeDamage((int)damageAmountMelee);
                }

                else
                {
                    health.TakeDamage(damageAmount);
                }
            }
        }
    }
}