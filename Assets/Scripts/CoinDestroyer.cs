using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CoinDestoryer : MonoBehaviour
{
    public HUDController hud;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Moneda"))
        {
            if (hud != null)
                hud.ActualizarMonedas();

            Destroy(other.gameObject);
        }
    }
}