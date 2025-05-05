using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health_Damage : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 100;
    private int currentHealth;
    public bool isPlayer = false;
    public GameObject deathPrefab;        // S�lo para enemigos

    [Header("Da�o (si aplica)")]
    public int damageAmount = 0;          // 0 si no inflige da�o
    public string targetTag = "";       // Tag del objetivo

    private CharacterController charController;

    void Awake()
    {
        currentHealth = maxHealth;
        if (isPlayer)
        {
            charController = GetComponent<CharacterController>();
            if (charController == null)
                Debug.LogWarning("No se encontr� CharacterController en el jugador.");
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
        // 1) Infligir da�o si aplica
        if (damageAmount > 0 && other.CompareTag(targetTag))
        {
            var hd = other.GetComponent<Health_Damage>();
            if (hd != null) hd.TakeDamage(damageAmount);
        }
        // 2) Recibir da�o si otro ataca
        var attacker = other.GetComponent<Health_Damage>();
        if (attacker != null && attacker.damageAmount > 0 && CompareTag(attacker.targetTag))
        {
            TakeDamage(attacker.damageAmount);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} recibi� {amount} de da�o. Vida restante: {currentHealth}");
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (isPlayer)
        {
            // Mensaje inicial de muerte
            Debug.Log("El jugador murio");
            // Deshabilitar control de movimiento
            if (charController != null) charController.enabled = false;
            // Rotar 90� en Y
            transform.Rotate(0f, 90f, 0f);
            // Iniciar cuenta atr�s y reinicio de escena
            StartCoroutine(PlayerDeathCountdown());
        }
        else
        {
            Debug.Log("Un enemigo murio");
            if (deathPrefab != null)
                Instantiate(deathPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

    private IEnumerator PlayerDeathCountdown()
    {
        int timer = 5;
        while (timer > 0)
        {
            Debug.Log($"Reiniciando en {timer}...");
            yield return new WaitForSeconds(1f);
            timer--;
        }
        // Reiniciar escena actual sin eliminar el objeto jugador
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
