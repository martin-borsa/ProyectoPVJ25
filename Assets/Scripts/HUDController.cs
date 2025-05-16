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

    [Header("Valores simulados")]
    public int vida = 100;
    public int monedas = 0;
    private float carga = 0f;

    void Start()
    {
        barraVida.maxValue = 100;
        barraCarga.maxValue = 100;
        ActualizarHUD();
    }

    void Update()
    {
        // Simulación de daño y recolección
        if (Input.GetKeyDown(KeyCode.H)) vida -= 10;
        if (Input.GetKeyDown(KeyCode.M)) monedas += 1;

        // Simulación de carga
        if (Input.GetKey(KeyCode.C))
        {
            carga += Time.deltaTime * 20f; // cargar con el tiempo
            if (carga > 100f) carga = 100f;
        }
        else
        {
            carga -= Time.deltaTime * 10f; // descarga lenta
            if (carga < 0f) carga = 0f;
        }
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
        monedas++;
        textoMonedas.text = "Monedas: " + monedas;
    }
    void ActualizarHUD()
    {
        barraVida.value = vida;
        barraCarga.value = carga;
        textoMonedas.text = "Monedas: " + monedas;
    }
}
