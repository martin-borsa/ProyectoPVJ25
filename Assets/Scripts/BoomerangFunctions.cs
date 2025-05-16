using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangFunctions : MonoBehaviour
{
    private enum BoomerangState { Rest, Launched, Returning, Melee }
    private BoomerangState state = BoomerangState.Rest;

    [Header("References")]
    public GameObject restBoomerang;
    public GameObject launchBoomerang;
    public GameObject meleeBoomerang;
    public Collider catchZoneCollider;

    [Header("Movement Settings")]
    public float launchSpeed = 10f;
    public float returnSpeed = 8f;
    public float maxDistance = 15f;
    public float collisionDetectRadius = 0.2f;
    public LayerMask collisionLayers;
    public float catchDistance = 0.5f;

    [Header("Rest Position Offset")]
    public Vector3 restOffset = new Vector3(0, 0, -1);

    [Header("Launch Offset")]
    public Vector3 launchOffset = new Vector3(0, 0, 1);

    [Header("Melee Settings")]
    [Tooltip("Cooldown (seconds) después de un ataque melee")]
    public float meleeCooldown = 2f;
    private float meleeCooldownTimer = 0f;

    private bool rightClicked = false;
    private Vector3 launchDirection;
    private Vector3 launchStartPosition;
    private Camera mainCamera;

    void Start()
    {
        state = BoomerangState.Rest;
        restBoomerang.SetActive(true);
        launchBoomerang.SetActive(false);
        meleeBoomerang.SetActive(false);
        restBoomerang.transform.localPosition = restOffset;
        restBoomerang.transform.localRotation = Quaternion.identity;
        if (catchZoneCollider != null)
            catchZoneCollider.enabled = true;

        mainCamera = Camera.main;
    }

    void Update()
    {
        if (meleeCooldownTimer > 0f)
            meleeCooldownTimer -= Time.deltaTime;

        switch (state)
        {
            case BoomerangState.Rest:
                restBoomerang.transform.localPosition = restOffset;
                restBoomerang.transform.localRotation = Quaternion.identity;

                if (Input.GetMouseButtonDown(1)) rightClicked = true;
                if (Input.GetMouseButtonUp(1)) rightClicked = false;

                if (rightClicked && Input.GetMouseButtonDown(0))
                    Launch();
                else if (Input.GetMouseButtonDown(0) && meleeCooldownTimer <= 0f && !rightClicked)
                    MeleeAttack();
                break;

            case BoomerangState.Melee:
                break;
            case BoomerangState.Launched:
            case BoomerangState.Returning:
                MoveLaunchBoomerang();
                if (Input.GetMouseButtonUp(1)) rightClicked = false;
                break;
        }
    }

    private void Launch()
    {
        state = BoomerangState.Launched;
        restBoomerang.SetActive(false);
        launchBoomerang.SetActive(true);
        launchBoomerang.transform.SetParent(null);
        launchBoomerang.transform.rotation = Quaternion.identity;

        // Posicionar boomerang en el offset
        Vector3 startPos = transform.position + transform.TransformVector(launchOffset);
        launchBoomerang.transform.position = startPos;
        launchStartPosition = startPos;

        // Calcular punto objetivo en el centro de la pantalla
        Vector3 targetPoint;
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, collisionLayers))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * maxDistance;
        }
        launchDirection = (targetPoint - startPos).normalized;

        if (catchZoneCollider != null)
            catchZoneCollider.enabled = false;
    }

    private void MeleeAttack()
    {
        state = BoomerangState.Melee;
        meleeBoomerang.SetActive(true);
        meleeCooldownTimer = meleeCooldown;
        StartCoroutine(EndMeleeAttackAfterAnimation());
    }

    private IEnumerator EndMeleeAttackAfterAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        meleeBoomerang.SetActive(false);
        state = BoomerangState.Rest;
    }

    private void MoveLaunchBoomerang()
    {
        Vector3 currentPos = launchBoomerang.transform.position;

        if (state == BoomerangState.Launched)
        {
            launchBoomerang.transform.position = currentPos + launchDirection * launchSpeed * Time.deltaTime;
            bool hitEnv = Physics.CheckSphere(currentPos, collisionDetectRadius, collisionLayers);
            float traveled = Vector3.Distance(launchStartPosition, currentPos);
            if (hitEnv || traveled >= maxDistance)
            {
                state = BoomerangState.Returning;
                if (catchZoneCollider != null)
                    catchZoneCollider.enabled = true;
            }
        }
        else if (state == BoomerangState.Returning)
        {
            Vector3 returnDir = (transform.position - currentPos).normalized;
            launchBoomerang.transform.position = currentPos + returnDir * returnSpeed * Time.deltaTime;
            if (Vector3.Distance(currentPos, transform.position) <= catchDistance)
            {
                CatchBoomerang();
            }
        }
    }

    private void CatchBoomerang()
    {
        state = BoomerangState.Rest;
        launchBoomerang.SetActive(false);
        restBoomerang.SetActive(true);
        restBoomerang.transform.localPosition = restOffset;
        restBoomerang.transform.localRotation = Quaternion.identity;
        launchBoomerang.transform.SetParent(transform);
        launchBoomerang.transform.localPosition = launchOffset;
        if (catchZoneCollider != null)
            catchZoneCollider.enabled = true;
    }
}