using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangFunctions : MonoBehaviour
{
    private enum BoomerangState { Rest, Launched, Returning }
    private BoomerangState state = BoomerangState.Rest;

    [Header("References")]
    [Tooltip("Objeto boomerang en reposo (child del Player)")]
    public GameObject restBoomerang;
    [Tooltip("Objeto boomerang que se lanzará (inicialmente inactivo)")]
    public GameObject launchBoomerang;
    [Tooltip("Collider trigger usado para atrapar el boomerang (catch zone), child del Player")]
    public Collider catchZoneCollider;

    [Header("Movement Settings")]
    [Tooltip("Velocidad de lanzamiento (m/s)")]
    public float launchSpeed = 10f;
    [Tooltip("Velocidad de retorno (m/s)")]
    public float returnSpeed = 8f;
    [Tooltip("Distancia máxima antes de iniciar retorno (m)")]
    public float maxDistance = 15f;
    [Tooltip("Radio para detectar colisión durante el vuelo")]
    public float collisionDetectRadius = 0.2f;
    [Tooltip("Capas con las que el boomerang colisionará durante el vuelo")]
    public LayerMask collisionLayers;
    [Tooltip("Distancia mínima al Player para capturar sin collider")]
    public float catchDistance = 0.5f;

    [Header("Rest Position Offset")]
    [Tooltip("Offset local desde el Player para el boomerang en reposo")]
    public Vector3 restOffset = new Vector3(0, 0, -1);

    [Header("Launch Offset")]
    [Tooltip("Offset local desde el Player donde aparece el boomerang al lanzarse")]
    public Vector3 launchOffset = new Vector3(0, 0, 1);

    // Estado interno
    private Vector3 launchDirection;
    private Vector3 launchStartPosition;

    void Start()
    {
        state = BoomerangState.Rest;
        restBoomerang.SetActive(true);
        launchBoomerang.SetActive(false);
        restBoomerang.transform.localPosition = restOffset;
        restBoomerang.transform.localRotation = Quaternion.identity;
        if (catchZoneCollider != null)
            catchZoneCollider.enabled = true;
    }

    void Update()
    {
        switch (state)
        {
            case BoomerangState.Rest:
                // Mantener rest en posición y sin rotación local
                restBoomerang.transform.localPosition = restOffset;
                restBoomerang.transform.localRotation = Quaternion.identity;
                if (Input.GetMouseButtonDown(0))
                    Launch();
                break;

            case BoomerangState.Launched:
            case BoomerangState.Returning:
                MoveLaunchBoomerang();
                break;
        }
    }

    private void Launch()
    {
        state = BoomerangState.Launched;
        restBoomerang.SetActive(false);
        launchBoomerang.SetActive(true);
        // Desparentar para moverse libremente
        launchBoomerang.transform.SetParent(null);
        // Resetear rotación antes de lanzar
        launchBoomerang.transform.rotation = Quaternion.identity;
        // Posicionar en el offset de lanzamiento
        launchBoomerang.transform.position = transform.position + transform.TransformVector(launchOffset);
        launchDirection = transform.forward;
        launchStartPosition = launchBoomerang.transform.position;
        if (catchZoneCollider != null)
            catchZoneCollider.enabled = false;
    }

    private void MoveLaunchBoomerang()
    {
        Vector3 currentPos = launchBoomerang.transform.position;

        if (state == BoomerangState.Launched)
        {
            launchBoomerang.transform.position = currentPos + launchDirection * launchSpeed * Time.deltaTime;
            bool hitEnv = Physics.CheckSphere(currentPos, collisionDetectRadius, collisionLayers);
            float traveled = Vector3.Distance(launchStartPosition, launchBoomerang.transform.position);
            if (hitEnv || traveled >= maxDistance)
            {
                state = BoomerangState.Returning;
                if (catchZoneCollider != null)
                    catchZoneCollider.enabled = true;
            }
        }
        else if (state == BoomerangState.Returning)
        {
            // Volver al Player
            Vector3 returnDir = (transform.position - currentPos).normalized;
            launchBoomerang.transform.position = currentPos + returnDir * returnSpeed * Time.deltaTime;
            // Captura por proximidad
            if (Vector3.Distance(currentPos, transform.position) <= catchDistance)
            {
                CatchBoomerang();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == BoomerangState.Returning && other.gameObject == launchBoomerang)
        {
            CatchBoomerang();
        }
    }

    private void CatchBoomerang()
    {
        state = BoomerangState.Rest;
        launchBoomerang.SetActive(false);
        restBoomerang.SetActive(true);
        restBoomerang.transform.localPosition = restOffset;
        restBoomerang.transform.localRotation = Quaternion.identity;
        // Volver a parent
        launchBoomerang.transform.SetParent(transform);
        launchBoomerang.transform.localPosition = launchOffset;
        if (catchZoneCollider != null)
            catchZoneCollider.enabled = true;
    }
}
