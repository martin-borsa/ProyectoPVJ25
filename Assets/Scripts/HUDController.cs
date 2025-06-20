using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Referencias UI")]
    public Slider barraVida;
    public Slider barraCarga;
    public TextMeshProUGUI textoMonedas;
    public GameObject crosshair;
    public TextMeshProUGUI textoMuerte;
    public Player player;

    [Header("Valores simulados")]
    public int vida = 100;
    public int monedas = 0;
    private float carga = 0f;

    void Start()
    {
        barraVida.maxValue = 100;
        barraCarga.minValue = 1f;
        barraCarga.maxValue = 2f;
        barraCarga.value = 0f;
        barraCarga.gameObject.SetActive(false);
        crosshair.gameObject.SetActive(false);
        textoMuerte.gameObject.SetActive(false);
       
    }

    void Update()
    {
        ActualizarHUD();

        // Simulación de daño y recolección
        if (Input.GetKeyDown(KeyCode.H)) vida -= 10;
        if (Input.GetKeyDown(KeyCode.M)) monedas += 1;

        if (Input.GetMouseButtonDown(1)) // clic derecho presionado
            crosshair.SetActive(true);

        if (Input.GetMouseButtonUp(1))   // clic derecho soltado
            crosshair.SetActive(false);
    }
    public void ActualizarVida(int vida)
    {
        barraVida.value = vida;
    }
    public void ActualizarMonedas()
    {
        int coins = player.CoinAmount;
        textoMonedas.text = "Monedas: " + coins;
    }
    // Llamar para encender la barra antes de empezar a cargar
    public void ShowChargeBar()
    {
        barraCarga.value = 0f;
        barraCarga.gameObject.SetActive(true);
    }

    // Llamar mientras cargas para actualizar el llenado (0–1)
    public void UpdateChargeBar(float normalized)
    {
        float porcentaje = Mathf.Clamp01(normalized) * barraCarga.maxValue;
        barraCarga.value = porcentaje;
    }

    // Llamar al soltar el botón de carga
    public void HideChargeBar()
    {
        barraCarga.gameObject.SetActive(false);
        barraCarga.value = 0f;
    }
    void ActualizarHUD()
    {
        barraVida.value = vida;
        barraCarga.value = carga;
        textoMonedas.text = "Monedas: " + player.CoinAmount;
    }

    private void ShowDeathScreen(GameObject player)
    {
        textoMuerte.gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        GameEvents.OnPlayerDeath += ShowDeathScreen;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDeath -= ShowDeathScreen;
    }
}
