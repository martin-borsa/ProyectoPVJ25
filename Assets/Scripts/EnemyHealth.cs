using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public GameObject deathPrefab;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibió {amount} de daño. Vida restante: {currentHealth}");
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("Un enemigo murio");
        if (deathPrefab != null)
            Instantiate(deathPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}

