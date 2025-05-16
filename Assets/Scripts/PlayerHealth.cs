using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public HUDController hud; // ? Drag en el Inspector
    public int maxHealth = 100;
    private int currentHealth;

    private CharacterController charController;

    void Awake()
    {
        currentHealth = maxHealth;
        charController = GetComponent<CharacterController>();
        if (charController == null)
            Debug.LogWarning("No se encontró CharacterController en el jugador.");
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (hud != null)
        {
            hud.ActualizarVida(currentHealth);
        }
        Debug.Log($"El jugador recibió {amount} de daño. Vida restante: {currentHealth}");
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Debug.Log("El jugador murio");
        if (charController != null) charController.enabled = false;
        transform.Rotate(0f, 90f, 0f);
        StartCoroutine(PlayerDeathCountdown());
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
